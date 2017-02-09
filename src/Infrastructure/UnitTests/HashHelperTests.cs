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
using System.Collections.ObjectModel;
using FluentAssertions;
using NUnit.Framework;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class HashHelperTests
    {
        private static readonly Collection<long> UsedNumbers = new Collection<long>();

        [TestCase("1")]
        [TestCase("2")]
        [TestCase("3")]
        [TestCase("F160298E - 3685 - 4F2D - A997 - 2B7B55CF93F9")]
        [TestCase("6D488412 - 7353 - 41F2 - B095 - 4A935D601C56")]
        [TestCase("5B1BCFA9 - A5F1 - 43F3 - 8480 - 36404A00ABCB")]
        [TestCase("crn:ffitem:00000000-0000-0000-0000-000000000000")]
        [TestCase("heliositn")]
        [TestCase("heliosidn")]
        [TestCase("tzcoqimpgh,xaoitgramzh")]
        [TestCase("7534298671658957482590780942")]
        [TestCase("7534298671658957482590780943")]
        [TestCase("6534298671658957482590780943")]
        [TestCase("6534298671658958482590780943")]
        public void Test(string inputString)
        {
            var action = new Action(() => UsedNumbers.Add(
                HashHelper.ConvertStringToLong(inputString)));

            action.ShouldNotThrow("HashHelper must be able to handle any string");
            UsedNumbers.Should().OnlyHaveUniqueItems("HashHelper must only produce unique identifiers");
        }
    }
}
