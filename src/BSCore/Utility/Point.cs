﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using ProtoBuf;

namespace JollyBit.BS.Core.Utility
{
    [ProtoContract]
    public struct Point3L
    {
        [ProtoMember(1, IsRequired = true)]
        public long X;
        [ProtoMember(2, IsRequired = true)]
        public long Y;
        [ProtoMember(3, IsRequired = true)]
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
        public static Point3L operator *(Point3L c1, Point3L c2)
        {
            return new Point3L(c1.X * c2.X, c1.Y * c2.Y, c1.Z * c2.Z);
        }
        public static Point3L operator /(Point3L c1, Point3L c2)
        {
            return new Point3L(c1.X / c2.X, c1.Y / c2.Y, c1.Z / c2.Z);
        }
        public static explicit operator Vector3(Point3L c)  // explicit byte to digit conversion operator
        {
            return new Vector3(c.X, c.Y, c.Z);
        }
        public override string ToString()
        {
            return (String.Format("({0},{1},{2})", this.X, this.Y, this.Z));
        }

        private const UInt32 FNV_PRIME = 16777619;
        private const UInt32 OFFSET_BASIS = 2166136261;
        /// <summary>
        /// Hahes a xyz coordinate using FNV hash. Look at
        /// http://isthe.com/chongo/tech/comp/fnv/ for more info on FNV hash.
        /// </summary>
        public override int GetHashCode()
        {
            //hash = offset_basis
            //for each octet_of_data to be hashed
            //    hash = hash xor octet_of_data
            //    hash = hash * FNV_prime
            //return hash
            return (int)((((((OFFSET_BASIS ^ (UInt32)X) * FNV_PRIME) ^ (UInt32)Y) * FNV_PRIME) ^ (UInt32)Z) * FNV_PRIME);
        }
    }
}
