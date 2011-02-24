using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Modules;
using JollyBit.BS.Server.Networking;
using JollyBit.BS.Core.Utility;
using JsonExSerializer;
using System.IO;
using JollyBit.BS.Core.World;
using JollyBit.BS.Server.World;
using JollyBit.BS.Server.World.Generation;
using JollyBit.BS.Core.Networking.Messages;
using JollyBit.BS.Server.Networking.Messages;
using JollyBit.BS.Core.World.Actors;
using JollyBit.BS.Server.World.Actors;
using JollyBit.BS.Server.Utility;

namespace JollyBit.BS.Server
{
    public class BSServerNinjectModule : NinjectModule
    {
        public override void Load()
        {
            //Create Bindings
            Kernel.Load(new JollyBit.BS.Core.BSCoreNinjectModule());
            Rebind<IGenerator>().To<SimpleTerrainGenerator>();
            Rebind<IMap>().To<Map>().InSingletonScope();
            Rebind<IConnectionManager>().To<ConnectionManager>().InSingletonScope();
            Rebind<IBlockManager>().To<JollyBit.BS.Server.World.BlockManager>().InSingletonScope();
            Rebind<IMessageTypeManager>().To<MessageTypeManager>().InSingletonScope();

            //Create Component Bindings
            Rebind<IPositionableComponent>().To<PositionComponent>().InActorScope();
            Rebind<IServerSyncComponent>().To<SimpleSyncComponent>().InActorScope();

            //Register services - Order is important services bound first get to register for events first and consequently receive events first.
            IStartupService startup = Kernel.Get<IStartupService>();
            startup.RegisterStartupType<TestService>();
        }
    }
}
