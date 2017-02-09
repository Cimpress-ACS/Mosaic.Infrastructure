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
using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.Credentials;
using VP.FF.PT.Common.Infrastructure.Net;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class WindowsCredentialToolBoxTests
    {
        private WindowsCredentialToolBox _windowsCredentialToolBox;

        [SetUp]
        public void Setup()
        {
            _windowsCredentialToolBox = new WindowsCredentialToolBox();   
        }

        [TearDown]
        public void Teardown()
        {
            _windowsCredentialToolBox = null;
        }

        /// <summary>
        /// Test first if a credential can be added, then if it can be deleted. During this check also the function Exists will be tested.
        /// </summary>
        [Test]
        public void AddCheckAndDeleteCredential()
        {
            string target = "github.com";
            string user = "cimpress\\saber";
            string passWord = "Baum";

            var addResult = _windowsCredentialToolBox.Add(target, user, passWord, WindowsCredentialTypes.CredentialType.DOMAIN_PASSWORD, WindowsCredentialTypes.CredentialPersist.LOCAL_MACHINE);

            addResult.Should().BeTrue();

            var existResult = _windowsCredentialToolBox.Exist(target, WindowsCredentialTypes.CredentialType.DOMAIN_PASSWORD);

            existResult.Should().BeTrue();

            var deleteResult = _windowsCredentialToolBox.Delete(target,WindowsCredentialTypes.CredentialType.DOMAIN_PASSWORD);

            deleteResult.Should().BeTrue();

            var deleteConfirmResult = _windowsCredentialToolBox.Exist(target, WindowsCredentialTypes.CredentialType.DOMAIN_PASSWORD);

            deleteConfirmResult.Should().BeFalse();
        }
    }
}
