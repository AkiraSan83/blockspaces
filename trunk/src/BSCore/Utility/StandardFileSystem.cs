using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ninject;

namespace JollyBit.BS.Core.Utility
{
    public class StandardFileSystem : IFileSystem
    {
        public readonly string _workingDirectory;
        [Inject]
        public StandardFileSystem(FileReference workingDirectory)
        {
            _workingDirectory = workingDirectory;
            if (_workingDirectory[_workingDirectory.Length - 1] != '/' ||
                _workingDirectory[_workingDirectory.Length - 1] != '\\')
            {
                _workingDirectory += Path.DirectorySeparatorChar;
            }
            _workingDirectory = fixPath(workingDirectory);
            if (!Directory.Exists(_workingDirectory))
            {
                throw new System.InvalidProgramException("Working directory does not exist.");
            }
        }

        public System.IO.Stream OpenFile(FileReference path)
        {
            path = _workingDirectory + fixPath(path);
            if (File.Exists(path))
                return File.Open(path, FileMode.Open);
            else
                return null;
        }

        public Stream CreateFile(FileReference path)
        {
            path = fixPath(path);
            if (File.Exists(_workingDirectory + path))
            {
                DeleteFile(path);
            }
            CreateDirectory(path.FileLocation.Substring(0, path.FileLocation.Length - Path.GetFileName(path).Length));
            return File.Create(_workingDirectory + path);
        }

        public void CreateDirectory(FileReference path)
        {
            path = _workingDirectory + fixPath(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void DeleteFile(FileReference path)
        {
            path = _workingDirectory + fixPath(path);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteDirectory(FileReference path)
        {
            path = _workingDirectory + fixPath(path);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public IEnumerable<string> ListFilesInDirectory(FileReference path)
        {
            path = _workingDirectory + fixPath(path);
            if (Directory.Exists(path))
                return Directory.GetFiles(path);
            else
                return null;
        }

        public IEnumerable<string> ListDirectoriesInDirectory(FileReference path)
        {
            path = _workingDirectory + fixPath(path);
            if (Directory.Exists(path))
                return Directory.GetDirectories(path);
            else
                return null;
        }

        private string fixPath(string path)
        {
            return FileReference.FixPath(path);
        }
    }
}
