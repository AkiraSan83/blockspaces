using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNoise;

namespace JollyBit.BS.World.Generation
{
    public class SimpleTerrainGenerator : IGenerator
    {
        private LibNoise.Perlin _perlinNoise = new Perlin();
        private IBlock _block = new Block();
        private int maxHeight = 30;
        public SimpleTerrainGenerator()
        {
            _perlinNoise.Seed = (new Random()).Next();
            _perlinNoise.Frequency = .05;
            _perlinNoise.NoiseQuality = NoiseQuality.Standard;
            _perlinNoise.OctaveCount = 6;
            _perlinNoise.Lacunarity = 2;
            _perlinNoise.Persistence = .3;
        }
        public IBlock GenerateBlock(Utility.Point3L location)
        {
            double value = ((_perlinNoise.GetValue(location.X, location.Z, 10) + 1.0) / 2.0);
            if (((double)(maxHeight - location.Y)) / (double)(maxHeight) > value)
            {
                return _block;
            }
            else
            {
                return null;
            }
        }
    }
}
