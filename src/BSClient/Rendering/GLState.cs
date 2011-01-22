using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace JollyBit.BS.Client.Rendering
{
    public class GLState
    {
        public GLState()
        {
            ClearColor = Color.Black;
            GL.Enable(EnableCap.DepthTest);
            GL.CullFace(CullFaceMode.Back);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.CullFace);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }

        public readonly float FarClippingPlane = 128;
		
		private int _maxSize = -1;
		public int MaxSize {
			get { 
				if(_maxSize == -1)
					GL.GetInteger(GetPName.MaxTextureSize, out _maxSize);
				return _maxSize;
			}
		}
		
        private Color _clearColor = Color.DarkSlateGray;
        public Color ClearColor
        {
            get { return _clearColor; }
            set
            {
                if (_clearColor != value)
                {
                    _clearColor = value;
                    GL.ClearColor(_clearColor);
                }
            }
        }

        private int _boundTexture = 0;
        public int BoundTexture
        {
            get { return _boundTexture; }
            set
            {
                if (_boundTexture != value)
                {
                    _boundTexture = value;
                    GL.BindTexture(TextureTarget.Texture2D, _boundTexture);
                }
            }
        }
    }
}
