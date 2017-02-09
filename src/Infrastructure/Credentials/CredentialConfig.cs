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


using System.Configuration;

namespace VP.FF.PT.Common.Infrastructure.Credentials
{
    public class CredentialConfig : ConfigurationElement
    {
        [ConfigurationProperty("target", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string Target
        {
            get { return (string)base["target"]; }
            set { base["target"] = value; }
        }

        [ConfigurationProperty("user", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string User
        {
            get { return (string)base["user"]; }
            set { base["user"] = value; }
        }

        [ConfigurationProperty("password", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }

        /// <summary>
        /// See CredentialType. Values are: GENERIC, DOMAIN_PASSWORD, DOMAIN_CERTIFICATE, DOMAIN_VISIBLE_PASSWORD, GENERIC_CERTIFICATE, DOMAIN_EXTENDED
        /// </summary>
        [ConfigurationProperty("type", DefaultValue = "DOMAIN_PASSWORD", IsKey = false, IsRequired = false)]
        public WindowsCredentialTypes.CredentialType Type
        {
            get{ return (WindowsCredentialTypes.CredentialType)base["type"]; }
            set { base["type"] = value; }
        }

        /// <summary>
        /// See CredentialPersist. Values are: SESSION, LOCAL_MACHINE, ENTERPRISE
        /// </summary>
        [ConfigurationProperty("persist", DefaultValue = "LOCAL_MACHINE", IsKey = false, IsRequired = false)]
        public WindowsCredentialTypes.CredentialPersist Persist
        {
            get { return (WindowsCredentialTypes.CredentialPersist)base["persist"]; }
            set { base["persist"] = value; }
        }
    }
}
