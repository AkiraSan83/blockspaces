using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking.Messages;

namespace JollyBit.BS.Core.World
{
    public class BlockChangedEventArgs : EventArgs
    {
        public readonly Point3L BlockLocation;
        public readonly IBlock OldValue;
        public readonly IBlock NewValue;
        public BlockChangedEventArgs(Point3L blockLocation, IBlock oldValue, IBlock newValue)
        {
            BlockLocation = blockLocation;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
    
    public interface IChunk
    {
        IBlock this[byte x, byte y, byte z] { get; set; }
        event EventHandler<BlockChangedEventArgs> BlockChanged;
        Point3L Location { get; set; }
        IMap Map { get; set; }
        ChunkMessage CreateMessage();
        void FillFromMessage(ChunkMessage message);
    }

    public interface IMap
    {
        IChunk this[Point3L blockLocation] { get; }
        IEnumerable<IChunk> Chunks { get; }
        event EventHandler<ChangedEventArgs<IChunk>> ChunkChanged;
    }

    public static class MapExtensions
    {
        public static IBlock GetBlock(this IMap self, Point3L blockLocation)
        {
            throw new System.NotImplementedException("Does not currently work for negative numbers");
            return self[blockLocation][(byte)(blockLocation.X % (long)Constants.CHUNK_SIZE_X),
                (byte)(blockLocation.Y % (long)Constants.CHUNK_SIZE_Y),
                (byte)(blockLocation.Z % (long)Constants.CHUNK_SIZE_Z)];
        }
    }
}
