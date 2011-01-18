using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JollyBit.BS.Utility
{
    public interface IFileReference
    {
        string FileLocation { get; }
    }
    public class FileReference : IFileReference
    {
        private readonly string _fileLocation;
        public FileReference(string fileLocation)
        {
            _fileLocation = fileLocation;
        }
        public override bool Equals(object obj)
        {
            if (obj is IFileReference)
            {
                return (obj as IFileReference).FileLocation == _fileLocation;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return _fileLocation.GetHashCode();
        }
        public static bool operator ==(FileReference a, IFileReference b)
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
        public static bool operator !=(FileReference a, IFileReference b)
        {
            return !(a == b);
        }
        public string FileLocation
        {
            get { return _fileLocation; }
        }
    }
}
