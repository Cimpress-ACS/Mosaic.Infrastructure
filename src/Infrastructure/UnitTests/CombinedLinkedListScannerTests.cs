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

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [TestFixture]
    public class CombinedLinkedListScannerTests
    {
        private LinkedListScanner<TestItem> _testee;
        private LinkedListScanner<TestItem> _testeeAfterwards;
   
        [SetUp]
        public void SetUp()
        {
            _testee = new LinkedListScanner<TestItem>();
            _testeeAfterwards = new LinkedListScanner<TestItem>();
        }

        [TearDown]
        public void TearDown()
        {
            _testee = null;
            _testeeAfterwards = null;
        }

        //           _testee  _testeeAfterwards
        //               |           |
        //   1 --> 2 --> 3 --> 4 --> 5
        [Test]
        public void GivenTwoScannersOnSameStream_ShouldBuildConsistendList()
        {
            _testee.MonitorEvents();
            _testeeAfterwards.MonitorEvents();

            _testee.ItemDetected(new TestItem(5));
            _testee.ItemDetected(new TestItem(4));
            _testeeAfterwards.ItemDetected(new TestItem(5));
            _testeeAfterwards.ItemDetected(new TestItem(4));
            _testee.ItemDetected(new TestItem(3));
            _testeeAfterwards.ItemDetected(new TestItem(3));
            _testee.ItemDetected(new TestItem(2));
            _testee.ItemDetected(new TestItem(1));
            _testeeAfterwards.ItemDetected(new TestItem(2));
            _testeeAfterwards.ItemDetected(new TestItem(1));

            _testee.ShouldNotRaise("ItemRemovedEvent");
            _testeeAfterwards.ShouldNotRaise("ItemRemovedEvent");
            var list1 = _testee.GetOrderedList();
            var list2 = _testeeAfterwards.GetOrderedList();
            list1.Count.Should().Be(5);
            list2.Count.Should().Be(5);
            list1[0].Should().Be(new TestItem(5));
            list1[4].Should().Be(new TestItem(1));
            list2[0].Should().Be(new TestItem(5));
            list2[4].Should().Be(new TestItem(1));
            list1[3].ItemInFront.Should().Be(list1[2]);
            list1[2].ItemBehind.Should().Be(list1[3]);
        }

        // _testee    _testeeAfterwards
        //     |             |
        //     1 --> (2) --> 3
        [Test]
        [Ignore("TODO: This feature needs to be implemented")]
        public void GivenTwoScannersOnSameStream_WhenItemRemovedOnSecondScanner_ShouldRemoveItem()
        {
            _testeeAfterwards.MonitorEvents();

            var item1 = new TestItem(1);
            var item2 = new TestItem(2);
            var item3 = new TestItem(3);

            _testee.ItemDetected(item3);
            _testee.ItemDetected(item2);
            _testeeAfterwards.ItemDetected(item3);
            _testee.ItemDetected(item1);
            _testeeAfterwards.ItemDetected(item1); // <-- item2 is missing

            _testeeAfterwards.ShouldRaise("ItemRemovedEvent");
        }

        // _testee    _testeeAfterwards
        //     |             |
        //     1 --> +2+ --> 3
        [Test]
        public void GivenTwoScannersOnSameStream_WhenItemAddedOnSecondScanner_ShouldAddItem()
        {
            var item1 = new TestItem(1);
            var item2 = new TestItem(2);
            var item3 = new TestItem(3);

            _testee.ItemDetected(item3);
            _testee.ItemDetected(item1);
            _testeeAfterwards.ItemDetected(item3);
            _testeeAfterwards.MonitorEvents();
            _testeeAfterwards.ItemDetected(item2); // <-- item2 is new

            _testeeAfterwards.ShouldRaise("NewItemDetectedEvent");
            _testee.GetOrderedList().Last().ItemInFront.Should().Be(item2, "_testee works on same datastructure as _testeeAfterwards");
        }
        
    }
}
