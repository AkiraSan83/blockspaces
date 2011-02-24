using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Ninject;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.Utility;
using System.IO;
using JollyBit.BS.Core.Networking;
using Lidgren.Network;
using System.Windows.Forms;
using JollyBit.BS.Core.Networking.Messages;
using JollyBit.BS.Core.World.Actors;

namespace JollyBit.BS.Core
{
    public class BSCoreNinjectModule : NinjectModule
    {


        public override void Load()
        {
            //Create bindings
            Rebind<IStartupService>().To<StartupService>().InSingletonScope();
            Rebind<IChunk>().To<Chunk>();
            Rebind<INetworkPeer>().To<NetworkPeer>().InSingletonScope();
            string path = Application.ExecutablePath.Substring(0, Application.ExecutablePath.Length - Path.GetFileName(Application.ExecutablePath).Length);
            Rebind<IFileSystem>().To<StandardFileSystem>().InSingletonScope().WithConstructorArgument("workingDirectory", new FileReference(path + "assets/"));
            Rebind<NetPeerConfiguration>().ToMethod(context => Kernel.Get<NetworkConfig>().CreateNetPeerConfig()).InSingletonScope();
            Rebind<IConfigManager>().To<ConfigManager>().InSingletonScope().WithConstructorArgument("fileReference", new FileReference(Application.ExecutablePath).GetFileName() + "_config.json");
            Bind<IActor>().ToMethod(context => context.Parameters.First(parm => parm.Name == "actor") as IActor);
            Bind<IActor>().ToProvider<ActorProvider>();
            
            //Logging config stuff
            {
                NLog.Config.LoggingConfiguration config = new NLog.Config.LoggingConfiguration();
                //targets
                NLog.Targets.FileTarget fileTarget = new NLog.Targets.FileTarget();
                config.AddTarget("file", fileTarget);
                NLog.Targets.ConsoleTarget consoleTarget = new NLog.Targets.ConsoleTarget();
                config.AddTarget("console", fileTarget);

                fileTarget.FileName = "${basedir}/${processname}_Log.txt";
                fileTarget.Layout = "[${longdate}] [${level}] [${message}]";
                consoleTarget.Layout = ">> [${date:format=HH\\:MM\\:ss}] [${level}] [${message}]";
                //rules
                NLog.Config.LoggingRule loggingRule;
                loggingRule = new NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, fileTarget);
                config.LoggingRules.Add(loggingRule);
                loggingRule = new NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, consoleTarget);
                config.LoggingRules.Add(loggingRule);
                //activate
                NLog.LogManager.Configuration = config;
            }

            //Register services - Order is important services bound first get to register for events first and consequently receive events first.
            IStartupService startup = Kernel.Get<IStartupService>();
            startup.RegisterStartupType<IConfigManager>();
            startup.RegisterStartupType<IMessageTypeManager>();
            startup.RegisterStartupType<IBlockManager>();
            startup.RegisterStartupType<IMap>();
        }
    }
}
