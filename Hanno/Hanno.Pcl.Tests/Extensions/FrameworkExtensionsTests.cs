using System;
using FluentAssertions;
using Hanno.Extensions;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Pcl.Tests.Extensions
{
    public interface ITestInterface
    {
    }

    public class TestClass : ITestInterface
    {
    }

    public class TestClass2:ITestInterface{}

    public class TestClass<T> : TestClass{}
    public class TestClassLevel2 : TestClass<object>
    { }

    public class ObjectExtensionsTests
    {
        [Theory, AutoData]
        public void IsAssignableTo_WithAssignableType_ShouldReturnTrue(
          TestClass sut)
        {
            var actual = sut.IsAssignableTo<ITestInterface>();
            actual.Should().BeTrue();
        }

        [Theory, AutoData]
        public void IsAssignableTo_WithAssignableTypeWithGeneric_ShouldReturnTrue(
          TestClass<object> sut)
        {
            var actual = sut.IsAssignableTo<TestClass>();
            actual.Should().BeTrue();
        }

        [Theory, AutoData]
        public void IsAssignableTo_With2Levels_ShouldReturnTrue(
          TestClassLevel2 sut)
        {
            var actual = sut.IsAssignableTo<TestClass>();
            actual.Should().BeTrue();
        }

        [Theory, AutoData]
        public void IsAssignableTo_WithNonAssignableType_ShouldReturnFalse(
          TestClass2 sut)
        {
            var actual = sut.IsAssignableTo<TestClass>();
            actual.Should().BeFalse();
        }
    }
}