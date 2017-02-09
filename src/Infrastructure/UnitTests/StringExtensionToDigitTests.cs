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


using FluentAssertions;
using NUnit.Framework;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class StringExtensionToDigitTests
    {
        [Test]
        public void GivenOnlyDigits_WhenConvert_MustNotChange()
        {
            var testee = "12345";

            var result = testee.ToDigitsOnly();

            result.Should().Be("12345");
        }

        [Test]
        public void GivenCharactersOnly_WhenConvert_MustRemoveAll()
        {
            var testee = "abc";

            var result = testee.ToDigitsOnly();

            result.Should().Be("");
        }

        [Test]
        public void GivenMixedString_WhenConvert_MustRemoveOnlyCharacters()
        {
            var testee = "a1b2c345";

            var result = testee.ToDigitsOnly();

            result.Should().Be("12345");
        }
    }
}
