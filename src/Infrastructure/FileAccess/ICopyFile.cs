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


using System.Threading.Tasks;

namespace VP.FF.PT.Common.Infrastructure.FileAccess
{
    /// <summary>
    /// Thin wrapper around File I/O operations, mainly used to make I/O unit testable
    /// </summary>
    public interface ICopyFile
    {
        /// <summary>
        /// Copies a file from the source location to the target location.
        /// </summary>
        /// <param name="source">Full path to the file to be copied.</param>
        /// <param name="destination">Full path to where the file should be copied to.</param>
        Task CopyAsync(string source, string destination);
    }
}
