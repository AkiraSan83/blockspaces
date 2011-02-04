using System;
using System.Collections.Generic;
using Ninject;

using JollyBit.BS.Core;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.Networking;
using JollyBit.BS.Server.Networking;
using JollyBit.BS.Core.Networking.Messages;
using Ninject.Extensions.Logging;

namespace JollyBit.BS.Server.World
{
    public class BlockManager : IBlockManager
    {
        private IDictionary<ushort,IBlock> _blocks = new Dictionary<ushort, IBlock>();
        private IDictionary<IBlock,ushort> _shorts = new Dictionary<IBlock, ushort>();
        private ushort counter = 1;
        private ILogger _logger;

        [Inject]
        public BlockManager(IConnectionManager connectionManager, ILoggerFactory loggerFactory) {
            connectionManager.ConnectionEstablished += new EventHandler<EventArgs<IConnection>>(connectionManager_ConnectionEstablished);
            _logger = loggerFactory.GetLogger(typeof(BlockManager));
            _logger.Info("BlockManager started");
        }

        void connectionManager_ConnectionEstablished(object sender, EventArgs<IConnection> e)
        {
            BlockMessage message = new BlockMessage();
            _logger.Debug("BlockManager is sending blocks to client");
            foreach (var x in _blocks)
            {
                message.ID = x.Key;

                message.Front = x.Value.GetTextureForSide(BlockSides.Front).FileLocation;
                message.Back = x.Value.GetTextureForSide(BlockSides.Back).FileLocation;
                message.Left = x.Value.GetTextureForSide(BlockSides.Left).FileLocation;
                message.Right = x.Value.GetTextureForSide(BlockSides.Right).FileLocation;
                message.Top = x.Value.GetTextureForSide(BlockSides.Top).FileLocation;
                message.Bottom = x.Value.GetTextureForSide(BlockSides.Bottom).FileLocation;

                e.Data.SendMessage(message);
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

