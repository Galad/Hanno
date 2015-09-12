using FluentAssertions;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.Commands
{
	public class CommandTests
	{
		[Theory, AutoMoqData]
		public void Execute_ShouldExecuteAction(
			object arg,
			ISchedulers schedulers,
			Mock<ICanExecuteStrategy<object>> canExecute)
		{
			//arrange
			object actual = null;
			var sut = new Command<object>(o => actual = o, schedulers, "name", canExecute.Object);

			//act
			sut.Execute(arg);

			//assert
			actual.Should().Be(arg);
		}

		[Theory, InlineAutoMoqData(true), InlineAutoMoqData(false)]
		public void CanExecute_ShouldReturnCorrectValue(
			bool canExecute,
			object arg,
			ISchedulers schedulers,
			Mock<ICanExecuteStrategy<object>> canExecuteStrat)
		{
			//arrange
			canExecuteStrat.Setup(c => c.CanExecute(It.IsAny<object>())).Returns(canExecute);
			var sut = new Command<object>(o => { }, schedulers, "name", canExecuteStrat.Object);
			//act
			var actual = sut.CanExecute(arg);

			//assert
			actual.Should().Be(canExecute);
		}

		[Theory, AutoMoqData]
		public void CanExecute_ShouldBeCalledWithArg(
		  object arg,
			ISchedulers schedulers,
			Mock<ICanExecuteStrategy<object>> canExecuteStrat)
		{
			//arrange
			object actual = null;
			canExecuteStrat.Setup(c => c.CanExecute(It.IsAny<object>())).Returns(true);
			var sut = new Command<object>(o => { }, schedulers, "name", canExecuteStrat.Object);

			//act
			sut.CanExecute(arg);

			//assert
			canExecuteStrat.Verify(c => c.CanExecute(arg));
		}

		[Theory, AutoMoqData]
		public void RaiseCanExecute_ShouldRaiseCanExecute(
		  Command<object> sut)
		{
			//arrange
			sut.MonitorEvents();

			//act
			sut.RaiseCanExecute();

			//assert
			sut.ShouldRaise("CanExecuteChanged");
		}
	}
}