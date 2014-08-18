using System;
using System.Reactive;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Microsoft.Reactive.Testing;
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
	public class SingleExecutionStrategyCanExecuteCustomization : ICustomization
	{
		private readonly bool _value;

		public SingleExecutionStrategyCanExecuteCustomization(bool value)
		{
			_value = value;
		}

		public void Customize(IFixture fixture)
		{
			fixture.Register<Func<object,bool>>(() => (_ => _value));
		}
	}

	public class SingleExecutionStrategyCanExecuteCompositeCustomization : CompositeCustomization
	{
		public SingleExecutionStrategyCanExecuteCompositeCustomization(bool value)
			: base(new AutoMoqCustomization(), new SingleExecutionStrategyCanExecuteCustomization(value))
		{
		}
	}

	public class SingleExecutionStrategyCanExecuteAutoDataAttribute : AutoDataAttribute
	{
		public SingleExecutionStrategyCanExecuteAutoDataAttribute(bool value)
			: base(new Fixture().Customize(new SingleExecutionStrategyCanExecuteCompositeCustomization(value)))
		{
		}
	}

	public class SingleExecutionStrategyCanExecuteInlineAutoDataAttribute : CompositeDataAttribute
	{
		public SingleExecutionStrategyCanExecuteInlineAutoDataAttribute(bool value, params object[] values)
			: base(new InlineDataAttribute(values), new SingleExecutionStrategyCanExecuteAutoDataAttribute(value))
		{
		}
	}
	#endregion

	public class SingleExecutionCanExecuteStrategyTests
	{
		[Theory, AutoData]
		public void Sut_IsCanExecuteStrategy(
		  SingleExecutionCanExecuteStrategy<object> sut)
		{
			sut.Should().BeAssignableTo<ICanExecuteStrategy<object>>();
		}

		[Theory,
		AutoData
		]
		public void NotifyExecuting_WithNullParameter_ShouldRaiseCanExecuteChanged(
			bool isExecuting,
			[Frozen]TestScheduler scheduler,
		  SingleExecutionCanExecuteStrategy<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<Unit>();
			sut.CanExecuteChanged.Subscribe(observer);

			//act
			sut.NotifyExecuting(null);

			//assert
			observer.Values().Should().HaveCount(1);
		}

		[Theory,
		AutoData
		]
		public void NotifyExecuting_WithParameter_ShouldRaiseCanExecuteChanged(
			bool isExecuting,
			[Frozen]TestScheduler scheduler,
		  SingleExecutionCanExecuteStrategy<object> sut,
			object parameter)
		{
			//arrange
			var observer = scheduler.CreateObserver<Unit>();
			sut.CanExecuteChanged.Subscribe(observer);

			//act
			sut.NotifyExecuting(parameter);

			//assert
			observer.Values().Should().HaveCount(1);
		}

		[Theory,
		AutoData
		]
		public void NotifyNotExecuting_WithNullParameter_ShouldRaiseCanExecuteChanged(
			bool isExecuting,
			[Frozen]TestScheduler scheduler,
		  SingleExecutionCanExecuteStrategy<object> sut)
		{
			//arrange
			var observer = scheduler.CreateObserver<Unit>();
			sut.CanExecuteChanged.Subscribe(observer);

			//act
			sut.NotifyNotExecuting(null);

			//assert
			observer.Values().Should().HaveCount(1);
		}

		[Theory,
		AutoData
		]
		public void NotifyNotExecuting_WithParameter_ShouldRaiseCanExecuteChanged(
			bool isExecuting,
			[Frozen]TestScheduler scheduler,
		  SingleExecutionCanExecuteStrategy<object> sut,
			object parameter)
		{
			//arrange
			var observer = scheduler.CreateObserver<Unit>();
			sut.CanExecuteChanged.Subscribe(observer);

			//act
			sut.NotifyNotExecuting(parameter);

			//assert
			observer.Values().Should().HaveCount(1);
		}

		[Theory, InlineAutoData(true), InlineAutoData(false)]
		public void CanExecute_ShouldReturnPredicateValue(
		 bool expected)
		{
			//arrange
			var sut = new SingleExecutionCanExecuteStrategy<object>(_ => expected);

			//act
			var actual = sut.CanExecute(null);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, SingleExecutionStrategyCanExecuteAutoData(true)]
		public void CanExecute_WhenCallingExecuting_ShouldReturnFalse(
		  SingleExecutionCanExecuteStrategy<object> sut)
		{
			//arrange
			sut.NotifyExecuting(null);

			//act
			var actual = sut.CanExecute(null);

			//assert
			actual.Should().BeFalse();
		}

		[Theory, SingleExecutionStrategyCanExecuteAutoData(true)]
		public void CanExecute_WhenCallingExecutingAndCanExecuteWithAnotherParameter_ShouldReturnFalse(
		  SingleExecutionCanExecuteStrategy<object> sut,
			object parameter)
		{
			//arrange
			sut.NotifyExecuting(null);

			//act
			var actual = sut.CanExecute(parameter);

			//assert
			actual.Should().BeFalse();
		}

		[Theory, SingleExecutionStrategyCanExecuteAutoData(true)]
		public void CanExecute_WhenCallingNotExecuting_ShouldReturnFalse(
		  SingleExecutionCanExecuteStrategy<object> sut)
		{
			//arrange
			sut.NotifyExecuting(null);
			sut.NotifyNotExecuting(null);

			//act
			var actual = sut.CanExecute(null);

			//assert
			actual.Should().BeTrue();
		}

		[Theory, SingleExecutionStrategyCanExecuteAutoData(true)]
		public void CanExecute_WhenCallingWithDifferentParameters_ShouldReturnFalse(
		  SingleExecutionCanExecuteStrategy<object> sut,
			object parameter1,
			object parameter2)
		{
			//arrange
			sut.NotifyExecuting(null);
			sut.NotifyNotExecuting(parameter1);

			//act
			var actual = sut.CanExecute(parameter2);

			//assert
			actual.Should().BeTrue();
		}

		[Theory, AutoData]
		public void Sut_ConstructorGuardClauses(
			GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructor<SingleExecutionCanExecuteStrategy<object>>();
		}
	}
}