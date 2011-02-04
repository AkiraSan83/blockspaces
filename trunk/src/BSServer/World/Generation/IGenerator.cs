using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Server.World.Generation
{
    public interface IGenerator
    {
        IBlock GenerateBlock(Point3L location);
    }
}
