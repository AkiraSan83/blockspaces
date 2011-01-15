using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ninject;

namespace JollyBit.BS.Utility
{
    public class StandardFileSystem : IFileSystem
    {
        public readonly string _workingDirectory;
        [Inject]
        public StandardFileSystem(string workingDirectory)
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

        public System.IO.Stream OpenFile(string path)
        {
            path = _workingDirectory + fixPath(path);
            if (File.Exists(path))
                return File.Open(path, FileMode.Open);
            else
                return null;
        }

        public Stream CreateFile(string path)
        {
            path = fixPath(path);
            if (File.Exists(_workingDirectory + path))
            {
                return OpenFile(path);
            }
            else
            {
                CreateDirectory(path.Substring(0, path.Length - Path.GetFileName(path).Length));
                return File.Create(path);
            }
        }

        public void CreateDirectory(string path)
        {
            path = _workingDirectory + fixPath(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void DeleteFile(string path)
        {
            path = _workingDirectory + fixPath(path);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public void DeleteDirectory(string path)
        {
            path = _workingDirectory + fixPath(path);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public IEnumerable<string> ListFilesInDirectory(string path)
        {
            path = _workingDirectory + fixPath(path);
            if (Directory.Exists(path))
                return Directory.GetFiles(path);
            else
                return null;
        }

        public IEnumerable<string> ListDirectoriesInDirectory(string path)
        {
            path = _workingDirectory + fixPath(path);
            if (Directory.Exists(path))
                return Directory.GetDirectories(path);
            else
                return null;
        }

        private string fixPath(string path)
        {
            return path
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}
