using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.World;
using JollyBit.BS.Utility;

namespace JollyBit.BS.World.Generation
{
    public interface IGenerator
    {
        IBlock GenerateBlock(Point3L location);
    }
}
