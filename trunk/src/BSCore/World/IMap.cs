using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Utility;
using JollyBit.BS.World.Generation;

namespace JollyBit.BS.World
{
    public class BlockChangedEventArgs : EventArgs
    {
        public readonly Point3L BlockLocation;
        public readonly IBlock OldValue;
        public readonly IBlock NewValue;
    }

    public interface IChunk
    {
        IBlock this[byte x, byte y, byte z] { get; set; }
        event EventHandler<BlockChangedEventArgs> BlockChanged;
    }

    public interface IMap
    {
        IChunk this[long x, long y, long z] { get; }
        Point3L GetChunkLocation(IChunk chunk);
        IEnumerable<IChunk> Chunks { get; }
        event EventHandler<ItemChangedEventArgs<IChunk>> ChunkChanged;
    }

    public static class MapExtensions
    {
        public static IBlock GetBlock(this IMap self, Point3L point)
        {
            return self.GetBlock(point.X, point.Y, point.Z);
        }
        public static IBlock GetBlock(this IMap self, long x, long y, long z)
        {
            return self[x, y, z][(byte)(x % (long)BSCoreConstants.CHUNK_SIZE_X),
                (byte)(y % (long)BSCoreConstants.CHUNK_SIZE_Y),
                (byte)(z % (long)BSCoreConstants.CHUNK_SIZE_Z)];
        }
    }
}
