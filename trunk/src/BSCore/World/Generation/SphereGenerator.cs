using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.World.Generation
{
    public class SphereGenerator : IGenerator
    {
        private long _radiusSquared;
        public long Radius
        {
            get{ return (long)Math.Sqrt(_radiusSquared); }
            set { _radiusSquared = value * value; }
        }
        public IBlock GenerateBlock(Utility.Point3L location)
        {
            if (location.X * location.X + location.Y * location.Y + location.Z * location.Z > _radiusSquared)
            {
                return new Block();
            }
            else
            {
                return null;
            }
        }
    }
}
