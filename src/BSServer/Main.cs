using System;
using Ninject;
using JollyBit.BS.Core;
using Ninject.Modules;
using JollyBit.BS.Server.Networking;
using JollyBit.BS.Core.Networking;

namespace JollyBit.BS.Server
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            Constants.Kernel = new StandardKernel();
            Constants.Kernel.Load(new INinjectModule[] { new BSCoreNinjectModule(), new BSServerNinjectModule() });
            IConnectionManager<object> connectionManager = Constants.Kernel.Get<IConnectionManager<object>>();
            connectionManager.StartListeningForConnections();
            Server s = new Server();
            s.TickInterval = 1;
            s.Tick += new EventHandler<Core.Utility.TimeTickEventArgs>(s_Tick);
            s.Start();
            Console.ReadLine();
		}

        static void s_Tick(object sender, Core.Utility.TimeTickEventArgs e)
        {
            Console.WriteLine("Server tick time={0} elapsed={1}", e.CurrentTime, e.ElapsedTime);
        }
	}
}

