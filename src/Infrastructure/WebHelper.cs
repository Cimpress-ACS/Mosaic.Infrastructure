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


using System;
using System.IO;
using System.Net;

namespace VP.FF.PT.Common.Infrastructure
{
    public static class WebHelper
    {
        /// <summary>
        /// Downloads an image from a web-page.
        /// </summary>
        /// <param name="uri">Web address.</param>
        /// <param name="fileName">Path and filename for the image to save.</param>
        /// <returns>True if download and save was successful, otherwise false.</returns>
        public static bool DownloadRemoteImageFile(string uri, string fileName)
        {
            var request = (HttpWebRequest) WebRequest.Create(uri);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse) request.GetResponse();
            }
            catch (Exception)
            {
                return false;
            }

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode == HttpStatusCode.OK ||
                 response.StatusCode == HttpStatusCode.Moved ||
                 response.StatusCode == HttpStatusCode.Redirect) &&
                response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {

                // if the remote file was found, download it
                using (Stream inputStream = response.GetResponseStream())
                using (Stream outputStream = File.OpenWrite(fileName))
                {
                    var buffer = new byte[4096];
                    int bytesRead;
                    do
                    {
                        bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                        outputStream.Write(buffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Returns a good place to store temporary application data.
        /// </summary>
        /// <returns></returns>
        public static string GetLocalCacheFolder(string subfolder = "")
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            if (!string.IsNullOrEmpty(subfolder))
                path += "\\" + subfolder;

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += "\\";

            return path;
        }
    }
}
