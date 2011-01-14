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
        }
    }
}
