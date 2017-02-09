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
    public class StringExtensionIncreaseAnyNumberTests
    {
        [Test]
        public void GivenSimpleNumber_ShouldIncrease()
        {
            var test = "1";

            var result = test.IncreaseAnyNumber();

            result.Should().Be("2");
        }

        [Test]
        public void GivenBigNumber_ShouldIncrease()
        {
            var test = "10000000000000000000000000000000000";

            var result = test.IncreaseAnyNumber();

            result.Should().Be("10000000000000000000000000000000001");
        }

        [Test]
        public void Given190_ShouldIncreaseTo191()
        {
            var test = "190";

            var result = test.IncreaseAnyNumber();

            result.Should().Be("191");
        }

        [Test]
        public void GivenStringWithCharacter_ShouldIncreaseOnlyNumber()
        {
            var test = "a1b";

            var result = test.IncreaseAnyNumber();

            result.Should().Be("a2b");
        }

        [Test]
        public void GivenComplexString_ShouldIncreaseOnlyNumber()
        {
            var test = "abcABC-1-999-(X)";

            var result = test.IncreaseAnyNumber();

            result.Should().Be("abcABC-2-000-(X)");
        }

        [Test]
        public void GivenOverflowNumber_WhenIncrease_ShouldBeZero()
        {
            var test = "999A";

            var result = test.IncreaseAnyNumber();

            result.Should().Be("000A");
        }
    }
}
