using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Server.World.Generation
{
    public class SphereGenerator : IGenerator
    {
        private long _radiusSquared = Constants.CHUNK_SIZE_X * Constants.CHUNK_SIZE_X / 2;
        public long Radius
        {
            get { return (long)Math.Sqrt(_radiusSquared); }
            set { _radiusSquared = value * value; }
        }
        private IBlock _block = new Block();
        public IBlock GenerateBlock(Point3L location)
        {
            if (location.X * location.X + location.Y * location.Y + location.Z * location.Z < _radiusSquared)
            {
                return _block;
            }
            else
            {
                return null;
            }
        }
    }
}
