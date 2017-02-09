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
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Configuration;
using FluentAssertions;
using VP.FF.PT.Common.Infrastructure.Credentials;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    class CredentialConfigTests
    {
        private IProvideConfiguration _provideConfiguration ;
        private IList<string> _targetList;
        private IList<string> _userList;
        private IList<string> _passwordList;
        private IList<WindowsCredentialTypes.CredentialType> _typeList;
        private IList<WindowsCredentialTypes.CredentialPersist> _persistList;

        [SetUp]
        public void Setup()
        {
            _provideConfiguration = new ConfigurationAccessor();

            _targetList = new List<string>();
            _targetList.Add("TEST_DEFAULT");
            _targetList.Add("TEST_SPECIFIC");

            _userList = new List<string>();
            _userList.Add(@"cimpress\saber");
            _userList.Add(@"cimpress\sizilium");

            _passwordList = new List<string>();
            _passwordList.Add("Hansaplast");
            _passwordList.Add("GurkenNase");

            _typeList = new List<WindowsCredentialTypes.CredentialType>();
            _typeList.Add(WindowsCredentialTypes.CredentialType.GENERIC);
            _typeList.Add(WindowsCredentialTypes.CredentialType.DOMAIN_PASSWORD);

            _persistList = new List<WindowsCredentialTypes.CredentialPersist>();
            _persistList.Add(WindowsCredentialTypes.CredentialPersist.SESSION);
            _persistList.Add(WindowsCredentialTypes.CredentialPersist.LOCAL_MACHINE);
        }


        [TearDown]
        public void Teardown()
        {
            _provideConfiguration = null;
            _targetList = null;
            _userList = null;
            _passwordList = null;
            _typeList = null;
            _persistList = null;
        }

        [Test]
        public void ReadConfigurationAndCheckResult()
        {
            var credentialConfig = _provideConfiguration.GetConfiguration<CredentialConfigSection>("credentialSection");
            int iIdx = 0;

            credentialConfig.Should().NotBeNull("Credential definition is in the App.config");

            foreach (CredentialConfig credential in credentialConfig.Credentials)
            {
                credential.Target.Should().Be(_targetList[iIdx]);
                credential.User.Should().Be(_userList[iIdx]);
                credential.Password.Should().Be(_passwordList[iIdx]);
                credential.Type.Should().Be(_typeList[iIdx]);
                credential.Persist.Should().Be(_persistList[iIdx]);

                iIdx++;
            }
        }
    }
}
