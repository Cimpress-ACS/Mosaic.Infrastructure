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


namespace VP.FF.PT.Common.Infrastructure.Credentials
{
    public interface IWindowsCredentialToolBox
    {
        /// <summary>
        /// Add a new user to the credential manager
        /// </summary>
        /// <param name="targetName">Targetpath or domain</param>
        /// <param name="userName">login user name</param>
        /// <param name="password">login password</param>
        /// <returns></returns>
        bool Add(string targetName, string userName, string password, WindowsCredentialTypes.CredentialType type, WindowsCredentialTypes.CredentialPersist persist);

        /// <summary>
        /// Checks if a credential with the given target name exist in the credential manager under the logged in user.
        /// </summary>
        /// <param name="targetName">Targetpath or domain</param>
        /// <returns></returns>
        bool Exist(string targetName, WindowsCredentialTypes.CredentialType type);

        /// <summary>
        /// Checks if a credential with the given target name exist in the credential manager under the logged in user. 
        /// Additionally it checks if the existing credential has the correct user and password
        /// </summary>
        /// <param name="targetName">Targetpath or domain</param>
        /// <param name="userName">login user name</param>
        /// <param name="password">login password</param>
        /// <returns></returns>
        bool Exist(string targetName, string userName, string password, WindowsCredentialTypes.CredentialType type);

        /// <summary>
        /// Delete a given credential from the credential manager with the matching target path
        /// </summary>
        /// <param name="targetName">Targetpath or domain</param>
        /// <returns></returns>
        bool Delete(string targetName, WindowsCredentialTypes.CredentialType type);
    }
}
