using System;
using System.Collections.Generic;
using Ninject;

//using JollyBit.BS.Client.World;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Core.Networking.Messages;

namespace JollyBit.BS.Client.World
{
    public class BlockManager : IBlockManager
    {
        private IDictionary<ushort,IBlock> _blocks = new Dictionary<ushort, IBlock>();
        private IDictionary<IBlock,ushort> _shorts = new Dictionary<IBlock, ushort>();
        private ushort counter = 1;

        [Inject]
        public BlockManager(IConnection connection) {
            connection.MessageReceived += handleMessage;
        }

        private void handleMessage(object sender, EventArgs<object> eargs) {
            if(eargs.Data.GetType() == typeof(BlockMessage)) {
                BlockMessage m = (BlockMessage)(eargs.Data);
                JollyBit.BS.Client.World.Block block = new JollyBit.BS.Client.World.Block(m.Left, m.Right, m.Front, m.Back, m.Top, m.Bottom);
                _blocks[m.ID] = block;
                _shorts[block] = m.ID;
            }
        }

        public IBlock getBlockFromShort(ushort id) {
            if(id == 0)
                return null;
            return _blocks[id];
        }

        public ushort getShortFromBlock(IBlock block) {
            ushort s;

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
    }}

