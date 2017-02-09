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
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace VP.FF.PT.Common.Infrastructure.Credentials
{
    public class WindowsCredentialTypes
    {
        #region Flags
        /// <summary>
        /// Credential type. Defines the purpose of the safed credential.
        /// See also https://msdn.microsoft.com/en-us/library/windows/desktop/aa374788(v=vs.85).aspx
        /// </summary>
        public enum CredentialType : uint
        {
            GENERIC = 1,
            DOMAIN_PASSWORD = 2,
            DOMAIN_CERTIFICATE = 3,
            DOMAIN_VISIBLE_PASSWORD = 4,
            GENERIC_CERTIFICATE = 5,
            DOMAIN_EXTENDED = 6,
            MAXIMUM = 7,      // Maximum supported cred type
            MAXIMUM_EX = (MAXIMUM + 1000),  // Allow new applications to run on old OSes
        }

        /// <summary>
        /// Credential Persistency Definition
        /// See also https://msdn.microsoft.com/en-us/library/windows/desktop/aa374788(v=vs.85).aspx
        /// </summary>
        public enum CredentialPersist : uint
        {
            SESSION = 1,
            LOCAL_MACHINE = 2,
            ENTERPRISE = 3,
        }
        #endregion Flags

        #region Structures
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct CredentialAttribute
        {
            string Keyword;
            uint Flags;
            uint ValueSize;
            IntPtr Value;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct NativeCredential
        {
            public UInt32 Flags;
            public CredentialType Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public IntPtr CredentialBlob;
            public UInt32 Persist;
            public UInt32 AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct Credential
        {
            public UInt32 Flags;
            public CredentialType Type;
            public string TargetName;
            public string Comment;
            public FILETIME LastWritten;
            public UInt32 CredentialBlobSize;
            public string CredentialBlob;
            public CredentialPersist Persist;
            public UInt32 AttributeCount;
            public CredentialAttribute[] Attributes;
            public string TargetAlias;
            public string UserName;
        }
        #endregion Structures

    }
}
