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
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure.Net
{
    public static class NetworkConnectionFactoryExtensions
    {
        public static IDisposable TryConnect(this INetworkConnectionFactory factory, ILogger logger, string directory, string username, string password, string domain = "")
        {
            // ignore if the username, password or directory is not set
            if (string.IsNullOrEmpty(directory) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                logger.WarnFormat("Username ('{0}'), password ('{1}') or domain ('{2}') empty; not connecting to network share.", username,
                    string.IsNullOrEmpty(password) ? "<empty>" : "<not empty>", domain);

                return null;
            }
            
            // ensure we can connect to the remote network connection
            if (directory.StartsWith(@"\\"))
            {
                try
                {
                    logger.DebugFormat("Connecting network share to '{0}' with user '{1}\\{2}'.", directory, domain, username);
                    return factory.Connect(directory, username, password, domain);
                }
                catch (Exception ex)
                {
                    // sigh, let's just log it and hope the connection works, for example because it was already connected
                    var msg = string.Format("Error when connecting to '{0}' with user '{1}\\{2}'.", directory, domain, username);
                    logger.Error(msg, ex);
                }
            }

            return null;
        }
    }
}
