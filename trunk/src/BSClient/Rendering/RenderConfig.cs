using System;
using JollyBit.BS.Core.Utility;
using JollyBit.BS.Client.Rendering;
using JollyBit.BS.Core;
using JsonExSerializer;

using Ninject;
using Ninject.Extensions.Logging;

namespace JollyBit.BS.Client.Rendering
{
	public class RenderConfig : IConfigSection
	{
		private GLState _glState;
		private ILogger _logger;
		
		public float FarClippingPlane = 128;
        public int FieldOfView = 120;
		
		public RenderConfig()
		{ 
			_glState = Constants.Kernel.Get<GLState>();
            _logger = Constants.Kernel.Get<ILoggerFactory>().GetLogger(typeof(RenderConfig)); 
		}
		
		public int MaxTextureSizePower = 11;
		
		[JsonExIgnore]
		public int MaxTextureSize {
			get {
				int glSizePower;
				glSizePower = (int)(Math.Log(_glState.MaxSize) / Math.Log(2));
				if( glSizePower < MaxTextureSizePower ) {
					_logger.Warn("Texture size too big. Decreased from 2^{0} to 2^{1}.",MaxTextureSizePower,glSizePower);
					MaxTextureSizePower = glSizePower;
				}
				return (int)Math.Pow(2.0, (double)MaxTextureSizePower);
			}
			set {
				MaxTextureSizePower = (int)(Math.Log(value) / Math.Log(2));
			}
		}
	}
}

