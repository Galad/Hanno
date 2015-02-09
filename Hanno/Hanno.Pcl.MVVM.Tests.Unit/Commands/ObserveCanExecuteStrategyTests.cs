using System;
using System.Reactive;
using System.Reactive.Subjects;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Ploeh.AutoFixture;
using Xunit.Extensions;
using Xunit;
using FluentAssertions;
using Moq;

namespace Hanno.Tests.Commands
{
	#region Customization
	public class ObserveCanExecuteStrategyCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register<IObservable<bool>>(() => fixture.Create<Subject<bool>>());
		}
	}

	public class ObserveCanExecuteStrategyCompositeCustomization : CompositeCustomization
	{
		public ObserveCanExecuteStrategyCompositeCustomization()
			: base(new AutoMoqCustomization(), new ObserveCanExecuteStrategyCustomization())
		{
		}
	}

	public class ObserveCanExecuteStrategyAutoDataAttribute : AutoDataAttribute
	{
		public ObserveCanExecuteStrategyAutoDataAttribute()
			: base(new Fixture().Customize(new ObserveCanExecuteStrategyCompositeCustomization()))
		{
		}
	}

	public class ObserveCanExecuteStrategyInlineAutoDataAttribute : CompositeDataAttribute
	{
		public ObserveCanExecuteStrategyInlineAutoDataAttribute(params object[] values)
			: base(new InlineDataAttribute(values), new ObserveCanExecuteStrategyAutoDataAttribute())
		{
		}
	}
	#endregion

	public class ObserveCanExecuteStrategyTests
	{
		[Theory, AutoMoqData]
		public void Sut_ShouldBeCanExecuteStrategy(
		  ObserveCanExecuteStrategy<object> sut)
		{
			sut.Should().BeAssignableTo<ICanExecuteStrategy<object>>();
		}

		[Theory, AutoMoqData]
		public void Sut_GuardClauses(
		  GuardClauseAssertion	assertion)
		{
			//arrange

			//act
			assertion.VerifyConstructors<ObserveCanExecuteStrategy<object>>();

			//assert
		}

		[Theory, AutoMoqData]
		public void NotifyExecuting_ShouldCallInner(
			[Frozen]Mock<ICanExecuteStrategy<object>> inner,
		  ObserveCanExecuteStrategy<object> sut,
			object arg)
		{
			//arrange

			//act
			sut.NotifyExecuting(arg);

			//assert
			inner.Verify(c => c.NotifyExecuting(arg));
		}

		[Theory, AutoMoqData]
		public void NotifyNotExecuting_ShouldCallInner(
			[Frozen]Mock<ICanExecuteStrategy<object>> inner,
		  ObserveCanExecuteStrategy<object> sut,
			object arg)
		{
			//arrange

			//act
			sut.NotifyNotExecuting(arg);

			//assert
			inner.Verify(c => c.NotifyNotExecuting(arg));
		}

		[Theory, ObserveCanExecuteStrategyAutoData]
		public void CanExecute_WhenObservableHasNoValue_ShouldReturnInner(
			[Frozen]Mock<ICanExecuteStrategy<object>> inner,
			ObserveCanExecuteStrategy<object> sut,
			object arg)
		{
			//arrange
			inner.Setup(i => i.CanExecute(arg)).Returns(true);

			//act
			var actual = sut.CanExecute(arg);

			//assert
			actual.Should().BeTrue();
		}

		[Theory, ObserveCanExecuteStrategyAutoData]
		public void CanExecute_WhenObservableIsFalse_ShouldReturnFalse(
			[Frozen]Subject<bool> canExecuteObservable,
			ObserveCanExecuteStrategy<object> sut,
			object arg)
		{
			//arrange
			canExecuteObservable.OnNext(false);

			//act
			var actual = sut.CanExecute(arg);

			//assert
			actual.Should().BeFalse();
		}

		[Theory, ObserveCanExecuteStrategyAutoData]
		public void CanExecute_WhenObservableIsTrue_ShouldReturnInner(
			[Frozen]Subject<bool> canExecuteObservable,
			[Frozen]Mock<ICanExecuteStrategy<object>> inner,
			ObserveCanExecuteStrategy<object> sut,
			object arg)
		{
			//arrange
			inner.Setup(i => i.CanExecute(arg)).Returns(false);
			canExecuteObservable.OnNext(true);

			//act
			var actual = sut.CanExecute(arg);

			//assert
			actual.Should().BeFalse();
		}

		[Theory, ObserveCanExecuteStrategyAutoData]
		public void CanExecuteChanged_WhenObservableYieldValue_ShouldYieldValue(
			[Frozen]Subject<bool> canExecuteObservable,
			[Frozen]Mock<ICanExecuteStrategy<object>> inner,
			ObserveCanExecuteStrategy<object> sut,
			TestSchedulers schedulers,
			object arg)
		{
			//arrange
			var observer = schedulers.CreateObserver<Unit>();
			sut.CanExecuteChanged.Subscribe(observer);
			

			//act
			canExecuteObservable.OnNext(true);

			//assert
			observer.Values().Should().HaveCount(1);
		}
	}
}