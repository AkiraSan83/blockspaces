using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using JollyBit.BS.World.Generation;
using JollyBit.BS.Utility;

namespace JollyBit.BS.World
{
    public class Map : IMap
    {
        private IDictionary<Point3L, IChunk> _chunks = new Dictionary<Point3L, IChunk>();
        private IKernel _kernel;
        private IGenerator _generator;

        [Inject]
        public Map(IKernel kernel, IGenerator generator)
        {
            _kernel = kernel;
            _generator = generator;
        }

        public IChunk this[Point3L blockLocation]
        {
            get
            {
                IChunk chunk;
                Point3L chunkLocation = calcChunkLocation(blockLocation);                
                if (!_chunks.TryGetValue(chunkLocation, out chunk))
                {
                    chunk = createChunk(chunkLocation);
                    _chunks.Add(chunkLocation, chunk);
                    if (ChunkChanged != null) ChunkChanged(this, new ItemChangedEventArgs<IChunk>(null, chunk));
                }
                return chunk;
            }
        }

        private Point3L calcChunkLocation(Point3L blockLocation) {
            return new Point3L(
                blockLocation.X / BSCoreConstants.CHUNK_SIZE_X * BSCoreConstants.CHUNK_SIZE_X - (blockLocation.X < 0 ? BSCoreConstants.CHUNK_SIZE_X : (byte)0),
                blockLocation.Y / BSCoreConstants.CHUNK_SIZE_Y * BSCoreConstants.CHUNK_SIZE_Y - (blockLocation.Y < 0 ? BSCoreConstants.CHUNK_SIZE_Y : (byte)0),
                blockLocation.Z / BSCoreConstants.CHUNK_SIZE_Z * BSCoreConstants.CHUNK_SIZE_Z - (blockLocation.Z < 0 ? BSCoreConstants.CHUNK_SIZE_Z : (byte)0)
            );
        }

        private IChunk createChunk(Point3L chunkLocation)
        {
            IChunk chunk = _kernel.Get<IChunk>();
            for (byte x = 0; x < BSCoreConstants.CHUNK_SIZE_X; x++)
                for (byte y = 0; y < BSCoreConstants.CHUNK_SIZE_Y; y++)
                    for (byte z = 0; z < BSCoreConstants.CHUNK_SIZE_Z; z++)
                    {
                        chunk[x, y, z] = _generator.GenerateBlock(chunkLocation + new Point3L(x, y, z));
                    }
            chunk.Location = chunkLocation;
            return chunk;
        }

        public IEnumerable<IChunk> Chunks
        {
            get { return _chunks.Values; }
        }

        public event EventHandler<Utility.ItemChangedEventArgs<IChunk>> ChunkChanged;
    }
}
