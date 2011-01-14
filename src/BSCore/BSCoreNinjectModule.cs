using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Ninject;
using JollyBit.BS.World;
using JollyBit.BS.World.Generation;

namespace JollyBit.BS
{
    public class BSCoreNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IKernel>().ToConstant(this.Kernel);
            Bind<IMap>().To<Map>();
            Bind<IGenerator>().To<SphereGenerator>();
            Bind<IChunk>().To<Chunk>();

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
