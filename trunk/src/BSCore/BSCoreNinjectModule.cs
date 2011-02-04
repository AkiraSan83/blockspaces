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

namespace JollyBit.BS.Core
{
    public class BSCoreNinjectModule : NinjectModule
    {
        public override void Load()
        {
            //Register services
            Bind<IService>().ToMethod(context => Kernel.Get<IBlockManager>());
            Bind<IService>().ToMethod(context => Kernel.Get<IMap>());

            //Create bindings
            Rebind<IKernel>().ToConstant(this.Kernel);
            Rebind<IChunk>().To<Chunk>();
            Rebind<INetworkPeer>().To<NetworkPeer>().InSingletonScope();
            Rebind<IMessageTypeManager>().To<MessageTypeManager>().InSingletonScope();
            string path = Application.ExecutablePath.Substring(0, Application.ExecutablePath.Length - Path.GetFileName(Application.ExecutablePath).Length);
            Rebind<IFileSystem>().To<StandardFileSystem>().InSingletonScope()
                .WithConstructorArgument("workingDirectory", path + "assets/");
            Rebind<NetPeerConfiguration>().ToMethod(
                (context) =>
                {
                    return Kernel.Get<IConfigManager>().GetConfig<NetworkConfig>().CreateNetPeerConfig();
                }).InSingletonScope();

            //Logging config stuff
            {
                NLog.Config.LoggingConfiguration config = new NLog.Config.LoggingConfiguration();
                //targets
                NLog.Targets.FileTarget fileTarget = new NLog.Targets.FileTarget();
                config.AddTarget("file", fileTarget);
                NLog.Targets.ConsoleTarget consoleTarget = new NLog.Targets.ConsoleTarget();
                config.AddTarget("console", fileTarget);

                fileTarget.FileName = "${basedir}/log.txt";
                fileTarget.Layout = "${longdate}|${level}|${message}";
                consoleTarget.Layout = "${date:format=HH\\:MM\\:ss} ${level} '${message}'";
                //rules
                NLog.Config.LoggingRule loggingRule;
                loggingRule = new NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, fileTarget);
                config.LoggingRules.Add(loggingRule);
                loggingRule = new NLog.Config.LoggingRule("*", NLog.LogLevel.Debug, consoleTarget);
                config.LoggingRules.Add(loggingRule);
                //activate
                NLog.LogManager.Configuration = config;
            }
        }
    }
}
