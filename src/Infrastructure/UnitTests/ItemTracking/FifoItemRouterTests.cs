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
    public class FifoItemRouterTests
    {
        [Test]
        public void Added_item_can_be_removed()
        {
            // setup
            var router = new FifoItemRouter<int>();
            router.Route(0, 0);
            
            // execute
            
            var result = router.Remove(0);
            
            // verify
            result.Should().BeTrue();
        }

        [Test]
        public void Not_added_item_cannot_be_removed()
        {
            // setup
            var router = new FifoItemRouter<int>();
            router.Route(0, 0);
            
            // execute
            
            var result = router.Remove(1);
            
            // verify
            result.Should().BeFalse();
        }

        [Test]
        public void Remove_until_removes_all_items_before_this_item()
        {
            // setup
            var router = new FifoItemRouter<int>();

            foreach (var r in Enumerable.Range(0, 10))
            {
                router.Route(r, 0);
            }
            
            // execute
            var result = router.RemoveUntil(5);

            // verify
            result.Should().Equal(0, 1, 2, 3, 4, 5);
        }

        [Test]
        public void Remove_does_not_remove_item_if_unavailable()
        {
            // setup
            var router = new FifoItemRouter<int>();

            foreach (var r in Enumerable.Range(0, 10))
            {
                router.Route(r, 0);
            }

            // execute
            var result = router.RemoveUntil(15);

            // verify
            result.Should().Equal(new int[0]);
        }

        [Test]
        public void Remove_does_remove_all_items_if_unavailable()
        {
            // setup
            var router = new FifoItemRouter<int>();

            foreach (var r in Enumerable.Range(0, 10))
            {
                router.Route(r, 0);
            }

            // execute
            var result = router.RemoveUntil(15, false);

            // verify
            result.Should().Equal(Enumerable.Range(0, 10));
        }
    }
}
