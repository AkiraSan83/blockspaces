using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core;
using JollyBit.BS.Core.Networking.Messages;
namespace JollyBit.BS.Core.World
{
    public abstract class MapBase
    {
        protected IDictionary<Point3L, IChunk> _chunks = new Dictionary<Point3L, IChunk>();
        protected IKernel _kernel;

        public MapBase(IKernel kernel)
        {
            _kernel = kernel;
        }

        
        public virtual IChunk this[Point3L blockLocation]
        {
            get
            {
                IChunk chunk;
                Point3L chunkLocation = calcChunkLocation(blockLocation);
                if (!_chunks.TryGetValue(chunkLocation, out chunk)) return chunk;
                return null;
            }
            protected set
            {
                IChunk chunk;
                Point3L chunkLocation = calcChunkLocation(blockLocation);
                if (_chunks.TryGetValue(chunkLocation, out chunk))
                {
                    if (chunk == value) return;
                    _chunks[chunkLocation] = value;
                }
                else _chunks.Add(chunkLocation, value);
                if (ChunkChanged != null) ChunkChanged(this, new ItemChangedEventArgs<IChunk>(chunk, value));
            }
        }

        protected Point3L calcChunkLocation(Point3L blockLocation)
        {
            return new Point3L(
                blockLocation.X / Constants.CHUNK_SIZE_X * Constants.CHUNK_SIZE_X - (blockLocation.X < 0 ? Constants.CHUNK_SIZE_X : (byte)0),
                blockLocation.Y / Constants.CHUNK_SIZE_Y * Constants.CHUNK_SIZE_Y - (blockLocation.Y < 0 ? Constants.CHUNK_SIZE_Y : (byte)0),
                blockLocation.Z / Constants.CHUNK_SIZE_Z * Constants.CHUNK_SIZE_Z - (blockLocation.Z < 0 ? Constants.CHUNK_SIZE_Z : (byte)0)
            );
        }

        public IEnumerable<IChunk> Chunks
        {
            get { return _chunks.Values; }
        }

        public event EventHandler<ItemChangedEventArgs<IChunk>> ChunkChanged;

    }
}
