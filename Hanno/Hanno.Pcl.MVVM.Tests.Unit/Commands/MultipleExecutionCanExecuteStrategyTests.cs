using System;
using System.Linq;
using System.Reactive;
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
	public class MultipleExecutionCanExecuteStrategyCustomization : ICustomization
	{
		private bool _value;

		public MultipleExecutionCanExecuteStrategyCustomization(bool value)
		{
			_value = value;
		}

		public void Customize(IFixture fixture)
		{
			fixture.Register<Func<object, bool>>(() => o => _value);
		}
	}

	public class MultipleExecutionCanExecuteStrategyCompositeCustomization : CompositeCustomization
	{
		public MultipleExecutionCanExecuteStrategyCompositeCustomization(bool value)
			: base(new AutoMoqCustomization(), new MultipleExecutionCanExecuteStrategyCustomization(value))
		{
		}
	}

	public class MultipleExecutionCanExecuteStrategyAutoDataAttribute : AutoDataAttribute
	{
		public MultipleExecutionCanExecuteStrategyAutoDataAttribute(bool value)
			: base(new Fixture().Customize(new MultipleExecutionCanExecuteStrategyCompositeCustomization(value)))
		{
		}
	}

	public class MultipleExecutionCanExecuteStrategyInlineAutoDataAttribute : CompositeDataAttribute
	{
		public MultipleExecutionCanExecuteStrategyInlineAutoDataAttribute(bool value, params object[] values)
			: base(new InlineDataAttribute(values), new MultipleExecutionCanExecuteStrategyAutoDataAttribute(value))
		{
		}
	}
	#endregion

	public class MultipleExecutionCanExecuteStrategyTests
	{
		[Theory, AutoData]
		public void Sut_ShouldBeCanExecuteStrategy(
		  MultipleExecutionCanExecuteStrategy<object> sut)
		{
			sut.Should().BeAssignableTo<ICanExecuteStrategy<object>>();
		}

		[Theory, AutoData]
		public void Sut_VerifyGuardClauses(
		  GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<MultipleExecutionCanExecuteStrategy<object>>();
		}

		[Theory, AutoData]
		public void NotifyExecuting_CanExecuteChangedShouldYieldValue(
			[Frozen]TestSchedulers schedulers,
		  MultipleExecutionCanExecuteStrategy<object> sut,
			object arg)
		{
			//arrange
			var observer = schedulers.CreateObserver<Unit>();
			sut.CanExecuteChanged.Subscribe(observer);

			//act
			sut.NotifyExecuting(arg);

			//assert
			observer.Values().Should().HaveCount(1);
		}

		[Theory, AutoData]
		public void NotifyNotExecuting_CanExecuteChangedShouldYieldValue(
			[Frozen]TestSchedulers schedulers,
			MultipleExecutionCanExecuteStrategy<object> sut,
			object arg)
		{
			//arrange
			var observer = schedulers.CreateObserver<Unit>();
			sut.CanExecuteChanged.Subscribe(observer);

			//act
			sut.NotifyNotExecuting(arg);

			//assert
			observer.Values().Should().HaveCount(1);
		}

		[Theory, InlineAutoData(true), InlineAutoData(false)]
		public void CanExecute_ShouldReturnPredicateValue(
		 bool expected)
		{
			//arrange
			var sut = new MultipleExecutionCanExecuteStrategy<object>(_ => expected);

			//act
			var actual = sut.CanExecute(null);

			//assert
			actual.Should().Be(expected);
		}

		[Theory,
		MultipleExecutionCanExecuteStrategyAutoData(true)
		]
		public void CanExecute_WhenNotifyExecuting_ShouldReturnFalse(
		 MultipleExecutionCanExecuteStrategy<object> sut,
			object arg)
		{
			//arrange
			sut.NotifyExecuting(arg);

			//act
			var actual = sut.CanExecute(arg);

			//assert
			actual.Should().BeFalse();
		}

		[Theory,
		MultipleExecutionCanExecuteStrategyAutoData(true)
		]
		public void CanExecute_WhenNotifyNotExecuting_ShouldReturnTrue(
		 MultipleExecutionCanExecuteStrategy<object> sut,
			object arg)
		{
			//arrange
			sut.NotifyExecuting(arg);
			sut.NotifyNotExecuting(arg);

			//act
			var actual = sut.CanExecute(arg);

			//assert
			actual.Should().BeTrue();
		}

		[Theory,
		MultipleExecutionCanExecuteStrategyAutoData(true)
		]
		public void CanExecute_WhenNotifyExecutingWithAnotherParameter_ShouldReturnTrue(
		 MultipleExecutionCanExecuteStrategy<object> sut,
			object arg1,
			object arg2)
		{
			//arrange
			sut.NotifyExecuting(arg1);

			//act
			var actual = sut.CanExecute(arg2);

			//assert
			actual.Should().BeTrue();
		}
	}
}