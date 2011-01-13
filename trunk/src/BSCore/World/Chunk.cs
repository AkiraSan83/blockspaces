﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JollyBit.BS.Utility;

namespace JollyBit.BS.World
{
    public class Chunk : IChunk
    {
		public Point3L Location {
			get;
			set;
		}
		public IMap Map {
			get;
			set;
		}
				
        private IBlock[, ,] _blocks = new IBlock[BSCoreConstants.CHUNK_SIZE_X, BSCoreConstants.CHUNK_SIZE_Y, BSCoreConstants.CHUNK_SIZE_Z];
        public IBlock this[byte x, byte y, byte z]
        {
            get
            {
                return _blocks[x, y, z];
            }
            set
            {
                IBlock block = _blocks[x, y, z];
                if (block != value)
                {
                    IBlock oldBlock = block;
                    _blocks[x, y, z] = value;
                    //if (BlockChanged != null) BlockChanged(this, new BlockChangedEventArgs(
                }
            }
        }

        public event EventHandler<BlockChangedEventArgs> BlockChanged;
    }
}