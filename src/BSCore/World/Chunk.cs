﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ninject;

using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking.Messages;

namespace JollyBit.BS.Core.World
{
    public class Chunk : IChunk
    {
        private IBlockManager _blockManager;
        public Chunk() {
            _blockManager = Constants.Kernel.Get<IBlockManager>();
        }

        public Point3L Location { get; set; }
        public IMap Map { get; set; }
				
        private ushort[] _blocks = new ushort[Constants.CHUNK_SIZE_X * Constants.CHUNK_SIZE_Y * Constants.CHUNK_SIZE_Z];
        public IBlock this[byte x, byte y, byte z]
        {
            get
            {
                return _blockManager.getBlockFromShort(_blocks[x + y * Constants.CHUNK_SIZE_X + z * Constants.CHUNK_SIZE_X * Constants.CHUNK_SIZE_Y]);
            }
            set
            {
                ushort currentID = _blocks[x + y * Constants.CHUNK_SIZE_X + z * Constants.CHUNK_SIZE_X * Constants.CHUNK_SIZE_Y];
                ushort newID = _blockManager.getShortFromBlock(value);

                if(newID != currentID)
                {
                    _blocks[x + y * Constants.CHUNK_SIZE_X + z * Constants.CHUNK_SIZE_X * Constants.CHUNK_SIZE_Y] = newID;
                    if(BlockChanged != null) {
                        IBlock block = _blockManager.getBlockFromShort(currentID);
                        BlockChanged(this, new BlockChangedEventArgs(Location + new Point3L(x, y, z), block, value));
                    }
                }
            }
        }

        public ChunkMessage CreateMessage() {
            ChunkMessage cm = new ChunkMessage();
            cm.Location = Location;
            cm.Chunk = _blocks;
            return cm;
        }

        public void FillFromMessage(ChunkMessage message)
        {
            this.Location = message.Location;
            _blocks = message.Chunk;
        }

        public event EventHandler<BlockChangedEventArgs> BlockChanged;
    }
}
