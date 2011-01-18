using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Utility;
using System.Drawing;
using System.IO;

namespace JollyBit.BS.Rendering
{
    public interface ITextureManager
    {
        /// <summary>
        /// Adds a cube texture and returns a reference to the created texture.
        /// If the texture already exists a new texture is not created and a reference to the
        /// existing texture is returned.
        /// </summary>
        /// <param name="cubeTexture">A reference to the file from wich to create the texture from.</param>
        /// <returns>A reference to the created texture.</returns>
        ITextureReference AddCubeTexture(IFileReference cubeTexture);
        void RenderCubeTexture();
    }
    public class TextureManager : ITextureManager
    {
        private readonly GLState _glState;
        private readonly IFileSystem _fileSystem;
        #region Cube Texture Atlas
        private readonly TextureAtlas _cubeTextureAtlas;
        private GLTextureObject _cubeTextureAtlasTextureObject;
        private readonly IDictionary<IFileReference, ITextureReference> _cubeTextureCache = new Dictionary<IFileReference, ITextureReference>();
        [Ninject.Inject]
        public TextureManager(GLState glState, IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
            _glState = glState;
            _cubeTextureAtlas = new TextureAtlas(1024, 64, 4);
        }
        public ITextureReference AddCubeTexture(IFileReference cubeTexture)
        {
            //Check if in cache
            ITextureReference textRef;
            if (_cubeTextureCache.TryGetValue(cubeTexture, out textRef))
            {
                return textRef;
            }
            //Not in cache create
            if (_cubeTextureAtlasTextureObject != null)
            {
                _cubeTextureAtlasTextureObject.Dispose();
                _cubeTextureAtlasTextureObject = null;
            }
            using (Stream stream = _fileSystem.OpenFile(cubeTexture.FileLocation))
            using (Bitmap bitmap = new Bitmap(stream))
            {
                RectangleF location = _cubeTextureAtlas.AddSubImage(bitmap);
                textRef = new TextureReference(location, new Action(RenderCubeTexture));
            }
            _cubeTextureCache.Add(cubeTexture, textRef);
            return textRef;
        }
        public void RenderCubeTexture()
        {
            if (_cubeTextureAtlasTextureObject == null)
            {
                _cubeTextureAtlasTextureObject = new GLTextureObject(_cubeTextureAtlas.Texture, 4);
            }
            _cubeTextureAtlasTextureObject.Render();
        }
        #endregion
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
