/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

namespace VP.FF.PT.Common.Infrastructure.FileAccess
{
    /// <summary>
    /// The <see cref="FileSystemAccessor"/> is able to access the file system.
    /// </summary>
    [Export(typeof(IDeleteFile))]
    [Export(typeof(ILoadFromFileSystem))]
    [Export(typeof(ISaveToFileSystem))]
    [Export(typeof(IAccessFileSystem))]
    [Export(typeof(ICopyFile))]
    [Export(typeof(IPathExists))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FileSystemAccessor : IAccessFileSystem
    {
        public bool DeleteFileIfExists(string file)
        {
            if (!File.Exists(file))
            {
                return false;
            }
            File.Delete(file);
            return true;
        }

        /// <summary>
        /// Reads the content of the specified <paramref name="filePath"/>
        /// and returns it as string.
        /// </summary>
        /// <param name="filePath">The path where the file can be found.</param>
        /// <returns>A string with the content of the file.</returns>
        public string LoadFileContent(string filePath)
        {
            bool fileExists = File.Exists(filePath);
            if (!fileExists)
                return string.Empty;
            return File.ReadAllText(filePath);
        }

        /// <summary>
        /// Overwrites the content of the file with the specified <paramref name="filePath"/>
        /// with the specified <paramref name="newContent"/>.
        /// </summary>
        /// <param name="filePath">The path where the file can be found.</param>
        /// <param name="newContent">The new content to save in the file.</param>
        public void OverwriteContentInFile(string filePath, string newContent)
        {
            bool fileExists = File.Exists(filePath);
            if (fileExists)
                File.Delete(filePath);
            using (FileStream fileStream = File.Create(filePath))
            using (var writer = new StreamWriter(fileStream))
                writer.Write(newContent);
        }

        public async Task CopyAsync(string source, string destination)
        {
            using (var sourceStream = File.Open(source, FileMode.Open))
            {
                var destinationFile = new FileInfo(destination);
                if (destinationFile.Exists)
                {
                    await destinationFile.DeleteAsync();
                }

                using (var destinationStream = destinationFile.Create())
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
            }
        }

        public bool PathExists(string path)
        {
            return DirectoryExists(path) || FileExists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
