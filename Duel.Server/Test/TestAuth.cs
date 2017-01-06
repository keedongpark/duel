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
    class TestAuth
    {
        [Test]
        public void TestAuthFlow()
        {
            var config = new Couchbase.Configuration.Client.ClientConfiguration
            {
                Servers = new List<Uri>
                   {
                       new Uri("http://localhost:8091/")
                   }
            };

            ClusterHelper.Initialize(config);

            var flow = new SingleThreadFlow();

            var cc = new Server.Client.ClientCase("temp", "temp");
            var lc = new Server.Session.LoginCase(flow);
            var ac = new Server.Master.AuthCase();
            var udc = new Server.Couchbase.UserDatabaseCase("default");

            flow.Add(cc);
            flow.Add(lc);
            flow.Add(ac);
            flow.Add(udc);

            Hub.Instance.Attach(flow);

            Hub.Startup();

            Thread.Sleep(100);

            cc.RequestLogin();

            while (!cc.HasLogin)
            {
                Thread.Sleep(10);
            } 

            Assert.IsTrue(cc.HasLogin);

            cc.RequestLogout();

            Hub.Shutdown();

            Hub.Instance.DetachAll();

            ClusterHelper.Close();
        }
    }
}
