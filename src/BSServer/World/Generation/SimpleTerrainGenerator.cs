using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNoise;
using JollyBit.BS.Core;
using JollyBit.BS.Core.World;
using JollyBit.BS.Core.Utility;

namespace JollyBit.BS.Server.World.Generation
{
    public class SimpleTerrainGenerator : IGenerator
    {
        private LibNoise.Perlin _perlinNoise = new Perlin();
        private IBlock _block = new Block();
		private IBlock _stoneBlock = new AnnoyingStoneBlock();
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
		
//		IDictionary<Point3L,double> _dict = new Dictionary<Point3L,double>();
//		private double _getNoise(Point3L location) {
//			//double val;
//			//if(!_dict.TryGetValue(location, out val)) {
//				val = ((_perlinNoise.GetValue(location.X, location.Z, 10) + 1.0) / 2.0);
//				_dict.Add(location,val);
//			}
//			return val;
//		}
		
        public IBlock GenerateBlock(Point3L location)
        {
            double value = ((_perlinNoise.GetValue(location.X, location.Z, 10) + 1.0) / 2.0);
            if (((double)(maxHeight - location.Y)) / (double)(maxHeight) > value)
            {
				if(value < .25)
					return _stoneBlock;
				else
                	return _block;
            }
            else
            {
                return null;
            }
        }
    }
}
