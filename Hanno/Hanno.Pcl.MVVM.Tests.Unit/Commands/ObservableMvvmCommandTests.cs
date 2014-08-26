using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Microsoft.Reactive.Testing;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Commands
{
	#region Customization
	public class ObservableMvvmCommandCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register<IObservable<object>>(() => Observable.Return(fixture.Create<object>()));
			fixture.Register<IObservable<Unit>>(() => fixture.Create<Subject<Unit>>());
			fixture.Customize<Mock<ICanExecuteStrategy<object>>>(c => c.Do(cc => cc.Setup(ccc => ccc.CanExecuteChanged).Returns(fixture.Create<IObservable<Unit>>())));
		}
	}

	public class ObservableMvvmCommandCompositeCustomization : CompositeCustomization
	{
		public ObservableMvvmCommandCompositeCustomization()
			: base(new RxCompositeCustomization(), new ObservableMvvmCommandCustomization())
		{
		}
	}

	public class ObservableMvvmCommandAutoDataAttribute : AutoDataAttribute
	{
		public ObservableMvvmCommandAutoDataAttribute()
			: base(new Fixture().Customize(new ObservableMvvmCommandCompositeCustomization()))
		{
		}
	}

	public class ObservableMvvmCommandInlineAutoDataAttribute : CompositeDataAttribute
	{
		public ObservableMvvmCommandInlineAutoDataAttribute(params object[] values)
			: base(new InlineDataAttribute(values), new ObservableMvvmCommandAutoDataAttribute())
		{
		}
	}
	#endregion

	public class ObservableMvvmCommandTests : ReactiveTest
	{
		[Theory, RxAutoData]
		public void Sut_TestDefaultValues(
			ObservableMvvmCommand<object, object> sut,
			ISchedulers schedulers)
		{
			//arrange
			//act

			//assert
			sut.DoScheduler.Should().NotBeNull();
			sut.DoScheduler.Should().NotBeNull();
			sut.CanExecuteStrategy.Should().NotBeNull();
		}

		[Theory, RxInlineAutoData(true), RxInlineAutoData(false)]
		public void CanExecute_ShouldReturnCorrectValue(
			bool canExecute,
			[Frozen]Mock<ICanExecuteStrategy<object>> canExecuteStrategy,
			ObservableMvvmCommand<object, object> sut,
			object arg)
		{
			//arrange
			canExecuteStrategy.Setup(c => c.CanExecute(arg)).Returns(canExecute);

			//act
			var actual = sut.CanExecute(arg);

			//assert
			actual.Should().Be(canExecute);
		}

		[Theory, RxAutoData]
		public void Execute_ShouldBeCalledWithArg(
			IObservable<object> value,
			object arg,
			TestSchedulers schedulers)
		{
			//arrange
			object actual = null;
			var sut = new ObservableMvvmCommand<object, object>(o =>
			{
				actual = o;
				return value;
			},
			schedulers,
			schedulers,
			"name",
			new AlwaysTrueCanExecuteStrategy<object>());

			//act
			sut.Execute(arg);
			schedulers.AdvanceBy(200);

			//assert
			actual.Should().Be(arg);
		}

		[Theory, AutoMoqData]
		public void Execute_ShouldExecuteObservable(
			TestSchedulers schedulers)
		{
			//arrange
			bool observableCalled = false;
			var value = Observable.Return(Unit.Default, ImmediateScheduler.Instance)
				.Do(unit => observableCalled = true);
			var sut = new ObservableMvvmCommand<object, System.Reactive.Unit>(o => value, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>());

			//act
			sut.Execute(null);
			schedulers.AdvanceBy(200);

			//assert
			observableCalled.Should().BeTrue();
		}

		[Theory, AutoMoqData]
		public void Execute_ShouldCallCallback(
			object expected,
			TestSchedulers schedulers)
		{
			//arrange
			object actual = null;
			var value = Observable.Return(expected, ImmediateScheduler.Instance);
			var sut = new ObservableMvvmCommand<object, object>(
				o => value,
				schedulers,
				schedulers,
				"name",
				new AlwaysTrueCanExecuteStrategy<object>(),
				doObserver: () => Observer.Create<object>(o => actual = o),
				doScheduler: ImmediateScheduler.Instance);

			//act
			sut.Execute(null);
			schedulers.AdvanceBy(200);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public void Execute_ShouldCallOnError(
			Exception expected,
			TestSchedulers schedulers)
		{
			//arrange
			Exception actual = null;
			var value = Observable.Throw<object>(expected, ImmediateScheduler.Instance);
			var sut = new ObservableMvvmCommand<object, object>(
				o => value,
				schedulers,
				schedulers,
				"name",
				new AlwaysTrueCanExecuteStrategy<object>(),
				doObserver: () => Observer.Create<object>(_ => { }, e => actual = e),
				doScheduler: ImmediateScheduler.Instance);

			//act
			sut.Execute(null);
			schedulers.AdvanceBy(200);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, ObservableMvvmCommandAutoData]
		public void Execute_ShouldCallNotifyExecuting(
			[Frozen]Mock<ICanExecuteStrategy<object>> canExecute,
			ObservableMvvmCommand<object, object> sut,
			[Frozen]object obj,
			TestSchedulers schedulers)
		{
			//arrange

			//act
			sut.Execute(obj);
			schedulers.AdvanceBy(200);

			//assert
			canExecute.Verify(c => c.NotifyExecuting(obj), Times.Once());
		}

		[Theory, ObservableMvvmCommandAutoData]
		public void Execute_WhenComplete_ShouldCallNotifyNotExecuting(
			Mock<ICanExecuteStrategy<object>> canExecute,
			object obj,
			TestSchedulers schedulers)
		{
			//arrange
			var value = Observable.Return(obj, ImmediateScheduler.Instance);
			var sut = new ObservableMvvmCommand<object, object>(_ => value, schedulers, schedulers, "name", canExecute.Object);

			//act
			sut.Execute(obj);
			schedulers.AdvanceBy(200);

			//assert
			canExecute.Verify(c => c.NotifyNotExecuting(obj), Times.Once());
		}

		[Theory, ObservableMvvmCommandAutoData]
		public void Execute_WhenError_ShouldCallNotifyNotExecuting(
			Mock<ICanExecuteStrategy<object>> canExecute,
			Exception error,
			object obj,
			TestSchedulers schedulers)
		{
			//arrange
			var value = Observable.Throw<object>(error, ImmediateScheduler.Instance);
			var sut = new ObservableMvvmCommand<object, object>(_ => value, schedulers, schedulers, "name", canExecute.Object);

			//act
			sut.Execute(obj);
			schedulers.AdvanceBy(200);

			//assert
			canExecute.Verify(c => c.NotifyNotExecuting(obj), Times.Once());
		}

		[Theory, ObservableMvvmCommandAutoData]
		public void Execute_WhenErrorInErrorAction_ShouldCallNotifyNotExecuting(
			Mock<ICanExecuteStrategy<object>> canExecute,
			Exception error,
			object obj,
			TestSchedulers schedulers)
		{
			//arrange
			var value = Observable.Throw<object>(error, ImmediateScheduler.Instance);
			Func<CancellationToken, Exception, Task> errorTask = async (ct, ex) =>
			{
				throw new Exception();
			};
			var sut = new ObservableMvvmCommand<object, object>(_ => value, schedulers, schedulers, "name", canExecute.Object, errorTask: errorTask);

			//act
			sut.Execute(obj);
			schedulers.AdvanceBy(200);

			//assert
			canExecute.Verify(c => c.NotifyNotExecuting(obj), Times.Once());
		}

		[Theory, AutoMoqData]
		public void Execute_WithObservableYielding3Values_ShouldReturnCorrectResult(
			object value1,
			object value2,
			object value3,
			TestSchedulers schedulers,
			ThrowingTestScheduler scheduler)
		{
			//arrange

			var observable = scheduler.CreateColdObservable(OnNext(10, value1),
				OnNext(20, value2),
				OnNext(30, value3),
				OnCompleted(40, new object()));
			var observer = scheduler.CreateObserver<object>();
			var sut = new ObservableMvvmCommand<object, object>(o => observable, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>(), doObserver: () => observer, doScheduler: ImmediateScheduler.Instance);

			//act
			sut.Execute(null);
			//allow any work scheduled with the schedulers to advance
			schedulers.AdvanceBy(200);
			//allow notifications to advance
			scheduler.AdvanceBy(200);

			//assert
			observer.Messages.AssertEqual(OnNext(10, value1),
				OnNext(20, value2),
				OnNext(30, value3),
				OnCompleted(40, new object()));
		}


		[Theory, ObservableMvvmCommandAutoData]
		public void CanExecuteChanged_WhenCanExecuteStrategyObservableYieldValue_ShouldBeRaiseCanExecute(
			[Frozen]Subject<Unit> canExecuteChanged,
			[Frozen]TestSchedulers schedulers,
			ObservableMvvmCommand<object, object> sut
			)
		{
			//arrange
			sut.MonitorEvents();

			//act
			canExecuteChanged.OnNext(Unit.Default);
			schedulers.AdvanceBy(200);

			//assert
			sut.ShouldRaise("CanExecuteChanged");
		}

		[Theory, RxAutoData]
		public void Execute_WithError_ShouldExecuteErrorTask(
		  object obj,
			Exception expected,
			TestSchedulers schedulers)
		{
			//arrange
			Exception actual = null;
			var value = Observable.Throw<object>(expected, ImmediateScheduler.Instance);
			Func<CancellationToken, Exception, Task> errorTask = async (ct, ex) => actual = ex;
			var sut = new ObservableMvvmCommand<object, object>(_ => value, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>(), errorTask: errorTask);

			//act
			sut.Execute(obj);
			schedulers.AdvanceBy(200);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, RxAutoData]
		public void Accept_ShouldCallVisit(
		  ObservableMvvmCommand<object, object> sut,
			Mock<IMvvmCommandVisitor> visitor)
		{
			//arrange

			//act
			sut.Accept(visitor.Object);

			//assert
			visitor.Verify(v => v.Visit((IAsyncMvvmCommand)sut));
		}

		[Theory, RxAutoData]
		public void SetDefaultError_ShouldSetDefaultError(
		  Func<CancellationToken, Exception, Task> error,
			Func<object, IObservable<object>> observable,
			TestSchedulers schedulers)
		{
			//arrange
			var sut = new ObservableMvvmCommand<object, object>(observable, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>());

			//act
			sut.SetDefaultError(error);

			//assert
			sut.ErrorTask.Should().Be(error);
		}

		[Theory, RxAutoData]
		public void SetDefaultError_ShouldReturnTrue(
		  Func<CancellationToken, Exception, Task> error,
			Func<object, IObservable<object>> observable,
			TestSchedulers schedulers)
		{
			//arrange
			var sut = new ObservableMvvmCommand<object, object>(observable, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>());

			//act
			var actual = sut.SetDefaultError(error);

			//assert
			actual.Should().BeTrue();
		}

		[Theory, RxAutoData]
		public void SetDefaultError_WithNull_VerifyGuardClause(
			ObservableMvvmCommand<object, object> sut,
			GuardClauseAssertion assertion)
		{
			assertion.Verify(() => sut.SetDefaultError(null));
		}

		[Theory, RxAutoData]
		public void SetDefaultError_ErrorAlreadySetFromConstructor_ShouldNotSetDefaultError(
			Func<CancellationToken, Exception, Task> errorTask,
			Func<CancellationToken, Exception, Task> expectedErrorTask,
			Func<object, IObservable<object>> observable,
			TestSchedulers schedulers)
		{
			//arrange
			var sut = new ObservableMvvmCommand<object, object>(observable, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>(), errorTask: expectedErrorTask);

			//act
			sut.SetDefaultError(errorTask);

			//assert
			sut.ErrorTask.Should().Be(expectedErrorTask);
		}

		[Theory, RxAutoData]
		public void SetDefaultError_ErrorAlreadySetFromConstructor_ShouldReturnFalse(
			Func<CancellationToken, Exception, Task> errorTask,
			Func<CancellationToken, Exception, Task> expectedErrorTask,
			Func<object, IObservable<object>> observable,
			TestSchedulers schedulers)
		{
			//arrange
			var sut = new ObservableMvvmCommand<object, object>(observable, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>(), errorTask: expectedErrorTask);

			//act
			var actual = sut.SetDefaultError(errorTask);

			//assert
			actual.Should().BeFalse();
		}

		[Theory, RxAutoData]
		public void DecorateValueFactory_ShouldDecorateFactory(
			Func<object, IObservable<object>> observable,
			IObservable<object> expected,
			TestSchedulers schedulers)
		{
			//arrange
			var sut = new ObservableMvvmCommand<object, object>(observable, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>());

			//act
			sut.DecorateValueFactory((_, __) => expected);
			var actual = sut.Factory(new object());

			//assert
			actual.Should().Be(expected);
		}

		[Theory, RxAutoData]
		public void DecorateDoFactory_ShouldDecorateDo(
			Func<object, IObservable<object>> observable,
			IObserver<object> expected,
			TestSchedulers schedulers)
		{
			//arrange
			var sut = new ObservableMvvmCommand<object, object>(observable, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>());

			//act
			sut.DecorateDo(observer => expected);
			var actual = sut.DoObserver();

			//assert
			actual.Should().Be(expected);
		}

		[Theory, RxAutoData]
		public void DecorateValueFactory_VerifyGuardClause(
			ObservableMvvmCommand<object, object> sut,
			GuardClauseAssertion assertion)
		{
			assertion.Verify(() => sut.DecorateDo(null));
		}

		[Theory, RxAutoData]
		public void DecorateDoFactory_VerifyGuardClause(
			ObservableMvvmCommand<object, object> sut,
			GuardClauseAssertion assertion)
		{
			assertion.Verify(() => sut.DecorateValueFactory(null));
		}

		[Theory, RxAutoData]
		public void Execute_WhenExuctingWhileOtherExecutionIsOccuring_ShouldCancelPreviousExecution(
		  [Frozen]TestSchedulers schedulers)
		{
			//arrange
			var firstObservable = schedulers.CreateColdObservable(OnNext(200, new object()), OnNext(300, new object()));
			var secondObservable = schedulers.CreateColdObservable(OnNext(400, new object()), OnNext(500, new object()));
			var i = 0;
			Func<object, IObservable<object>> observableFactory = _ => ++i == 1 ? firstObservable : secondObservable;
			var sut = new ObservableMvvmCommand<object, object>(observableFactory, schedulers, schedulers, "name", new AlwaysTrueCanExecuteStrategy<object>());

			//act
			sut.Execute(null);
			schedulers.AdvanceBy(250);
			sut.Execute(null);
			schedulers.AdvanceBy(350);

			//assert
			firstObservable.Subscriptions.Single().Unsubscribe.Should().Be(250);
		}
	}
}