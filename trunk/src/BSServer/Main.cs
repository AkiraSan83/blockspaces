using System;
using Ninject;
using JollyBit.BS.Core;
using Ninject.Modules;
using JollyBit.BS.Server.Networking;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.World.Actors;

namespace JollyBit.BS.Server
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            //Config
			Console.WindowWidth = 100;
			Constants.Kernel = new StandardKernel();
			Constants.Kernel.Load(new BSServerNinjectModule());
            Server server = new Server();
			Constants.Kernel.Bind<ITimeService>().ToConstant(server);
			
            
            //Start
            Constants.Kernel.Get<IStartupService>().ActivateStartupTypes(); //Start all the services
			IConnectionManager connectionManager = Constants.Kernel.Get<IConnectionManager>();
            //Temp code
            //Generate a world
            IMap map = Constants.Kernel.Get<IMap>();
            IChunk c;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    c = map[new Point3L(i * Constants.CHUNK_SIZE_X - 1, 0, j * Constants.CHUNK_SIZE_Z - 1)];
                }
            }
            connectionManager.StartListeningForConnections();
			server.Start();

            //Console Shit
            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
            Constants.Kernel.Dispose();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
		}
	}
}

