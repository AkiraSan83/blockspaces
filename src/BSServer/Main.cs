using System;
using Ninject;
using JollyBit.BS.Core;
using Ninject.Modules;
using JollyBit.BS.Server.Networking;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Server
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            Constants.Kernel = new StandardKernel();
            Constants.Kernel.Load(new INinjectModule[] { new BSCoreNinjectModule(), new BSServerNinjectModule() });
            Server server = new Server();
            Constants.Kernel.Bind<ITimeService>().ToConstant(server);
            IConnectionManager<object> connectionManager = Constants.Kernel.Get<IConnectionManager<object>>();
            connectionManager.StartListeningForConnections();
            server.Start();
            Console.ReadLine();
		}
	}
}

