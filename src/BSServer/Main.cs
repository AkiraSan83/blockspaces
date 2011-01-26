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
            Console.ReadLine();
		}
	}
}

