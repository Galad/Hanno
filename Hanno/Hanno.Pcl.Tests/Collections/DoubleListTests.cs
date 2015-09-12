using System.Collections.Generic;
using FluentAssertions;
using Hanno.Extensions;
using Hanno.Collection;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.Collections
{
    public class DoubleListTests
    {
        [Theory, AutoData]
        public void Add_ShouldAddItem(
          DoubleList<object> sut,
            object expected)
        {
            //arrange

            //act
            sut.Add(expected);

            //assert
            sut.Should().Contain(expected);
        }

        [Theory, AutoData]
        public void SecondayListAdd_ShouldAddItem(
          DoubleList<object> sut,
            object expected)
        {
            //arrange

            //act
            sut.SecondaryList.Add(expected);

            //assert
            sut.Should().Contain(expected);
        }

        [Theory, AutoData]
        public void Remove_ShouldRemoveItem(
          DoubleList<object> sut,
            object expected)
        {
            //arrange
            sut.Add(expected);

            //act
            sut.Remove(expected);

            //assert
            sut.Should().NotContain(expected);
        }

        [Theory, AutoData]
        public void Remove_WhenItemIsInSecondaryList_ShouldRemoveItem(
          DoubleList<object> sut,
            object expected)
        {
            //arrange
            sut.SecondaryList.Add(expected);

            //act
            sut.Remove(expected);

            //assert
            sut.Should().NotContain(expected);
            sut.SecondaryList.Should().NotContain(expected);
        }

        [Theory, AutoData]
        public void GetEnumerator_ShouldReturnAllItemsInOrder(
          DoubleList<object> sut,
            object item,
            object secondaryItem)
        {
            //arrange
            sut.SecondaryList.Add(secondaryItem);
            sut.Add(item);

            //act
            var actual = new List<object>();
            foreach (var i in sut)
            {
                actual.Add(i);
            }

            //assert
            var expected = new List<object>() { item, secondaryItem };
            actual.ShouldAllBeEquivalentTo(expected);
        }

        [Theory, InlineAutoData(2), InlineAutoData(-1)]
        public void Insert_ShouldInsertItem(
            int factor,
            object item,
          DoubleList<object> sut,
            object[] items,
            object[] secondaryItems)
        {
            //arrange
            sut.AddRange(items);
            sut.SecondaryList.AddRange(secondaryItems);
            var index = items.Length - 1 + factor;
            //act

            sut.Insert(index, item);

            //assert
            sut.Should().HaveElementAt(index, item);
        }
    }
}
