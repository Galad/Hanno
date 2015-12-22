using System;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Test.Autofixture.Tests
{
	public class AutoMoqCustomizationTests
	{
		private readonly InterfaceWithAsyncMethods _sut;

		public AutoMoqCustomizationTests()
		{
			var fixture = new Fixture().Customize(new HannoCustomization());
			_sut = fixture.Create<InterfaceWithAsyncMethods>();
		}

		[Fact]
		public void CallingMethodWithTask_ShouldReturnCorrectValue()
		{
			//arrange
			//act
			var actual = _sut.DoSomething();
			//assert
			actual.Should().NotBeNull();
		}

		[Fact]
		public async Task AwaitMethodWithTask()
		{
			//arrange
			
			//act
			await _sut.DoSomething();
			//assert
		}

		[Fact]
		public async Task CallingMethodWithTaskReturningBool_ShouldReturnCorrectValue()
		{
			//arrange
			//act
			var actual = await _sut.ReturnSomething();
			//assert
			actual.Should().BeTrue();
		}

		[Fact]
		public async Task CallingMethodWithTaskReturningObject_ShouldReturnCorrectValue()
		{
			//arrange
			//act
			var actual = await _sut.ReturnObject();
			//assert
			actual.Should().NotBeNull();
		}

		[Fact]
		public async Task CallingMethodWithTaskReturningVersion_ShouldReturnCorrectValue()
		{
			//arrange
			//act
			var actual = await _sut.ReturnGuid();
			//assert
			actual.Should().NotBe(default(Guid));
		}

		[Fact]
		public async Task CallingMethodWithTaskReturningAbstraction_ShouldReturnCorrectValue()
		{
			//arrange
			//act
			var actual = await _sut.ReturnAbstraction();
			//assert
			actual.Should().NotBeNull();
		}        
	}

    public class AutoMoqCustomizationsTests2
    {
        [Fact]
        public void CallingDispose_ShouldCallDispose()
        {            
            var fixture = new Fixture().Customize(new HannoCustomization());
            var mock = fixture.Freeze<Mock<InterfaceWithDispose>>();            
            var sut = fixture.Create<ClassWithDependencyImplementingIDisposable>();
            sut.Do();
            mock.Verify(d => d.Dispose());
        }
    }

	public interface InterfaceWithAsyncMethods
	{
		Task DoSomething();
		Task<bool> ReturnSomething();
		Task<object> ReturnObject();
		Task<Guid> ReturnGuid();
		Task<IOtherInterface> ReturnAbstraction();
	}

	public interface IOtherInterface
	{
		void Do();
	}

    public class ClassWithDependencyImplementingIDisposable
    {
        private readonly InterfaceWithDispose _dependency;

        public ClassWithDependencyImplementingIDisposable(InterfaceWithDispose dependency)
        {
            if (dependency == null)
                throw new ArgumentNullException(nameof(dependency), $"{nameof(dependency)} is null.");
            _dependency = dependency;
        }

        public void Do()
        {
            _dependency.Dispose();
        }
    }

    public interface InterfaceWithDispose : IDisposable
    {
        void Do();
    }
}