using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JollyBit.BS.Core.Utility
{
    /// <summary>
    /// Provides basic file operations to allow files to be accessed transparently
    /// from various sources. 
    /// </summary>
    /// <remarks>
    /// 1. For compatibility file paths are case sensitive.
    /// 2. The name of two files in the same directory are not allowed to differ only in capitalization.
    /// 3. The name of two directories in the same directory are not allowed to differ only in capitalization.
    /// 4. Both '/' and '\' are valid delimiters
    /// </remarks>
    public interface IFileSystem
    {
        /// <summary>
        /// Opens a file at specified path.
        /// </summary>
        /// <param name="path">The path to open. 
        /// The path is relative to the working directory.</param>
        /// <returns>A stream that can read and write from the file. If the file is not found null is returned.</returns>
        Stream OpenFile(FileReference path);
        /// <summary>
        /// Creates any directories in the specified path which do not already exist and the file specified
        /// at the end of the path. If the file already exists it will be replaced. 
        /// </summary>
        Stream CreateFile(FileReference path);
        /// <summary>
        /// Creates any directories in the specified path which do not already exist.
        /// </summary>
        void CreateDirectory(FileReference path);
        /// <summary>
        /// Deletes the specified file if the file exists.
        /// </summary>
        /// <param name="path">The path that specifies the file to delete.</param>
        void DeleteFile(FileReference path);
        /// <summary>
        /// Deletes the specified folder if the folder exists.
        /// </summary>
        /// <param name="path">The path that specifies the directory to delete.</param>
        void DeleteDirectory(FileReference path);
        /// <summary>
        /// Returns a list of all the files in the location specified by the passed in directoryPath.
        /// If the directory does not exist null is returned.
        /// </summary>
        /// <returns>A list of directories</returns>
        IEnumerable<string> ListFilesInDirectory(FileReference path);
        /// <summary>
        /// Returns a list of all the directories in the location specified by the passed in directoryPath.
        /// If the directory does not exist null is returned.
        /// </summary>
        /// <returns>A list of files</returns>
        IEnumerable<string> ListDirectoriesInDirectory(FileReference path);
    }
    public static class FileExtensions
    {
        /// <summary>
        /// Copies a file.
        /// </summary>
        /// <param name="fileSystem">The file system</param>
        /// <param name="oldFilePath">The file that should be copied</param>
        /// <param name="newFilePath">The file that should be created</param>
        /// <param name="overwrite">If this parameter is true and created file will overwrite an existing file if one existing
        /// otherwise an error will be thrown.</param>
        public static void Copy(this IFileSystem fileSystem, FileReference oldFilePath, FileReference newFilePath, bool overwrite)
        {
            using (Stream oldStream = fileSystem.OpenFile(oldFilePath))
            {
                if (oldStream == null) throw new System.IO.FileNotFoundException("File specified by oldFilePath parameter not found");
                if (!overwrite && fileSystem.FileExists(newFilePath)) throw new System.IO.IOException("File specified by newFilePath parameter already exists.");
                using (Stream newStream = fileSystem.CreateFile(newFilePath))
                {
                    byte[] buffer = new byte[256];
                    int numRead;
                    while ((numRead = oldStream.Read(buffer, 0, 256)) != 0)
                    {
                        newStream.Write(buffer, 0, numRead);
                    }
                }
            }
        }
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="fileSystem">The file system</param>
        /// <param name="filePath">The path to the file</param>
        /// <returns>A bool indicating if the file exists</returns>
        public static bool FileExists(this IFileSystem fileSystem, FileReference filePath)
        {
            return fileSystem.ListFilesInDirectory(filePath.GetDirectory())
                .Contains(filePath.GetFileName().FileLocation);
        }        
    }
}
