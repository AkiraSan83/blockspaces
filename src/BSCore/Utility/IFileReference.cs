using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Core.Utility
{
    public class FileReference
    {
        private readonly string _fileLocation;
        public FileReference(string fileLocation)
        {
            _fileLocation = FixPath(fileLocation);
        }
        public override bool Equals(object obj)
        {
            if (obj is FileReference)
            {
                return (obj as FileReference).FileLocation == _fileLocation;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return _fileLocation.GetHashCode();
        }
        public static bool operator ==(FileReference a, FileReference b)
        {
            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }
        public static bool operator !=(FileReference a, FileReference b)
        {
            return !(a == b);
        }
        public static FileReference operator +(FileReference a, FileReference b)
        {
            return new FileReference(a.FileLocation + b.FileLocation);
        }
        public string FileLocation
        {
            get { return _fileLocation; }
        }
        public static implicit operator FileReference(string a)
        {
            return new FileReference(a);
        }
        public static implicit operator string(FileReference a)
        {
            return a.FileLocation;
        }
        public override string ToString()
        {
            return this.FileLocation;
        }
        public FileReference GetDirectory()
        {
            return new FileReference(System.IO.Path.GetDirectoryName(FileLocation));
        }
        public string GetFileExtension()
        {
            return System.IO.Path.GetExtension(FileLocation);
        }
        public FileReference GetFileName()
        {
            return new FileReference(System.IO.Path.GetFileName(FileLocation));
        }
        public static string FixPath(string path)
        {
            return path
                .Replace('/', System.IO.Path.DirectorySeparatorChar)
                .Replace('\\', System.IO.Path.DirectorySeparatorChar);
        }
    }
}
