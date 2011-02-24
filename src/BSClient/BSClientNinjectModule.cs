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
using JollyBit.BS.Core.World.Actors;
using JollyBit.BS.Client.World.Actors;

namespace JollyBit.BS.Client
{
    public class BSClientNinjectModule : NinjectModule
    {
        public override void Load()
        {
            //Register services - Order is important services bound first get to register for events first and consequently receive events first.


            //Create Bindings
            Kernel.Load(new JollyBit.BS.Core.BSCoreNinjectModule());
            Rebind<GLState>().To<GLState>().InSingletonScope();
            Rebind<IBlockManager>().To<JollyBit.BS.Client.World.BlockManager>().InSingletonScope();
            Rebind<IMap>().To<JollyBit.BS.Client.World.Map>().InSingletonScope();
            Rebind<ITextureAtlasFactory>().To<TextureAtlasFactory>().InSingletonScope();
            Rebind<ContentManager>().To<ContentManager>().InSingletonScope();
            Rebind<IClientConnection>().To<Connection>().InSingletonScope();
            Rebind<IConnection>().ToMethod(context => Kernel.Get<IClientConnection>());
            Rebind<IMessageTypeManager>().To<MessageTypeManager>().InSingletonScope();

            //Create Component Bindings
            Rebind<IPositionableComponent>().To<PositionComponent>().InActorScope();
            Rebind<IClientSyncComponent>().ToProvider<ClientSyncComponentProvider>().InActorScope();

            //Associate each ActorMessage with a component type that will handle the message
            Bind<MessageComponentAssociation>().ToConstant(MessageComponentAssociation.Create<PositionMessage, IPositionableComponent>());
        }
    }
}
