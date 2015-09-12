using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.Commands
{
	public class ObservableCommandBuilderOptionsTests
	{
		[Theory, RxAutoData]
		public void ToCommand_ShouldReturnCorrectValue(
			IObservable<object> value,
			ISchedulers schedulers)
		{
			//arrange
			var sut = new ObservableCommandBuilderOptions<string, object>(s => value, command => { }, schedulers, "name");

			//act
			var actual = sut.ToCommand();

			//assert
			actual.Should().BeOfType<ObservableMvvmCommand<string, object>>();
			((ObservableMvvmCommand<string, object>)actual).Factory("").Should().Be(value);
		}

		[Theory, RxAutoData]
		public void ToCommand_ShouldCallAction(
			IObservable<object> value,
			ISchedulers schedulers)
		{
			//arrange
			bool actionCalled = false;
			var sut = new ObservableCommandBuilderOptions<string, object>(s => value, command => actionCalled = true, schedulers, "name");

			//act
			sut.ToCommand();

			//assert
			actionCalled.Should().BeTrue();
		}

		[Theory, RxAutoData]
		public void ToCommand_WithCanExecute_ShouldReturnCorrectValue(
			ObservableCommandBuilderOptions<string, object> sut)
		{
			//arrange

			//act
			var actual = sut.CanExecute(() => false).ToCommand();

			//assert
			actual.As<ObservableMvvmCommand<string, object>>().CanExecuteStrategy.Should().BeOfType<SingleExecutionCanExecuteStrategy<string>>()
			      .Which.CanExecutePredicate(null).Should().BeFalse();
		}

		[Theory, RxAutoData]
		public void ToCommand_WithoutCanExecute_ShouldReturnCorrectValue(
			ObservableCommandBuilderOptions<string, object> sut)
		{
			//arrange

			//act
			var actual = sut.ToCommand();

			//assert
			actual.As<ObservableMvvmCommand<string, object>>()
				  .CanExecuteStrategy
				  .Should()
				  .BeOfType<SingleExecutionCanExecuteStrategy<string>>()
				  .Which
				  .CanExecutePredicate(null)
				  .Should()
				  .BeTrue();
		}

		[Theory, RxAutoData]
		public void ToCommand_WithCanExecuteOfT_ShouldReturnCorrectValue(
			ObservableCommandBuilderOptions<string, object> sut)
		{
			//arrange
			Func<object, bool> predicate = o => true;

			//act
			var actual = sut.CanExecute(predicate).ToCommand();

			//assert
			actual.As<ObservableMvvmCommand<string, object>>().CanExecuteStrategy.Should().BeOfType<SingleExecutionCanExecuteStrategy<string>>()
				.Which.CanExecutePredicate.Should().Be(predicate);
		}

		[Theory, RxAutoData]
		public void ToCommand_WithCanExecuteObservable_ShouldReturnCorrectValue(
			ObservableCommandBuilderOptions<string, object> sut,
			IObservable<bool> predicate)
		{
			//arrange

			//act
			var actual = sut.CanExecute(predicate).ToCommand();

			//assert
			actual.As<ObservableMvvmCommand<string, object>>().CanExecuteStrategy.Should().BeOfType<ObserveCanExecuteStrategy<string>>()
				.Which.CanExecuteObservable.Should().Be(predicate);
		}

		[Theory, RxAutoData]
		public void ToCommand_WithDo_ShouldReturnCorrectValue(
			ObservableCommandBuilderOptions<string, object> sut,
			IObserver<object> callback,
			IScheduler scheduler)
		{
			//arrange

			//act
			var actual = sut.Do(() => callback).WithScheduler(scheduler).ToCommand();

			//assert
			actual.As<ObservableMvvmCommand<string, object>>().DoObserver().Should().Be(callback);
			actual.As<ObservableMvvmCommand<string, object>>().DoScheduler.Should().Be(scheduler);
		}


		[Theory, RxAutoData]
		public void ToCommand_WithObservableCanExecuteAndCallback_ShouldReturnCorrectValue(
			ObservableCommandBuilderOptions<string, object> sut,
			IObserver<object> callback,
			IScheduler scheduler,
			IObservable<bool> observableCanExecute)
		{
			//arrange

			//act
			var actual = sut.CanExecute(observableCanExecute).Do(() => callback).WithScheduler(scheduler).ToCommand();

			//assert
			actual.As<ObservableMvvmCommand<string, object>>().DoObserver().Should().Be(callback);
			actual.As<ObservableMvvmCommand<string, object>>().DoScheduler.Should().Be(scheduler);
		}

		[Theory, RxAutoData]
		public void CanExecute_ShouldSetCanExecute(
		  ObservableCommandBuilderOptions<string, object> sut)
		{
			//arrange

			//act
			sut.CanExecute(() => true);

			//assert
			sut.CanExecutePredicate(null).Should().BeTrue();
		}

		[Theory, RxAutoData]
		public void CanExecute_ShouldRemoveObservableCanExecute(
		  ObservableCommandBuilderOptions<string, object> sut,
			IObservable<bool> observableCanExecute)
		{
			//arrange
			sut.CanExecute(observableCanExecute);

			//act
			sut.CanExecute(() => true);

			//assert
			sut.ObservableCanExecute.Should().BeNull();
		}

		[Theory, RxAutoData]
		public void CanExecuteOfT_ShouldSetCanExecute(
		  ObservableCommandBuilderOptions<string, object> sut)
		{
			//arrange

			//act
			sut.CanExecute(_ => true);

			//assert
			sut.CanExecutePredicate(null).Should().BeTrue();
		}

		[Theory, RxAutoData]
		public void CanExecuteOfT_ShouldRemoveObservableCanExecute(
		  ObservableCommandBuilderOptions<string, object> sut,
			IObservable<bool> observableCanExecute)
		{
			//arrange
			sut.CanExecute(observableCanExecute);

			//act
			sut.CanExecute(_ => true);

			//assert
			sut.ObservableCanExecute.Should().BeNull();
		}

		[Theory, RxAutoData]
		public void CanExecuteObservable_ShouldSetCanExecute(
		  ObservableCommandBuilderOptions<string, object> sut,
			IObservable<bool> observableCanExecute)
		{
			//arrange

			//act
			sut.CanExecute(observableCanExecute);

			//assert
			sut.ObservableCanExecute.Should().Be(observableCanExecute);
		}

		[Theory, RxAutoData]
		public void CanExecuteObservable_ShouldRemoveObservableCanExecute(
		  ObservableCommandBuilderOptions<string, object> sut,
			IObservable<bool> observableCanExecute)
		{
			//arrange
			sut.CanExecute(() => true);

			//act
			sut.CanExecute(observableCanExecute);

			//assert
			sut.CanExecutePredicate.Should().BeNull();
		}

		[Theory, RxAutoData]
		public void Callback_ShouldSetCallback(
		  ObservableCommandBuilderOptions<string, object> sut,
			object expected)
		{
			//arrange
			object actual = null;

			//act
			sut.Do(o => actual = o);

			//assert
			sut.DoObserver().OnNext(expected);
			actual.Should().Be(expected);
		}

		[Theory, RxAutoData]
		public void OnError_ShouldSetOnError(
		  ObservableCommandBuilderOptions<string, object> sut,
			Action<Exception> action,
			Exception expected)
		{
			//arrange
			Exception actual = null;

			//act
			sut.Do(o => { }, e => actual = e);
			sut.DoObserver().OnError(expected);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, RxAutoData]
		public void Error_ShouldSetError(
		  ObservableCommandBuilderOptions<string, object> sut,
			Func<CancellationToken, Exception, Task> errorTask,
			Exception expected)
		{
			//arrange

			//act
			sut.Error(errorTask);

			//assert
			sut.ErrorTask.Should().Be(errorTask);
		}

		[Theory, RxAutoData]
		public void CallbackScheduler_ShouldSetScheduler(
			ObservableCommandBuilderOptions<string, object> sut,
			Action<object> action,
			IScheduler scheduler)
		{
			//arrange

			//act
			sut.Do(action).WithScheduler(scheduler);

			//assert
			sut.DoScheduler.Should().Be(scheduler);
		}


		[Theory, AutoMoqData]
		public void CallbackDefaultScheduler_ShouldSetScheduler(
			[Frozen]Mock<ISchedulers> schedulers,
			ObservableCommandBuilderOptions<string, object> sut,
			Action<object> action,
			IScheduler scheduler)
		{
			//arrange
			schedulers.Setup(schedulers1 => schedulers1.Immediate).Returns(scheduler);

			//act
			sut.Do(action).WithDefaultScheduler();

			//assert
			sut.DoScheduler.Should().Be(scheduler);
		}

		[Theory, RxAutoData]
		public void ToCommand_WhenUsingMultipleExecution_ShouldReturnCorrectValue(
			ObservableCommandBuilderOptions<string, object> sut 
			)
		{
			//arrange
			Func<object, bool> predicate = _ => true;

			//act
			var actual = sut.CanExecute(predicate)
			   .MultipleExecution()
			   .ToCommand();

			//assert
			actual.As<ObservableMvvmCommand<string, object>>().CanExecuteStrategy.Should().BeOfType<MultipleExecutionCanExecuteStrategy<string>>()
			      .Which.CanExecutePredicate.Should().Be(predicate);
		}

		[Theory, RxAutoData]
		public void ToCommand_WhenUsingObservablePredicateAndMultipleExecution_ShouldReturnCorrectValue(
			ObservableCommandBuilderOptions<string, object> sut,
			IObservable<bool>  predicate
			)
		{
			//arrange

			//act
			var actual = sut.CanExecute(predicate)
			   .MultipleExecution()
			   .ToCommand();

			//assert
			actual.As<ObservableMvvmCommand<string, object>>().CanExecuteStrategy.Should().BeOfType<ObserveCanExecuteStrategy<string>>()
			      .Which.InnerCanExecuteStrategy.Should().BeOfType<MultipleExecutionCanExecuteStrategy<string>>();
		}
	}
}