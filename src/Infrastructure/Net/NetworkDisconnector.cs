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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace VP.FF.PT.Common.Infrastructure.Net
{
    internal class NetworkDisconnector
    {
        #region dll ipmorts
        [DllImport("Mpr.dll", EntryPoint = "WNetOpenEnumA", CallingConvention = CallingConvention.Winapi)]
        private static extern int WNetOpenEnum(ResourceScope dwScope, ResourceType dwType, ResourceUsage dwUsage, NetResource p, out IntPtr lphEnum);

        [DllImport("Mpr.dll", EntryPoint = "WNetCloseEnum", CallingConvention = CallingConvention.Winapi)]
        private static extern int WNetCloseEnum(IntPtr hEnum);

        [DllImport("Mpr.dll", EntryPoint = "WNetEnumResourceA", CallingConvention = CallingConvention.Winapi)]
        private static extern int WNetEnumResource(IntPtr hEnum, ref uint lpcCount, IntPtr buffer, ref uint lpBufferSize);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, int flags, bool force);
        #endregion

        public void Disconnect(string networkName)
        {
            WNetCancelConnection2(networkName, 0, true);
        }

        public void DisconnectAll()
        {
            var data = new List<NetResource>();
            EnumerateServers(ResourceScope.Connected, ResourceType.Disk, ResourceUsage.Default, data);

            foreach (var share in data.Where(d => d.LocalName == null).Select(d => d.RemoteName))
            {
                WNetCancelConnection2(share, 0, true);
            }
        }

        // code of this function is based on http://www.codeproject.com/Articles/6235/Enumerating-Network-Resources
        private static void EnumerateServers(ResourceScope scope, ResourceType type, ResourceUsage usage, IList<NetResource> aData)
        {
            uint bufferSize = 16384;
            IntPtr buffer = Marshal.AllocHGlobal((int)bufferSize);
            IntPtr handle;
            uint cEntries = 1;
            var pRsrc = new NetResource();

            int result = WNetOpenEnum(scope, type, usage, pRsrc, out handle);

            if (result == 0)
            {
                while (result == 0)
                {
                    result = WNetEnumResource(handle, ref cEntries, buffer, ref	bufferSize);

                    if (result == 0)
                    {
                        var netResource = new NetResource();
                        Marshal.PtrToStructure(buffer, netResource);

                        aData.Add(netResource);

                        if ((pRsrc.Usage & (int)ResourceUsage.Container) == (int)ResourceUsage.Container)
                        {
                            EnumerateServers(scope, type, usage, aData);
                        }
                    }
                }

                WNetCloseEnum(handle);
            }

            Marshal.FreeHGlobal(buffer);
        }

    }
}
