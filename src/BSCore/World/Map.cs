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
        private IDictionary<UInt64, IChunk> _chunks = new Dictionary<UInt64, IChunk>();
        private IKernel _kernel;
        private IGenerator _generator;

        [Inject]
        public Map(IKernel kernel, IGenerator generator)
        {
            _kernel = kernel;
            _generator = generator;
        }

        public IChunk this[long x, long y, long z]
        {
            get
            {
                IChunk chunk;
                ulong hashedValue = hash((ulong)x, (ulong)y, (ulong)z);
                if (!_chunks.TryGetValue(hashedValue, out chunk))
                {
                    chunk = createChunk(calcChunkLocation(new Point3L(x, y, z)));
                    _chunks.Add(hashedValue, chunk);
                }
                return chunk;
            }
        }

        private Point3L calcChunkLocation(Point3L blockLocation)
        {
            long x = blockLocation.X;
            long y = blockLocation.Y;
            long z = blockLocation.Z;
            return new Point3L(x - x % BSCoreConstants.CHUNK_SIZE_X, y - y % BSCoreConstants.CHUNK_SIZE_Y, z - z % BSCoreConstants.CHUNK_SIZE_Z);
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
            return chunk;
        }

        public Utility.Point3L GetChunkLocation(IChunk chunk)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IChunk> Chunks
        {
            get { throw new NotImplementedException(); }
        }

        public event EventHandler<Utility.ItemChangedEventArgs<IChunk>> ChunkChanged;

        private const UInt64 FNV_PRIME = 1099511628211;
        private const UInt64 OFFSET_BASIS = 14695981039346656037;
        /// <summary>
        /// Hahes a xyz coordinate using FNV hash. Look at
        /// http://isthe.com/chongo/tech/comp/fnv/ for more info on FNV hash.
        /// </summary>
        protected UInt64 hash(UInt64 x, UInt64 y, UInt64 z)
        {
            //hash = offset_basis
            //for each octet_of_data to be hashed
            //    hash = hash xor octet_of_data
            //    hash = hash * FNV_prime
            //return hash
            return (((((OFFSET_BASIS ^ x) * FNV_PRIME) ^ y) * FNV_PRIME) ^ z) * FNV_PRIME;
        }
    }
}
