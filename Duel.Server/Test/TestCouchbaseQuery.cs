using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using x2;
using Couchbase;

namespace Test
{
    [TestFixture]
    class TestCouchbaseQuery
    {
        public class Person
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int Age { get; set; }
        }


        [Test]
        public void TestBasicQuery()
        {
            Cluster cluster = new Cluster();

            using (var bucket = cluster.OpenBucket())
            {
                var document = new Document<Person>
                {
                    Id = "P1",
                    Content = new Person
                    {
                        FirstName = "John",
                        LastName = "Adams",
                        Age = 21
                    }
                };

                var result = bucket.Insert(document);

                if (result.Success)
                {
                    Console.WriteLine("Inserted document '{0}'", document.Id);

                    result = bucket.GetDocument<Person>("P1");
                    if (result.Success)
                    {
                        var person = result.Content;

                        Console.WriteLine(
                            "Retrieved document '{0}': {1} {2}", 
                            result.Id, person.FirstName, person.LastName
                        );
                    }
                } 
            } 
        }

        [Test]
        public void TestClusterHelper()
        {
            var config = new Couchbase.Configuration.Client.ClientConfiguration
            {
                Servers = new List<Uri>
                   {
                       new Uri("http://localhost:8091/")
                   }
            };

            ClusterHelper.Initialize(config);

            for (int i = 0; i < 100; ++i)
            {
                string key = string.Format("Key{0}", i + 1);

                var bucket = ClusterHelper.GetBucket("default");
                {
                    var document = new Document<Person>
                    {
                        Id = key,
                        Content = new Person
                        {
                            FirstName = "John",
                            LastName = "Adams",
                            Age = 21
                        }
                    };

                    var result = bucket.Upsert(document);

                    if (result.Success)
                    {
                        Console.WriteLine("Inserted document '{0}'", document.Id);

                        result = bucket.GetDocument<Person>(key);
                        if (result.Success)
                        {
                            var person = result.Content;

                            Console.WriteLine(
                                "Retrieved document '{0}': {1} {2}",
                                result.Id, person.FirstName, person.LastName
                            );
                        }
                    }
                }
            }

            ClusterHelper.Close();
            

            // Perf: 100,000 / 2 sec. 빠르당.  
        }

        [Test]
        public void TestMultipleFlows()
        {
            var config = new Couchbase.Configuration.Client.ClientConfiguration
            {
                Servers = new List<Uri>
                   {
                       new Uri("http://localhost:8091/")
                   }
            }; 

            ClusterHelper.Initialize(config);

            var mf = new MultiThreadFlow(10);
            var sf = new SingleThreadFlow();

            var tc = new DbTesterCase();
            tc.TestCount = 100;

            mf.Add(new Server.Couchbase.UserDatabaseCase("default"));
            sf.Add(tc);

            Hub.Instance.Attach(mf);
            Hub.Instance.Attach(sf);

            Hub.Startup();

            Thread.Sleep(100);

            for (int i = 0; i < tc.TestCount; ++i)
            {
                tc.Request();
            }

            while ( (tc.ProcessCount + tc.FailCount) < tc.TestCount)
            {
                Thread.Sleep(10);
            }

            Assert.IsTrue(tc.ProcessCount == tc.TestCount);

            Hub.Shutdown();

            Hub.Instance.DetachAll();

            ClusterHelper.Close();

            // Couchbase 성능이 C#에서 C++보다 더 좋다. 
            // 내부 라이브러리 의존성이거나 그 전에 C++로 제대로 구현하지 못 했던 것 같다. 
            // 1만건 로드에 1초 안 걸림. 초당 5만건 정도. 
        }

        class DbTesterCase : Case
        {
            public int TestCount { get; set; }

            public int ProcessCount { get; private set; }

            public int FailCount{ get; private set; }

            int requestCount;

            public void Request()
            {
                if ( requestCount < TestCount )
                {
                    ++requestCount;

                    new Events.Database.EventCreateOrLoadUserReq
                    {
                        Account = string.Format("Test{0}", requestCount),
                        Password = string.Format("Test{0}", requestCount),
                        Context = requestCount
                    }
                    .Post();
                }
            }

            protected override void Setup()
            {
                base.Setup();

                for ( int i=1; i<=TestCount; ++i)
                {
                    new Events.Database.EventLoadUserResp
                    {
                        Account = string.Format("Test{0}", i),
                        Context = i
                    }
                    .Bind(OnUserLoaded);
                }
            }

            void OnUserLoaded(Events.Database.EventLoadUserResp resp)
            {
                if (resp.Result == 0)
                {
                    ProcessCount++;
                }
                else
                {
                    FailCount++;
                }
            }

        }
    }
}
