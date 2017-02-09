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


using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.Infrastructure.ItemTracking;

namespace VP.FF.PT.Common.Infrastructure.UnitTests.ItemTracking
{
    [TestFixture]
    public class FifoItemRouterWithCounterTests
    {
        [Test]
        public void Item_count_works_on_add()
        {
            // setup
            var router = new FifoItemRouterWithCount<int>();

            // execute
            foreach (var r in Enumerable.Range(0, 10))
            {
                router.Route(r, 0);

                // verify
                router.Count().Should().Be(r + 1);
            }
        }

        [Test]
        public void Item_count_works_on_remove()
        {
            // setup
            var router = new FifoItemRouterWithCount<int>();
            foreach (var r in Enumerable.Range(0, 10))
            {
                router.Route(r, 0);
            }

            foreach (var r in Enumerable.Range(0, 10))
            {
                router.Remove(r);

                router.Count().Should().Be(10 - r - 1);
            }
        }
    }
}
