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
using System.Text;

namespace VP.FF.PT.Common.Infrastructure.Credentials
{
    public class WindowsCredentialToolBox: IWindowsCredentialToolBox
    {
        #region DllImport
        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool CredRead(string target, WindowsCredentialTypes.CredentialType type, int reservedFlag, out IntPtr credentialPtr);

        [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool CredWrite(ref WindowsCredentialTypes.NativeCredential userCredential, UInt32 flags);

        [DllImport("Advapi32.dll", EntryPoint = "CredDeleteW", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool CredDelete(string targetName, WindowsCredentialTypes.CredentialType type, UInt32 flags);

        [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
        static extern bool CredFree(IntPtr cred);
        #endregion DllImport

        /// <summary>
        /// This method derives a NativeCredential instance from a given Credential instance.
        /// </summary>
        /// <param name="cred">The managed Credential counterpart containing data to be stored.</param>
        /// <returns>A NativeCredential instance that is derived from the given Credential
        /// instance.</returns>
        private static WindowsCredentialTypes.NativeCredential GetNativeCredential(WindowsCredentialTypes.Credential cred)
        {
            WindowsCredentialTypes.NativeCredential ncred = new WindowsCredentialTypes.NativeCredential();
            ncred.AttributeCount = 0;
            ncred.Attributes = IntPtr.Zero;
            ncred.Comment = IntPtr.Zero;
            ncred.TargetAlias = IntPtr.Zero;
            ncred.Type = (WindowsCredentialTypes.CredentialType)cred.Type;
            ncred.Persist = (UInt32)cred.Persist;
            ncred.CredentialBlobSize = (UInt32)cred.CredentialBlobSize;
            ncred.TargetName = Marshal.StringToCoTaskMemUni(cred.TargetName);
            ncred.CredentialBlob = Marshal.StringToCoTaskMemUni(cred.CredentialBlob);
            ncred.UserName = Marshal.StringToCoTaskMemUni(cred.UserName);
            return ncred;
        }

        private static object MarshalNativeToManaged(IntPtr nativeDataPtr)
        {
            if (nativeDataPtr == IntPtr.Zero)
            {
                return null;
            }

            WindowsCredentialTypes.NativeCredential rawCredential =
                (WindowsCredentialTypes.NativeCredential)Marshal.PtrToStructure(nativeDataPtr, typeof(WindowsCredentialTypes.NativeCredential));

            WindowsCredentialTypes.Credential cred = new WindowsCredentialTypes.Credential()
            {
                UserName = Marshal.PtrToStringUni(rawCredential.UserName),
                TargetName = Marshal.PtrToStringUni(rawCredential.TargetName),
                TargetAlias = Marshal.PtrToStringUni(rawCredential.TargetAlias),
                Persist = (WindowsCredentialTypes.CredentialPersist)rawCredential.Persist,
                Comment = Marshal.PtrToStringUni(rawCredential.Comment),
                Flags = rawCredential.Flags,
                LastWritten = rawCredential.LastWritten,
                Type = rawCredential.Type,
                CredentialBlob = Marshal.PtrToStringUni(rawCredential.CredentialBlob),
                CredentialBlobSize = rawCredential.CredentialBlobSize,
                Attributes = new WindowsCredentialTypes.CredentialAttribute[rawCredential.AttributeCount],
                AttributeCount = rawCredential.AttributeCount
            };

            // ToDo: Fill the Attributes array with values. It is not important because the information is not needed and the complexity to marshal this data is quite high.

            CredFree(nativeDataPtr);

            return cred;
        }

        public bool Add(string targetName, string userName, string password, WindowsCredentialTypes.CredentialType type, WindowsCredentialTypes.CredentialPersist persist)
        {
            // Check if the password is not too long.
            byte[] byteArray = Encoding.Unicode.GetBytes(password);

            if (byteArray.Length > 512)
                throw new ArgumentOutOfRangeException("The password has exceeded 512 bytes.");

            WindowsCredentialTypes.Credential cred = new WindowsCredentialTypes.Credential()
            {
                UserName = userName,
                TargetName = targetName,
                TargetAlias = null,
                Persist = persist,
                Comment = null,
                Flags = 0,
                Type = type,
                CredentialBlob = password,
                CredentialBlobSize = (UInt32)Encoding.Unicode.GetBytes(password).Length,
                Attributes = null,
                AttributeCount = 0
            };

            WindowsCredentialTypes.NativeCredential ncred = GetNativeCredential(cred);

            // Write the info into the CredMan storage.
            bool written = CredWrite(ref ncred, 0);
            int lastError = Marshal.GetLastWin32Error();

            Marshal.FreeCoTaskMem(ncred.TargetName);
            Marshal.FreeCoTaskMem(ncred.CredentialBlob);
            Marshal.FreeCoTaskMem(ncred.UserName);

            if (written)
            {
                return true;
            }
            else
            {
                string message = string.Format("CredWrite failed with the error code {0}.", lastError);
                throw new Exception(message);
            }
        }

        public bool Exist(string targetName, WindowsCredentialTypes.CredentialType type)
        {
            return Exist(targetName, null, null, type);
        }

        /// <summary>
        /// Check if the entry exists. Ignore for now the passoword.
        /// </summary>
        public bool Exist(string targetName, string userName, string password, WindowsCredentialTypes.CredentialType type)
        {
            IntPtr credPtr;

            if (CredRead(targetName, type, 0, out credPtr))
            {
                WindowsCredentialTypes.Credential cred = (WindowsCredentialTypes.Credential)MarshalNativeToManaged(credPtr);

                return cred.TargetName.Equals(targetName, StringComparison.CurrentCultureIgnoreCase) &&
                        (cred.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase) ||
                        userName.IsNullOrEmpty());
            }
            else
            {
                return false;
            }
        }

        public bool Delete(string targetName, WindowsCredentialTypes.CredentialType type)
        {
            return CredDelete(targetName, type, 0);
        }
    }
}
