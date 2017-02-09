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


namespace VP.FF.PT.Common.Infrastructure.FileAccess
{
    /// <summary>
    /// An implementer of <see cref="ISaveToFileSystem"/> is capable of
    /// saving information to the local file system.
    /// </summary>
    public interface ISaveToFileSystem
    {
        /// <summary>
        /// Overwrites the content of the file with the specified <paramref name="filePath"/>
        /// with the specified <paramref name="newContent"/>.
        /// </summary>
        /// <param name="filePath">The path where the file can be found.</param>
        /// <param name="newContent">The new content to save in the file.</param>
        void OverwriteContentInFile(string filePath, string newContent);
    }
}
