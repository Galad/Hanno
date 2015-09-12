using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.Albedo;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class ConcurrencyExecutionAsyncCommandBusTests
	{
		public class TestCommand : IAsyncCommand
		{
			public CancellationToken CancellationToken
			{
				get
				{
					return CancellationToken.None;
				}
			}
		}

		[Theory, AutoMoqData]
		public void Sut_ShouldBeAsyncCommandBus(ConcurrencyExecutionAsyncCommandBus sut)
		{
			sut.Should().BeAssignableTo<IAsyncCommandBus>();
		}

		[Theory, AutoMoqData]
		public void Sut_VerifyConstructorGuardClauses(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<ConcurrencyExecutionAsyncCommandBus>();
		}

		[Theory, AutoMoqData]
		public void Sut_ProcessCommand_VerifyGuardClauses(
			GuardClauseAssertion assertion,
			IAsyncCommand command)
		{
			var method = new Methods<ConcurrencyExecutionAsyncCommandBus>()
				.Select(sut => sut.ProcessCommand(command));				
            assertion.Verify(method);
		}

		[Theory, AutoMoqData]
		public async Task Process_ShouldCallInnerProcessCommand(
			[Frozen]Mock<IAsyncCommandBus> mockCommandBus,
			ConcurrencyExecutionAsyncCommandBus sut,
			IAsyncCommand command)
		{
			//act
			await sut.ProcessCommand(command);

			//assert
			mockCommandBus.Verify(m => m.ProcessCommand(command));
		}

		[Theory, AutoMoqData]
		public async Task Process_WhenRegisteringCommandType_ShouldCallInnerProcessCommand(
			[Frozen]Mock<IAsyncCommandBus> mockCommandBus,
			ConcurrencyExecutionAsyncCommandBus sut,
			IAsyncCommand command)
		{
			//arrange
			sut.ForCommand<TestCommand>(1);

			//act
			await sut.ProcessCommand(command);

			//assert
			mockCommandBus.Verify(m => m.ProcessCommand(command));
		}

		[Theory, AutoMoqData]
		public async Task Process_ShouldRunCorrectNumberOfConcurrentInnerProcessCommand(
			[Frozen]Mock<IAsyncCommandBus> mockCommandBus,
			ConcurrencyExecutionAsyncCommandBus sut,
			TestCommand command)
		{
			//arrange
			mockCommandBus.Setup(m => m.ProcessCommand(It.IsAny<TestCommand>()))
				.ReturnsDefaultTask();
			sut.ForCommand<TestCommand>(1);

			//act
			var t1 = Task.Run(() => sut.ProcessCommand(command));
			var t2 = Task.Run(() => sut.ProcessCommand(command));
			await Task.WhenAll(t1, t2);

			//assert
			mockCommandBus.Verify(m => m.ProcessCommand(command), Times.Exactly(2));
		}

		[Theory, AutoMoqData]
		public void ForCommand_WhenRegisteringCommandTypeTwice_ShouldThrow(
			[Frozen]Mock<IAsyncCommandBus> mockCommandBus,
			ConcurrencyExecutionAsyncCommandBus sut,
			IAsyncCommand command)
		{
			//arrange
			sut.ForCommand<TestCommand>(1);

			//act
			Action action = () => sut.ForCommand<TestCommand>(1);

			//assert
			action.ShouldThrow<ArgumentException>();
		}
	}
}
