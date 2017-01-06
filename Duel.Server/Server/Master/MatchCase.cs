using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using x2;
using Events.Instance;
using Events.Timer;

namespace Server.Master
{
    /// <summary>
    /// 매칭을 관리한다. 
    /// 
    /// Matching Rule: 
    ///  - 큐에서 둘을 뽑아서 매칭해서 돌려준다. 
    ///  - 일정 시간 이상 대기하면 봇과 매칭한다. 
    ///  - 게임이 끝나면 다시 매칭 진행.
    /// </summary>
    public class MatchCase : Case
    {

        class Entry
        {
            public Member member;
            public DateTime time;
        }

        class Coord
        {
            public int serverId;
            public int count;
        }

        Queue<Entry> matchQueue;    // channel 구분 되도록 설정
        List<Coord> coords;
        Random rand;

        public int Zone { get; private set; }

        public int Timeout { get; set; }

        public MatchCase(int zone)
        {
            Zone = zone;
            matchQueue = new Queue<Entry>();
            coords = new List<Coord>();
            rand = new Random();

            Timeout = 10;           // 10 sec. default.
        }

        void OnMatchReq(EventMatchReq req)
        {
            // TODO: 사용자별 고유한 요청이 보장되어야 함

            matchQueue.Enqueue(
                new Entry
                {
                    member = req.Requester,
                    time = DateTime.Now
                }
            );

            Match();
        }

        void OnTimer(TimerMatch m)
        {
            Match();
        }

        void OnCoordStatus(EventCoordStatus status)
        {
            var coord = coords.FirstOrDefault(c => c.serverId == status.ServerId);

            if (coord == null)
            {
                coords.Add(
                    new Coord
                    {
                        serverId = status.ServerId,
                        count = status.Count
                    }
                 );
            }
            else
            {
                coord.count = status.Count;
            }
        }

        void Match()
        {
            // 비어 있으면 리턴
            if (matchQueue.Count == 0)
            {
                return;
            }

            // 처리할 서버가 없으면 리턴
            if (coords.Count == 0)
            {
                return;
            }

            // 짝이 맞으면 둘을 매칭
            while (matchQueue.Count >= 2)
            {
                var e1 = matchQueue.Dequeue();
                var e2 = matchQueue.Dequeue();

                MatchPair(e1.member, e2.member);
            }

            // 남은 요청 있으면 시간 확인해서 봇과 매칭
            if (matchQueue.Count >= 1)
            {
                var entry = matchQueue.Peek();

                var span = DateTime.Now - entry.time;

                if (span.Seconds >= Timeout)
                {
                    MatchSingle(entry.member);
                }
            }
        }

        void MatchPair(Member m1, Member m2)
        {
            var coord = coords[rand.Next(coords.Count)];

            new EventMatchResp
            {
                Zone = this.Zone,
                ServerId = coord.serverId,
                Result = (int)Events.ErrorCodes.Success,
                Members = new List<Member>
                {
                    m1,  
                    m2
                }
            }
            .Post();
        }

        void MatchSingle(Member m)
        {
            var bot = new Member
            {
                Account = GenerateBotAccount(),
                Nick = GenerateBotNick(),
                Gold = m.Gold * 3,
                Bot = true
            };

            MatchPair(m, bot);
        }

        string GenerateBotAccount()
        {
            return Guid.NewGuid().ToString();
        }

        string GenerateBotNick()
        {
            // TODO: Get random from a pregenerated bot names

            return "ProBot";
        }

        protected override void Setup()
        {
            base.Setup();

            new EventMatchReq
            {
                Zone = this.Zone
            }
            .Bind(OnMatchReq);

            new EventCoordStatus().Bind(OnCoordStatus);

            TimeFlow.Default.ReserveRepetition(
                new TimerMatch(),
                TimeSpan.FromMilliseconds(100)
            );

            new TimerMatch().Bind(OnTimer);
        }
    }
}
