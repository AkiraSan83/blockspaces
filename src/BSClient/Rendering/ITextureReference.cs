using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace JollyBit.BS.Rendering
{
    public interface ITextureReference : IRenderable
    {
        RectangleF TextureLocation { get; }
    }
}
