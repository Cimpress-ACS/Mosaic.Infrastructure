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
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace VP.FF.PT.Common.Infrastructure.UnitTests
{
    [Serializable]
    public class TestItem : Item, ILinkedItem<TestItem>
    {
        public TestItem(int id)
        {
            Id = id;
        }

        public long Id { get; private set; }
        public TestItem ItemInFront { get; set; }
        public TestItem ItemBehind { get; set; }

        public override bool Equals(object obj)
        {
            return Id == ((TestItem)obj).Id;
        }

        public override int GetHashCode()
        {
            return (int) Id;
        }
    }

    [TestFixture]
    public class LinkedListScannerTests
    {
        private LinkedListScanner<TestItem> _testee;
            
        [SetUp]
        public void SetUp()
        {
            _testee = new LinkedListScanner<TestItem>();
        }

        [TearDown]
        public void TearDown()
        {
            _testee = null;
        }

        [Test]
        public void GivenNoItem_ShouldReturnEmptyList()
        {
            _testee.Items.Should().HaveCount(0);
            _testee.GetOrderedList().Should().BeEmpty();
        }

        [Test]
        public void WhenDetectItems_ShouldBeInTheList_ButOnlyOne()
        {
            var item = new TestItem(1);
            _testee.ItemDetected(item);

            _testee.Items.Should().ContainKey(item);

            _testee.ItemDetected(item);

            _testee.Items.Should().ContainKey(item);
            _testee.Items.Should().HaveCount(1, "same item was detected twice");
            _testee.GetOrderedList().Should().Contain(item);
            _testee.GetOrderedList().Should().HaveCount(1);
            _testee.Contains(item).Should().BeTrue();
        }

        [Test]
        public void GivenItem_WhenDetectNewItem_ShouldRaiseEvent()
        {
            var oldItem = new TestItem(1);
            _testee.ItemDetected(oldItem);
            _testee.MonitorEvents();

            _testee.ItemDetected(oldItem);

            _testee.ShouldNotRaise("NewItemDetectedEvent", "this item is not new it was just scanned a second time");

            var newItem = new TestItem(2);
            _testee.ItemDetected(newItem);

            _testee
                .ShouldRaise("NewItemDetectedEvent")
                .WithArgs<ItemEventArgs<TestItem>>(x => x.Item == newItem);

            _testee
                .ShouldRaise("ItemDetectedEvent")
                .WithArgs<ItemEventArgs<TestItem>>(x => x.Item == newItem);
        }

        [Test]
        public void WhenDetectItemsInARow_ShouldLinkItCorreclty()
        {
            var firstItem = new TestItem(1);
            var secondItem = new TestItem(2);
            var thirdItem = new TestItem(3);

            _testee.ItemDetected(firstItem);
            _testee.ItemDetected(secondItem);
            _testee.ItemDetected(thirdItem);

            _testee.Items.Should().HaveCount(3);
            firstItem.ItemInFront.Should().BeNull();
            firstItem.ItemBehind.Should().Be(secondItem);
            secondItem.ItemInFront.Should().Be(firstItem);
            secondItem.ItemBehind.Should().Be(thirdItem);
            thirdItem.ItemInFront.Should().Be(secondItem);
            thirdItem.ItemBehind.Should().BeNull();
        }

        [Test]
        public void GivenItemsInCircularBuffer_WhenRemoveItem_ShouldWireUpCorrectly()
        {
            var firstItem = new TestItem(1);
            var secondItem = new TestItem(2);
            var thirdItem = new TestItem(3);
            _testee.ItemDetected(firstItem);
            _testee.ItemDetected(secondItem);
            _testee.ItemDetected(thirdItem);
            _testee.ItemDetected(firstItem); // detect again first item -> circular

            _testee.RemoveItem(secondItem);

            _testee.Items.Should().HaveCount(2);
            firstItem.ItemInFront.Should().Be(thirdItem);
            firstItem.ItemBehind.Should().Be(thirdItem);
            secondItem.ItemInFront.Should().BeNull();
            secondItem.ItemBehind.Should().BeNull();
            thirdItem.ItemInFront.Should().Be(firstItem);
            thirdItem.ItemBehind.Should().Be(firstItem);
        }

        [Test]
        public void GivenItemsInARow_WhenRemoveMultipleItems_ShouldWireUpCorrectly()
        {
            var item1 = new TestItem(1);
            var item2 = new TestItem(2);
            var item3 = new TestItem(3);
            var item4 = new TestItem(4);
            _testee.ItemDetected(item1);
            _testee.ItemDetected(item2);
            _testee.ItemDetected(item3);
            _testee.ItemDetected(item4);

            _testee.RemoveItem(item3);
            _testee.RemoveItem(item4);

            _testee.Count.Should().Be(2);
            _testee.GetOrderedList().Count.Should().Be(2);
            item3.ItemBehind.Should().BeNull();
            item3.ItemInFront.Should().BeNull();
            item4.ItemBehind.Should().BeNull();
            item4.ItemInFront.Should().BeNull();
        }

        [Test]
        public void GivenItemsInCircularBuffer_WhenRemoveMultipleItems_ShouldWireUpCorrectly()
        {
            var items = SetupDetectItems(4);
            _testee.ItemDetected(items[0]); // detect again first item -> circular

            _testee.RemoveItem(items[0]);
            _testee.RemoveItem(items[1]);

            _testee.Count.Should().Be(2);
            _testee.GetOrderedList().Count.Should().Be(2);
            items[0].ItemBehind.Should().BeNull();
            items[0].ItemInFront.Should().BeNull();
            items[1].ItemBehind.Should().BeNull();
            items[1].ItemInFront.Should().BeNull();
        }

        [Test]
        public void GivenTwoItemsInCircularBuffer_WhenOneItems_ShouldWireUpCorrectly()
        {
            var items = SetupDetectItems(2);
            _testee.ItemDetected(items[0]); // detect again first item -> circular

            _testee.RemoveItem(items[0]);

            _testee.Count.Should().Be(1);
            _testee.GetOrderedList().Count.Should().Be(1);
            items[0].ItemBehind.Should().BeNull();
            items[0].ItemInFront.Should().BeNull();
            items[1].ItemBehind.Should().BeNull();
            items[1].ItemInFront.Should().BeNull();
        }

        [Test]
        public void GivenSingleItem_WhenRemove_ShouldBeEmpty()
        {
            var item = new TestItem(1);
            _testee.ItemDetected(item);

            _testee.RemoveItem(item);

            _testee.Count.Should().Be(0);
            _testee.GetOrderedList().Count.Should().Be(0);
            item.ItemBehind.Should().BeNull();
            item.ItemInFront.Should().BeNull();
       }

        [Test]
        public void GivenThreeItemsInCircularBuffer_WhenOneNotDetected_ShouldRemoveAndRaiseEvent()
        {
            var firstItem = new TestItem(1);
            var secondItem = new TestItem(2);
            var thirdItem = new TestItem(3);
            _testee.ItemDetected(firstItem);
            _testee.ItemDetected(secondItem);
            _testee.ItemDetected(thirdItem);
            _testee.MonitorEvents();

            _testee.ItemDetected(firstItem); // detect again first item -> circular
            // second item disappeared
            _testee.ItemDetected(thirdItem);

            _testee.GetOrderedList().Should().HaveCount(2, "one item suddenly disappeared");
            _testee
                .ShouldRaise("ItemUnexpectedlyDisappearedEvent")
                .WithArgs<ItemEventArgs<TestItem>>(x => x.Item == secondItem);
            firstItem.ItemInFront.Should().Be(thirdItem, "it's a circulating buffer");
            firstItem.ItemBehind.Should().Be(thirdItem, "it's a circulating buffer");
            secondItem.ItemInFront.Should().BeNull("this item has been removed");
            secondItem.ItemBehind.Should().BeNull("this item has been removed");
            thirdItem.ItemInFront.Should().Be(firstItem, "it's a circulating buffer");
            thirdItem.ItemBehind.Should().Be(firstItem, "it's a circulating buffer");
        }

        [Test]
        public void GivenFiveItemsInARow_WhenTwoInARowNotDetected_ShouldRemoveAndRaiseEvent()
        {
            var firstItem = new TestItem(1);
            var secondItem = new TestItem(2);
            var thirdItem = new TestItem(3);
            var fourthItem = new TestItem(4);
            var fifthItem = new TestItem(5);
            _testee.ItemDetected(firstItem);
            _testee.ItemDetected(secondItem);
            _testee.ItemDetected(thirdItem);
            _testee.ItemDetected(fourthItem);
            _testee.ItemDetected(fifthItem);
            _testee.MonitorEvents();

            _testee.ItemDetected(firstItem);
            _testee.ItemDetected(secondItem);
            // third item disappeared
            // fourth item disappeared
            _testee.ItemDetected(fifthItem);

            _testee.Items.Should().HaveCount(3, "two items suddenly disappeared");
            _testee.GetOrderedList().Should().HaveCount(3, "two items suddenly disappeared");
            _testee.ShouldRaise("ItemUnexpectedlyDisappearedEvent");
        }

        [Test]
        public void GivenTwoItemsDetectedInARow_WhenGetItems_ShouldReturnOrderedList()
        {
            var firstItem = new TestItem(1);
            var secondItem = new TestItem(2);
            _testee.ItemDetected(firstItem);
            _testee.ItemDetected(secondItem);

            var list = _testee.GetOrderedList();

            list.Should().HaveCount(2);
            list[0].Should().Be(firstItem);
            list[1].Should().Be(secondItem);
        }

        // <---  1 - 2 - 3 - (1) - (2) ...
        [Test]
        public void GivenThreeItemsDetectedInACirculatingBuffer_WhenGetItems_ShouldReturnOrderedListBeginningWithNextExpectedItem()
        {
            var firstItem = new TestItem(0);
            var secondItem = new TestItem(1);
            var thirdItem = new TestItem(2);
            _testee.ItemDetected(firstItem);
            _testee.ItemDetected(secondItem);
            _testee.ItemDetected(thirdItem);

            _testee.ItemDetected(firstItem);
            _testee.ItemDetected(secondItem); // last detected item

            var items = _testee.GetOrderedList();

            items.Should().HaveCount(3);
            items[0].Should().Be(thirdItem, "this item is expected next");
            items[1].Should().Be(firstItem);
            items[2].Should().Be(secondItem);

            // double check wirings
            items[0].ItemInFront.Should().Be(items[2]);
            items[0].ItemBehind.Should().Be(items[1]);
            items[1].ItemInFront.Should().Be(items[0]);
            items[1].ItemBehind.Should().Be(items[2]);
            items[2].ItemInFront.Should().Be(items[1]);
            items[2].ItemBehind.Should().Be(items[0]);
        }

        [Test]
        public void GivenSomeItems_WhenRemoveFront_ShouldBeRemoved_AndRaiseEvent()
        {
            var items = SetupDetectItems(5);
            _testee.MonitorEvents();

            _testee.RemoveFrontItem();

            _testee
                .ShouldNotRaise("ItemUnexpectedlyDisappearedEvent");
            _testee
                .ShouldRaise("ItemRemovedEvent")
                .WithArgs<ItemEventArgs<TestItem>>(x => x.Item.Equals(items[0]));
            _testee.Items.Should().HaveCount(4);
            _testee.GetOrderedList().Should().NotContain(items[0]);
            _testee.GetOrderedList()[0].ItemInFront.Should().BeNull("that is the new item in front");
        }

        [Test]
        public void GivenItem_WhenRemoveItem_ShouldBeRemoved_AndRaiseEvent()
        {
            var items = SetupDetectItems(1);
            _testee.MonitorEvents();

            _testee.RemoveItem(items[0]);

            _testee
                .ShouldNotRaise("ItemUnexpectedlyDisappearedEvent");
            _testee
                .ShouldRaise("ItemRemovedEvent")
                .WithArgs<ItemEventArgs<TestItem>>(x => x.Item.Equals(items[0]));
        }

        [Test]
        public void GivenTwoItemsInARow_WhenRemoveItem_ShouldSetLinksToNull()
        {
            var item1 = new TestItem(1);
            var item2 = new TestItem(2);
            _testee.ItemDetected(item1);
            _testee.ItemDetected(item2);

            _testee.RemoveItem(item2);

            item1.ItemBehind.Should().BeNull();
            item1.ItemInFront.Should().BeNull();
            item2.ItemBehind.Should().BeNull();
            item2.ItemInFront.Should().BeNull();
        }
        
        // 
        // <--- 1 - 2 - (1)
        //               |    
        //             remove
        [Test]
        public void GivenTwoItemsInARow_WhenRemoveItem_ShouldS2etLinksToNull()
        {
            var items = SetupDetectItems(2);
            _testee.ItemDetected(items[0]);

            _testee.RemoveItem(items[0]);

            items[0].ItemBehind.Should().BeNull();
            items[0].ItemInFront.Should().BeNull();
            items[1].ItemBehind.Should().BeNull();
            items[1].ItemInFront.Should().BeNull();
        }

        [Test]
        public void GivenSomeItems_WhenRemoveItemWhichDoesntExist_ShouldNotRaiseEvent()
        {
            SetupDetectItems(3);
            var item = new TestItem(99);
            
            _testee.MonitorEvents();

            _testee.RemoveItem(item);

            _testee
                .ShouldNotRaise("ItemUnexpectedlyDisappearedEvent");
            _testee
                .ShouldNotRaise("ItemRemovedEvent");
        }

        [Test]
        public void GivenNoItems_WhenRemoveItem_ShouldNotRaiseEvent()
        {
            var item = new TestItem(1);
            _testee.MonitorEvents();

            _testee.RemoveItem(item);

            _testee
                .ShouldNotRaise("ItemUnexpectedlyDisappearedEvent");
            _testee
                .ShouldNotRaise("ItemRemovedEvent");
        }

        [Test]
        public void GivenCirularBuffer_WhenRemoveFront_NothingHappens()
        {
            var items = SetupDetectItems(5);
            _testee.ItemDetected(items[0]); // detect again first item -> circular

            _testee.RemoveFrontItem();

            _testee.Items.Should().HaveCount(5);
        }

        [Test]
        public void GivenCircularBuffer_WhenDetectMissingById_ShouldRemove()
        {
            var items = SetupDetectItems(3);
            _testee.ItemDetected(items[0]); // detect again first item -> circular

            _testee.ItemDetected(new TestItem(2)); // detect other object, but same ID

            _testee.Items.Count.Should().Be(2);
            _testee.CircularLinkedListConsistencyCheck();
        }

        // <--- 0-1-2-3-4-0 -(1)-(2)- ...  <---
        //                 |
        //              insert 5
        [Test]
        public void GivenCircularBuffer_WhenDetectNewItem_ShouldHaveRightWiring()
        {
            var items = SetupDetectItems(5);
            _testee.ItemDetected(items[0]); // detect again first item -> circular

            var newItem = new TestItem(5);
            _testee.ItemDetected(newItem);

            newItem.ItemInFront.Should().Be(items[0]);
            newItem.ItemBehind.Should().Be(items[1]);
            items[0].ItemBehind.Should().Be(newItem);
            items[1].ItemInFront.Should().Be(newItem);
        }

        // <--- 0-1-2-3-4-0 -(1)-(2)- ...  <---
        //                 |
        //              insert 5
        [Test]
        public void GivenCircularBuffer_WhenDetectNewItem_ShouldRaiseEvent()
        {
            var items = SetupDetectItems(5);
            _testee.ItemDetected(items[0]); // detect again first item -> circular
            _testee.MonitorEvents();

            var newItem = new TestItem(5);
            _testee.ItemDetected(newItem);

            _testee
                .ShouldRaise("NewItemDetectedEvent")
                .WithArgs<ItemEventArgs<TestItem>>(x =>
                                               x.Item.ItemBehind == items[1] &&
                                               x.Item.ItemInFront == items[0]);
        }

        // <--- 0-1-2-3-4-(0) <---
        //          |___
        //              |
        // <--- 0-1-3-4-2-(0) <---
        [Test]
        public void GivenCircularBuffer_WhenSomeoneMovesItemBackwardsManually_ShouldFixAfterAWhile()
        {
            var items = SetupDetectItems(5);
            _testee.ItemDetected(items[0]); // detect again first item -> circular

            _testee.ItemDetected(items[1]);
            _testee.ItemDetected(items[3]);
            _testee.ItemDetected(items[4]);
            _testee.ItemDetected(items[2]);
            _testee.ItemDetected(items[0]);

            var list = _testee.GetOrderedList(); // returns next expected item which is 1
            list[0].Should().Be(items[1]);
            list[1].Should().Be(items[3]);
            list[2].Should().Be(items[4], "item 2 was suddenly moved");
            list[3].Should().Be(items[2]);
            list[4].Should().Be(items[0]);
        }

        // <--- 0-1-2-3-4-(0) <---
        //           ___|
        //          |    
        // <--- 0-1-4-2-3-(0) <---
        [Test]
        public void GivenCircularBuffer_WhenSomeoneMovesItemForwardsManually_ShouldFixAfterAWhile_ShouldNotRaiseEvents()
        {
            var items = SetupDetectItems(5);
            _testee.ItemDetected(items[0]); // detect again first item -> circular

            _testee.ItemDetected(items[1]);
            _testee.ItemDetected(items[4]);
            _testee.ItemDetected(items[2]);
            _testee.ItemDetected(items[3]);
            _testee.ItemDetected(items[0]);

            var list = _testee.GetOrderedList();
            list[0].Should().Be(items[1]);
            list[1].Should().Be(items[4]);
            list[2].Should().Be(items[2], "item 4 was suddenly moved");
            list[3].Should().Be(items[3]);
            list[4].Should().Be(items[0]);
        }

        // <--- 0-1-2-3-4-5  to  <--- 3-(3)
        [Test]
        public void GivenSomeItemsInCircularBuffer_WhenRemoveAllExceptOne_ShouldContainOne()
        {
            var items = SetupDetectItems(7);
            _testee.ItemDetected(items[0]); // detect again first item -> circular

            _testee.ItemDetected(items[3]);
            _testee.ItemDetected(items[3]);

            _testee.Items.Should().HaveCount(1);
        }

        [Test]
        public void GivenSomeItems_WhenDectectAsOtherObjectsButSameId_ShouldNotAddDuplicates()
        {
            _testee.ItemDetected(new TestItem(1));
            _testee.ItemDetected(new TestItem(2));
            _testee.ItemDetected(new TestItem(3));
            _testee.ItemDetected(new TestItem(1));

            _testee.Items.Should().HaveCount(3);
        }

        [Test]
        public void GivenTwoItemsCircular_WhenConsistencyCheck_ShouldNotThrowAnException()
        {
            var items = SetupDetectItems(2);
            _testee.ItemDetected(items[0]);

            Action act = () => _testee.CircularLinkedListConsistencyCheck();
            act.ShouldNotThrow();
        }

        [Test]
        [Ignore("UnitTest with timing behavior might break on the build machine...")]
        public void GivenManyItems_WhenNewDetected_ShouldCalculateFast()
        {
            var items = SetupDetectItems(1000000);
            _testee.ItemDetected(items[0]); // detect again first item -> circular

            _testee
                .ExecutionTimeOf(t => t.ItemDetected(items[1]))
                .ShouldNotExceed(TimeSpan.FromMilliseconds(300));

            _testee
                .ExecutionTimeOf(t => t.ItemDetected(new TestItem(999999999)))
                .ShouldNotExceed(TimeSpan.FromMilliseconds(300));
        }

        [Test]
        public void GivenSomeItems_WhenClear_ShouldClearAndReleaseAllLinks()
        {
            var items = SetupDetectItems(3);

            _testee.Clear();

            _testee.LastDetectedItem.Should().BeNull();
            _testee.Items.Should().BeEmpty();
            items[0].ItemInFront.Should().BeNull();
            items[0].ItemBehind.Should().BeNull();
            items[1].ItemInFront.Should().BeNull();
            items[1].ItemBehind.Should().BeNull();
            items[2].ItemInFront.Should().BeNull();
            items[2].ItemBehind.Should().BeNull();
        }

        [Test, Timeout(3000)]
        public void GiveManyItems_WhenNewDetectedWithRaceConditions_ShouldBeConsistent()
        {
            var items = SetupDetectItems(100);
            _testee.ItemDetected(items[0]); // detect again first item -> circular
            var random = new Random();

            // simulate race conditions
            Parallel.For(0, 1000, x =>
            {
                var rand = random.Next(99);
                _testee.ItemDetected(items[rand]);
                _testee.CircularLinkedListConsistencyCheck();
            });

            Action act = () => _testee.CircularLinkedListConsistencyCheck();
            act.ShouldNotThrow();
        }

        [Test, Timeout(30000)]
        [Ignore("for debugging only")]
        public void LongRunningTest_EndlessLoop()
        {
            for (int i = 0; i < 100; i++)
            {
                _testee = new LinkedListScanner<TestItem>();
                GiveManyItems_WhenNewDetectedWithRaceConditions_ShouldBeConsistent();
                Action act = () => _testee.CircularLinkedListConsistencyCheck();
                act.ShouldNotThrow();
            }
        }

        private IList<TestItem> SetupDetectItems(int count)
        {
            var list = new List<TestItem>();

            for (int i = 0; i < count; i++)
            {
                var item = new TestItem(i);
                list.Add(item);
                _testee.ItemDetected(item);
            }

            return list;
        }
    }
}
