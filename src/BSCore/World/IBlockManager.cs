using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Core.World
{

    public class BlockManager : IBlockManager
    {
        private IDictionary<short,IBlock> _blocks = new Dictionary<short, IBlock>();
        private IDictionary<IBlock,short> _shorts = new Dictionary<IBlock, short>();
        private short counter = 1;

        public IBlock getBlockFromShort(short id) {
            if(id == 0)
                return null;
            return _blocks[id];
        }

        public short getShortFromBlock(IBlock block) {
            short s;

            if(block == null)
                return 0;

            if(!_shorts.TryGetValue(block, out s)) {
                // Have to add the block to the dictionaries
                s = counter;
                counter++;

                _blocks.Add(s,block);
                _shorts.Add(block,s);
            }

            return s;
        }
    }
}

