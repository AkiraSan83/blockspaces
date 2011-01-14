using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace JollyBit.BS.Utility
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
        Stream OpenFile(string path);
        /// <summary>
        /// Creates any directories in the specified path wich do not already exist and the file specified
        /// at the end of the path. If the file already exists it will be opened. 
        /// </summary>
        Stream CreateFile(string path);
        /// <summary>
        /// Creates any directories in the specified path wich do not already exist.
        /// </summary>
        void CreateDirectory(string path);
        /// <summary>
        /// Deletes the specified file if the file exists.
        /// </summary>
        /// <param name="path">The path that specifies the file to delete.</param>
        void DeleteFile(string path);
        /// <summary>
        /// Deletes the specified folder if the folder exists.
        /// </summary>
        /// <param name="path">The path that specifies the directory to delete.</param>
        void DeleteDirectory(string path);
        /// <summary>
        /// Returns a list of all the files in the location specified by the passed in directoryPath.
        /// If the directory does not exist null is returned.
        /// </summary>
        /// <returns>A list of directories</returns>
        IEnumerable<string> ListFilesInDirectory(string path);
        /// <summary>
        /// Returns a list of all the directories in the location specified by the passed in directoryPath.
        /// If the directory does not exist null is returned.
        /// </summary>
        /// <returns>A list of files</returns>
        IEnumerable<string> ListDirectoriesInDirectory(string path);
    }
}
