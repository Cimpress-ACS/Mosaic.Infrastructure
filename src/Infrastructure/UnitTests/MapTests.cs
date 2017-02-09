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
    public class MapTests
    {
        private Map<int, string> _testee;

        [SetUp]
        public void SetUp()
        {
            _testee = new Map<int, string>
                {
                    {1, "1"}, 
                    {2, "2"}, 
                    {3, "3"}
                };
        }

        [TearDown]
        public void TearDown()
        {
            _testee = null;
        }

        [Test]
        public void GivenMapContainsKey1_DictionaryShouldReturnProperValue()
        {
            _testee[1].Should().Be("1", "map contains keys and values 1, 2, 3");
        }

        [Test]
        public void GivenMapContainsKey3_ForwardIndexerShouldReturnProperValue()
        {
            _testee.Forward[3].Should().Be("3", "map contains keys and values 1, 2, 3");
        }

        [Test]
        public void GivenMapContainsValue2_ReverseIndexerShouldReturnProperKey()
        {
            _testee.Reverse["2"].Should().Be(2, "map contains keys and values 1, 2, 3");
        }

        [Test]
        public void GivenMapContainsValue2_ReverseIndexerContainsShouldReturnTrue()
        {
            _testee.Reverse.Contains("2").Should().BeTrue("map contains keys and values 1, 2, 3");
        }

        [Test]
        public void GivenMapDoesNotContainsValue9_ForwardIndexerContainsShouldReturnFalse()
        {
            _testee.Forward.Contains(9).Should().BeFalse("map contains only keys and values 1, 2, 3");
        }
    }
}
