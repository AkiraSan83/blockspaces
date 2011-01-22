using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Core.World
{
    public class Chunk : IChunk
    {
        public Point3L Location { get; set; }
        public IMap Map { get; set; }
				
        private IBlock[, ,] _blocks = new IBlock[Constants.CHUNK_SIZE_X, Constants.CHUNK_SIZE_Y, Constants.CHUNK_SIZE_Z];
        public IBlock this[byte x, byte y, byte z]
        {
            get
            {
                return _blocks[x, y, z];
            }
            set
            {
                IBlock block = this[x, y, z];
                if (block != value)
                {
                    _blocks[x, y, z] = value;
                    if (BlockChanged != null)
                        BlockChanged(this, new BlockChangedEventArgs(Location + new Point3L(x, y, z), block, value));
                }
            }
        }

        public event EventHandler<BlockChangedEventArgs> BlockChanged;
    }
}
