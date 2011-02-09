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
using JollyBit.BS.Core.Networking.Messages;
using JollyBit.BS.Client.Networking.Messages;

namespace JollyBit.BS.Client
{
    public class BSClientNinjectModule : NinjectModule
    {
        public override void Load()
        {
            //Register services - Order is important services bound first get to register for events first and consequently receive events first.


            //Create bindings
            Kernel.Load(new JollyBit.BS.Core.BSCoreNinjectModule());
            Rebind<GLState>().To<GLState>().InSingletonScope();
            Rebind<IBlockManager>().To<JollyBit.BS.Client.World.BlockManager>().InSingletonScope();
            Rebind<IMap>().To<JollyBit.BS.Client.World.Map>().InSingletonScope();
            Rebind<ITextureAtlasFactory>().To<TextureAtlasFactory>().InSingletonScope();
            Rebind<ContentManager>().To<ContentManager>().InSingletonScope();
            Rebind<IClientConnection>().To<Connection>().InSingletonScope();
            Rebind<IConnection>().ToMethod(context => Kernel.Get<IClientConnection>());
            Rebind<IMessageTypeManager>().To<MessageTypeManager>().InSingletonScope();
            Rebind<IConfigManager>().ToMethod(
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
