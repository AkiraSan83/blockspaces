using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using JollyBit.BS.Server.World.Generation;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Server.Networking;
using JollyBit.BS.Core.Networking.Messages;
using Ninject.Extensions.Logging;

namespace JollyBit.BS.Server.World
{
    public class Map : MapBase, IMap
    {
        private IGenerator _generator;
        private IConnectionManager _connectionManager;

        [Inject]
        public Map(IKernel kernel, IGenerator generator, IConnectionManager connectionManager, ILoggerFactory loggerFactory)
            : base(kernel, loggerFactory)
        {
            _generator = generator;
            _connectionManager = connectionManager;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    IChunk t = this[new Point3L(i * Constants.CHUNK_SIZE_X - 1, 0, j * Constants.CHUNK_SIZE_Z - 1)];
                }
            }
            _connectionManager.ConnectionInitialized += new EventHandler<EventArgs<Core.Networking.IConnection>>(_connectionManager_ConnectionInitialized);
            _logger.Info("Map started.");
        }

        void _connectionManager_ConnectionInitialized(object sender, EventArgs<Core.Networking.IConnection> e)
        {
            //Send the chunks
            foreach (KeyValuePair<Point3L, IChunk> chunk in _chunks)
            {
                e.Data.SendMessage(chunk.Value.CreateMessage());
            }
        }

        public override IChunk this[Point3L blockLocation]
        {
            get
            {
                IChunk ret = base[blockLocation];
                if (ret == null)
                {
                    Point3L chunkLocation = calcChunkLocation(blockLocation);
                    ret = createChunk(chunkLocation);
                    this[chunkLocation] = ret;
                }
                return ret;
            }
            protected set
            {
                base[blockLocation] = value;
            }
        }

        private IChunk createChunk(Point3L chunkLocation)
        {
            IChunk chunk = _kernel.Get<IChunk>();
            for (byte x = 0; x < Constants.CHUNK_SIZE_X; x++)
                for (byte y = 0; y < Constants.CHUNK_SIZE_Y; y++)
                    for (byte z = 0; z < Constants.CHUNK_SIZE_Z; z++)
                    {
                        chunk[x, y, z] = _generator.GenerateBlock(chunkLocation + new Point3L(x, y, z));
                    }
            chunk.Location = chunkLocation;
            return chunk;
        }
    }
}
