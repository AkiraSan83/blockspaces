using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Utility
{
    public struct Point3L
    {
        public long X;
        public long Y;
        public long Z;
        public Point3L(long x, long y, long z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Point3L operator +(Point3L c1, Point3L c2)
        {
            return new Point3L(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z);
        }
        public static Point3L operator -(Point3L c1, Point3L c2)
        {
            return new Point3L(c1.X - c2.X, c1.Y - c2.Y, c1.Z - c2.Z);
        }
        public override string ToString()
        {
            return (String.Format("({0},{1},{2})", this.X, this.Y, this.Z));
        }
    }
}
