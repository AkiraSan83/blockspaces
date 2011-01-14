using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject.Modules;
using Ninject;
using JollyBit.BS.Utility;
using Ninject.Parameters;

namespace JollyBit.BS
{
    public class BSClientNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFileSystem>().To<StandardFileSystem>()
                .InSingletonScope()
                .WithParameter(new ConstructorArgument("workingDirectory", this.GetType().Assembly.CodeBase + "resources"));
        }
    }
}
