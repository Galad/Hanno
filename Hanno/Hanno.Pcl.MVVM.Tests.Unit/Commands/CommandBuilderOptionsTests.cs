using System;
using FluentAssertions;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Commands
{
    public class CommandBuilderOptionsTests
    {
        [Theory, RxAutoData]
        public void ToCommand_ShouldReturnCorrectValue(
            [Frozen] Action<object> action,
            CommandBuilderOptions<object> sut)
        {
            //arrange

            //act
            var actual = sut.ToCommand();

            //assert
            actual.Should().BeOfType<Command<object>>()
                  .And.Match<Command<object>>(options => options.Action == action);
        }

        [Theory, RxAutoData]
        public void ToCommand_WithCanExecute_ShouldReturnCorrectValue(
            CommandBuilderOptions<object> sut)
        {
            //arrange
	        
            //act
            var actual = sut.CanExecute(() => false).ToCommand();

            //assert
	        actual.As<Command<object>>().CanExecuteStrategy.Should().BeOfType<SingleExecutionCanExecuteStrategy<object>>()
	              .Which.CanExecutePredicate(null).Should().BeFalse();
        }

        [Theory, RxAutoData]
        public void ToCommand_WithCanExecuteOfT_ShouldReturnCorrectValue(
            CommandBuilderOptions<object> sut)
        {
            //arrange
            Func<object, bool> predicate = o => true;

            //act
            var actual = sut.CanExecute(predicate).ToCommand();

            //assert
	        actual.As<Command<object>>().CanExecuteStrategy.Should().BeOfType<SingleExecutionCanExecuteStrategy<object>>()
	              .Which.CanExecutePredicate(null).Should().BeTrue();
        }

		[Theory, AutoMoqData]
		public void ToCommand_WithCanExecuteObservable_ShouldReturnCorrectValue(
			CommandBuilderOptions<object> sut,
			IObservable<bool> predicate)
		{
			//arrange

			//act
			var actual = sut.CanExecute(predicate).ToCommand();

			//assert
			actual.As<Command<object>>().CanExecuteStrategy.Should().BeOfType<ObserveCanExecuteStrategy<object>>()
			      .Which.CanExecuteObservable.Should().Be(predicate);
		}

        [Theory, RxAutoData]
        public void CanExecute_ShouldReturnCorrectValue(
          CommandBuilderOptions<object> sut)
        {
            //arrange
            Func<bool> predicate = () => true;

            //act
            sut.CanExecute(predicate);

            //assert
            sut.CanExecutePredicate(null).Should().BeTrue();
            sut.ObservablePredicate.Should().BeNull();
        }

		[Theory, RxAutoData]
        public void CanExecuteOfT_ShouldReturnCorrectValue(
          CommandBuilderOptions<object> sut)
        {
            //arrange
            Func<object, bool> predicate = _ => true;

            //act
            sut.CanExecute(predicate);

            //assert
            sut.CanExecutePredicate.Should().Be(predicate);
            sut.ObservablePredicate.Should().BeNull();
        }

		[Theory, RxAutoData]
        public void CanExecute_WithObservable_ShouldReturnCorrectValue(
            IObservable<bool> observablePredicate,
            CommandBuilderOptions<object> sut)
        {
            //arrange

            //act
            sut.CanExecute(observablePredicate);

            //assert
            sut.ObservablePredicate.Should().Be(observablePredicate);
            sut.CanExecutePredicate.Should().BeNull();
        }
    }
}