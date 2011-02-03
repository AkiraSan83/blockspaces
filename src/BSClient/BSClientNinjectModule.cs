using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Ninject;
using JollyBit.BS.Core.Utility;
using Ninject.Parameters;
using System.IO;
using System.Windows.Forms;
using JollyBit.BS.Client.Rendering;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Client.Networking;
using JollyBit.BS.Core.World;
using JollyBit.BS.Client.World;

namespace JollyBit.BS.Client
{
    public class BSClientNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Load(new JollyBit.BS.Core.BSCoreNinjectModule());
            Bind<GLState>().To<GLState>().InSingletonScope();
            Bind<IBlockManager>().To<JollyBit.BS.Client.World.BlockManager>().InSingletonScope();
            Bind<ITextureAtlasFactory>().To<TextureAtlasFactory>().InSingletonScope();
            Bind<ContentManager>().To<ContentManager>().InSingletonScope();
            Bind<IClientConnection<object>>().To<Connection<object>>().InSingletonScope();
            Bind<IConnection<object>>().ToMethod(context => Kernel.Get<IClientConnection<object>>());
            Bind<IConfigManager>().ToMethod(
                (context) =>
                {
                    //XmlSerializer
                    Stream stream = context.Kernel.Get<IFileSystem>().OpenFile("ClientConfig.json");
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
        }
    }
}
