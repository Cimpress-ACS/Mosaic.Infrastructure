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
using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;

namespace VP.FF.PT.Common.Infrastructure.Net
{
    internal class NetworkConnection : IDisposable
    {
        #region DLL Imports
        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NetResource netResource, string password, string username, int flags);
        #endregion

        private readonly string _networkName;

        public NetworkConnection(string networkName, NetworkCredential credentials)
        {
            _networkName = networkName;

            var netResource = new NetResource
            {
                Scope = ResourceScope.GlobalNetwork,
                ResourceType = ResourceType.Disk,
                DisplayType = ResourceDisplaytype.Share,
                RemoteName = _networkName
            };

            var userName = string.IsNullOrEmpty(credentials.Domain) ? credentials.UserName : string.Format(@"{0}\{1}", credentials.Domain, credentials.UserName);

            int result = WNetAddConnection2(netResource, credentials.Password, userName, 0);

            if (result != 0)
            {
                var e = new Win32Exception(result);
                throw e;
            }
        }

        ~NetworkConnection()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            new NetworkDisconnector().Disconnect(_networkName);
        }
    }
}
