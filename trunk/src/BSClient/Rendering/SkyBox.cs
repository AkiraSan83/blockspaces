using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Rendering;
using JollyBit.BS.Utility;
using System.Drawing;
using Ninject;
using Vertex = JollyBit.BS.Rendering.VertexPositionColorTexture;
using OpenTK.Graphics.OpenGL;

using JollyBit.BS.Client.Rendering;
using JollyBit.BS.Utility;

namespace JollyBit.BS.Rendering
{
    public class SkyBox : IRenderable
    {
        private Vbo<Vertex> _vbo;
        private ITextureAtlas _atlas;
        private readonly ITextureReference _neg_x;
        private readonly ITextureReference _pos_x;
        private readonly ITextureReference _neg_y;
        private readonly ITextureReference _pos_y;
        private readonly ITextureReference _pos_z;
        private readonly ITextureReference _neg_z;
		private RenderConfig _config;
        public SkyBox(IBitmap neg_x, IBitmap pos_x, IBitmap neg_y, IBitmap pos_y, IBitmap neg_z, IBitmap pos_z)
        {
			_config = BSCoreConstants.Kernel.Get<IConfigManager>().GetConfig<RenderConfig>();
			
            ITextureAtlasFactory atlasFactory = BSCoreConstants.Kernel.Get<ITextureAtlasFactory>();
            _atlas = atlasFactory.CreateTextureAtlas(_config.MaxTextureSize, _config.MaxTextureSize / 4, 1);
            _neg_x = _atlas.AddSubImage(neg_x);
            _pos_x = _atlas.AddSubImage(pos_x);
            _pos_y = _atlas.AddSubImage(pos_y);
            _neg_y = _atlas.AddSubImage(neg_y);
            _pos_z = _atlas.AddSubImage(pos_z);
            _neg_z = _atlas.AddSubImage(neg_z);
        }

        private void rebuild()
        {
            if (_vbo != null)
            {
                _vbo.Dispose();
                _vbo = null;
            }
            float v = BSCoreConstants.Kernel.Get<GLState>().FarClippingPlane / 2;
            Vertex[] vertexes = new Vertex[]
            {
                //_pos_z
                new Vertex(-v, -v, v, Color.White, _pos_z.TextureLocation.X, _pos_z.TextureLocation.Y + _pos_z.TextureLocation.Height),
                new Vertex(-v, v, v, Color.White, _pos_z.TextureLocation.X, _pos_z.TextureLocation.Y),
                new Vertex(v, v, v, Color.White, _pos_z.TextureLocation.X + _pos_z.TextureLocation.Width, _pos_z.TextureLocation.Y),
                new Vertex(v, -v, v, Color.White, _pos_z.TextureLocation.X + _pos_z.TextureLocation.Width, _pos_z.TextureLocation.Y + _pos_z.TextureLocation.Height),
                //_neg_z
                new Vertex(-v, -v, -v, Color.White, _neg_z.TextureLocation.X + _neg_z.TextureLocation.Width, _neg_z.TextureLocation.Y + _neg_z.TextureLocation.Height),
                new Vertex(-v, v, -v, Color.White, _neg_z.TextureLocation.X + _neg_z.TextureLocation.Width, _neg_z.TextureLocation.Y),
                new Vertex(v, v, -v, Color.White, _neg_z.TextureLocation.X, _neg_z.TextureLocation.Y),
                new Vertex(v, -v, -v, Color.White, _neg_z.TextureLocation.X, _neg_z.TextureLocation.Y + _neg_z.TextureLocation.Height),
                //_neg_x
                new Vertex(-v, -v, -v, Color.White, _neg_x.TextureLocation.X, _neg_x.TextureLocation.Y + _neg_x.TextureLocation.Height),
                new Vertex(-v, v, -v, Color.White, _neg_x.TextureLocation.X, _neg_x.TextureLocation.Y),
                new Vertex(-v, v, v, Color.White, _neg_x.TextureLocation.X + _neg_x.TextureLocation.Width, _neg_x.TextureLocation.Y),
                new Vertex(-v, -v, v, Color.White, _neg_x.TextureLocation.X + _neg_x.TextureLocation.Width, _neg_x.TextureLocation.Y + _neg_x.TextureLocation.Height),
                //_pos_x        
                new Vertex(v, -v, -v, Color.White, _pos_x.TextureLocation.X + _pos_x.TextureLocation.Width, _pos_x.TextureLocation.Y + _pos_x.TextureLocation.Height),
                new Vertex(v, v, -v, Color.White, _pos_x.TextureLocation.X + _pos_x.TextureLocation.Width, _pos_x.TextureLocation.Y),
                new Vertex(v, v, v, Color.White, _pos_x.TextureLocation.X, _pos_x.TextureLocation.Y),
                new Vertex(v, -v, v, Color.White, _pos_x.TextureLocation.X, _pos_x.TextureLocation.Y + _pos_x.TextureLocation.Height),
                //_pos_y
                new Vertex(-v, v, v, Color.White, _pos_y.TextureLocation.X + _pos_y.TextureLocation.Width, _pos_y.TextureLocation.Y + _pos_y.TextureLocation.Height),
                new Vertex(-v, v, -v, Color.White, _pos_y.TextureLocation.X, _pos_y.TextureLocation.Y + _pos_y.TextureLocation.Height),
                new Vertex(v, v, -v, Color.White, _pos_y.TextureLocation.X, _pos_y.TextureLocation.Y),
                new Vertex(v, v, v, Color.White, _pos_y.TextureLocation.X + _pos_y.TextureLocation.Width, _pos_y.TextureLocation.Y),
                //_neg_y
                new Vertex(-v, -v, v, Color.White, _neg_y.TextureLocation.X + _neg_y.TextureLocation.Width, _neg_y.TextureLocation.Y + _neg_y.TextureLocation.Height),
                new Vertex(-v, -v, -v, Color.White, _neg_y.TextureLocation.X, _neg_y.TextureLocation.Y + _neg_y.TextureLocation.Height),
                new Vertex(v, -v, -v, Color.White, _neg_y.TextureLocation.X, _neg_y.TextureLocation.Y),
                new Vertex(v, -v, v, Color.White, _neg_y.TextureLocation.X + _neg_y.TextureLocation.Width, _neg_y.TextureLocation.Y)
            };
            short[] indices = new short[]
            {
                0,1,2,3,
                7,6,5,4,
                8,9,10,11,
                15,14,13,12,
                16,17,18,19,
                23,22,21,20
            };
            _vbo = new Vbo<Vertex>(vertexes, indices);
            _vbo.PrimitiveMode = OpenTK.Graphics.OpenGL.BeginMode.Quads;
        }
        /*
                 _________________________ (1,1,0)
                / _________Top_________  /|
               / / ___________________/ / |   
              / / /| |               / /  |  /|\
             / / / | |              / / . |   |
            / / /| | |             / / /| |   |
           / / / | | |            / / / | |   |
          / / /  | | |           / / /| | |   | +Y axis
         / /_/__________________/ / / | | |   |
        /________________________/ /  | | |   |
Left--> | ______________________ | |  | | |   | 
        | | |    | | |_________| | |__| | |   |
        | | |    | |___________| | |____| |   |
        | | |   / / ___________| | |_  / /    
        | | |  / / /           | | |/ / /     /
        | | | / / /            | | | / /     /
        | | |/ / /             | | |/ /     /
        | | | / /              | | ' /     /  +Z axis
        | | |/_/_______________| |  /     /
        | |____________________| | /     /
        |________Front___________|/    \/
        (0,0,1)
          ---------------------->
        +X axis
*/

        public void Render()
        {
            if (_vbo == null)
            {
                rebuild();
            }
            GL.DepthMask(false);
            _atlas.Render();
            _vbo.Render();
            GL.DepthMask(true);
        }
    }
}
