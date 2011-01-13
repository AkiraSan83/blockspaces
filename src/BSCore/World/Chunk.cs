using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.World
{
    public class Chunk : IChunk
    {
        public IBlock this[byte x, byte y, byte z]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public event EventHandler<BlockChangedEventArgs> BlockChanged;
    }
}
