using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using JollyBit.BS.Utility;
using System.IO;
using Ninject;

namespace JollyBit.BS.Rendering
{
    public interface IUniquelyIdentifiable
    {
        string UniqueId { get; }
    }
    public interface IBitmap : IUniquelyIdentifiable
    {
        Bitmap Bitmap { get; }
    }
    public class ContentManager
    {
        private readonly IDictionary<string, IUniquelyIdentifiable> _cache = new Dictionary<string, IUniquelyIdentifiable>();
        private readonly IFileSystem _fileSystem;
        public ContentManager(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IBitmap LoadBitmap(IFileReference fileReference)
        {
            IBitmap bitmap;
            if (tryGetFromCache<IBitmap>(fileReference.FileLocation, out bitmap))
            {
                return bitmap;
            }
            bitmap = new BitmapFile(fileReference.FileLocation);
            _cache.Add(fileReference.FileLocation, bitmap);
            return bitmap;
        }

        private bool tryGetFromCache<T>(string uniqueId, out T item) where T : IUniquelyIdentifiable
        {
            IUniquelyIdentifiable temp;
            if (_cache.TryGetValue(uniqueId, out temp))
            {
                item = (T)temp;
                return true;
            }
            item = default(T);
            return false;
        }

        private class BitmapFile : IBitmap
        {
            private readonly string _filePath;
            private Bitmap _bitmap = null;
            public BitmapFile(string filePath)
            {
                _filePath = filePath;
            }
            public Bitmap Bitmap
            {
                get
                {
                    if (_bitmap == null)
                    {
                        using (Stream stream = BSCoreConstants.Kernel.Get<IFileSystem>().OpenFile(_filePath))
                        {
                            _bitmap = new Bitmap(stream); 
                        }
                        return _bitmap;
                    }
                    return _bitmap;
                }
            }

            public string UniqueId
            {
                get { return _filePath; }
            }
        }
    }
}
