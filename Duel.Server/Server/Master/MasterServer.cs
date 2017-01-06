using System.Reflection;
using x2;

namespace Server.Master
{
    public class MasterServer : AsyncTcpServer
    {
        Config config; 
        public MasterServer(Config cfg)
            : base(cfg.Name)
        {
            config = cfg;
        }

        protected override void Setup()
        {
            base.Setup();

            InitializeFactoryEvents();
            InitializeBinds();

            Listen(config.Port);
        }

        void InitializeFactoryEvents()
        {
            EventFactory.Register(Assembly.Load(config.EventAssembly));
        }
        
        void InitializeBinds()
        {
            // Send or Multicast when received
            new Event().Bind(Send);
        }
    }
}
