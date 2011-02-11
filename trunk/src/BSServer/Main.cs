using System;
using Ninject;
using JollyBit.BS.Core;
using Ninject.Modules;
using JollyBit.BS.Server.Networking;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.World;

namespace JollyBit.BS.Server
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WindowWidth = 100;
			Constants.Kernel = new StandardKernel();
			Constants.Kernel.Load(new BSServerNinjectModule());
            Server server = new Server();
			Constants.Kernel.Bind<ITimeService>().ToConstant(server);
			Constants.Kernel.Get<IStartupService>().ActivateStartupTypes(); //Start all the services
			IConnectionManager connectionManager = Constants.Kernel.Get<IConnectionManager>();
			connectionManager.StartListeningForConnections();
			server.Start();

			// Generate a world
			IMap map = Constants.Kernel.Get<IMap>();
			IChunk c;
			for(int i = 0; i < 2; i++) {
				for(int j = 0; j < 2; j++) {
					c = map[new Point3L(i*Constants.CHUNK_SIZE_X-1, 0, j*Constants.CHUNK_SIZE_Z-1)];
				}
			}

            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
            Constants.Kernel.Dispose();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
	}
}

