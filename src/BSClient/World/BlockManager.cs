using System;
using System.Collections.Generic;
using Ninject;

//using JollyBit.BS.Client.World;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Core.Networking.Messages;
using Ninject.Extensions.Logging;

namespace JollyBit.BS.Client.World
{
    public class BlockManager : IBlockManager
    {
        private IDictionary<ushort,IBlock> _blocks = new Dictionary<ushort, IBlock>();
        private IDictionary<IBlock,ushort> _shorts = new Dictionary<IBlock, ushort>();
        private ushort counter = 1;
        private ILogger _logger;
        [Inject]
        public BlockManager(IConnection connection, ILoggerFactory logFactory) {
            _logger = logFactory.GetLogger(typeof(BlockManager));
            connection.MessageReceived += new EventHandler<EventArgs<object>>(connection_MessageReceived);
            _logger.Info("BlockManager started.");
        }

        void connection_MessageReceived(object sender, EventArgs<object> e)
        {
            if (e.Data.GetType() == typeof(BlockMessage))
            {
                BlockMessage m = (BlockMessage)(e.Data);
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

