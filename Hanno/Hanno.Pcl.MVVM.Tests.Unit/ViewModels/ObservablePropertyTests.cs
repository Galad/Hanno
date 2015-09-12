using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.ViewModels
{
	public class ObservablePropertyTests
	{
		[Theory, AutoMoqData]
		public void TestValueProperty(
			WritablePropertyAssertion assertion,
			ISchedulers schedulers)
		{
			var sut = new ObservableProperty<string>(schedulers);
			assertion.VerifyProperty(() => sut.Value);
		}

		[Theory, RxAutoData]
		public void Subscribe_ShouldCallOnNextWhenSettingProperty(
			string value,
			Mock<IObserver<string>> observer,
			ISchedulers schedulers
			)
		{
			//arrange
			var sut = new ObservableProperty<string>(schedulers);
			sut.Subscribe(observer.Object);

			//act
			sut.Value = value;

			//assert
			observer.Verify(o => o.OnNext(value));
		}

		[Theory, AutoMoqData]
		public void Subscribe_WithDefaultValue_ShouldNotCallOnNext(
			string value,
			Mock<IObserver<string>> observer,
			ISchedulers schedulers)
		{
			//arrange
			var sut = new ObservableProperty<string>(schedulers);
			sut.Subscribe(observer.Object);

			//assert
			observer.Verify(o => o.OnNext(It.IsAny<string>()), Times.Never());
		}

		[Theory, AutoMoqData]
		public void OnNext_ShouldSetValue(
			string value,
			ISchedulers schedulers)
		{
			var sut = new ObservableProperty<string>(schedulers);

			//act
			sut.OnNext(value);

			//assert
			sut.Value.Should().Be(value);
		}

		[Theory, AutoMoqData]
		public void OnError_ShouldSetError(
			Exception value,
			ISchedulers schedulers)
		{
			var sut = new ObservableProperty<string>(schedulers);

			//act
			sut.OnError(value);

			//assert
			sut.Error.Should().Be(value);
		}

		[Theory, AutoMoqData]
		public async Task Sut_WhenAwaited_ShouldReturnCorrectValue(
			string expected,
			ISchedulers schedulers)
		{
			//arrange
			var sut = new ObservableProperty<string>(schedulers);
			sut.Value = expected;

			//act
			var actual = await sut;

			//assert
			actual.Should().Be(expected);
		}

		[Theory, RxAutoData]
		public async Task OnNext_WhenSubscribingThenPushingFirstValue_AndFirstValueIsDefaultValue_ShouldReturnCorrectValue(
			TestSchedulers schedulers)
		{
			//arrange
			var observer = schedulers.CreateObserver<int>();
			var sut = new ObservableProperty<int>(schedulers);
			var expected = sut.Value;
			sut.Subscribe(observer);
			
			//act
			sut.OnNext(expected);

			//assert
			observer.Values().Last().Should().Be(expected);
		}
	}
}
