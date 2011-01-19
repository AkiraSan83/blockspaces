using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Rendering;
using JollyBit.BS.Utility;
using System.Drawing;

namespace JollyBit.BS.Rendering
{
    public class SkyBox : IRenderable
    {
        private Vbo<VertexPositionColorTexture> _vbo;
        //private TextureAtlas _atlas = new TextureAtlas(1024, 256, 2);
        public SkyBox(IFileReference fileSystem)
        {
            
        }
        public void Render()
        {
            throw new NotImplementedException();
        }
    }
}
