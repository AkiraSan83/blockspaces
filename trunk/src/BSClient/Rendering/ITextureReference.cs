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

    public class TextureReference : ITextureReference
    {
        public readonly RectangleF _textureLocation;
        private readonly Action _renderFunction = null;
        public TextureReference(RectangleF textureLocation, Action renderFunction)
        {
            _textureLocation = textureLocation;
            _renderFunction = renderFunction;
        }

        public void Render()
        {
            _renderFunction();
        }

        public RectangleF TextureLocation
        {
            get { return _textureLocation; }
        }
    }
}
