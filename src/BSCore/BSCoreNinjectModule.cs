using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Ninject;
using JollyBit.BS.World;
using JollyBit.BS.World.Generation;
using JollyBit.BS.Utility;
using System.IO;

namespace JollyBit.BS
{
    public class BSCoreNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IKernel>().ToConstant(this.Kernel);
            Bind<IMap>().To<Map>();
            Bind<IGenerator>().To<SimpleTerrainGenerator>();
            Bind<IChunk>().To<Chunk>();
            Bind<IConfigManager>().ToMethod(
                (context) =>
                {
                    //XmlSerializer
                    Stream stream = context.Kernel.Get<IFileSystem>().OpenFile("config.json");
                    ConfigManager configManager;
                    if (stream != null)
                    {
                        TextReader reader = new StreamReader(stream);
                        JsonExSerializer.Serializer serializer = new JsonExSerializer.Serializer(typeof(ConfigManager));
                        configManager = serializer.Deserialize(stream) as ConfigManager;
                        stream.Close();
                    }
                    else configManager = new ConfigManager();
                    return configManager;
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
