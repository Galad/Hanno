using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class AsyncCommandQueryBusTests
	{
		[Theory, AutoMoqData]
		public void Sut_IsAsyncQueryBus(AsyncCommandQueryBus sut)
		{
			sut.Should().BeAssignableTo<IAsyncQueryBus>();
		}

		[Theory, AutoMoqData]
		public void Sut_IsAsyncCommandBus(AsyncCommandQueryBus sut)
		{
			sut.Should().BeAssignableTo<IAsyncCommandBus>();
		}

		[Theory, AutoMoqData]
		public void Sut_ConstructorGuardClause(GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructors<AsyncCommandQueryBus>();
		}

		[Theory, AutoMoqData]
		public async Task ProcessQuery_ShouldReturnCorrectValue(
			[Frozen]Mock<IAsyncQueryCommandHandlerFactory> asyncQueryHandlerFactory,
			Mock<IAsyncQueryHandler<IAsyncQuery<string>, string>> queryHandler,
			IAsyncQuery<string> query,
			string expected,
			AsyncCommandQueryBus sut)
		{
			//arrange
			asyncQueryHandlerFactory.Setup(a => a.Create<IAsyncQuery<string>, string>()).Returns(queryHandler.Object);
			queryHandler.Setup(q => q.Execute(query)).ReturnsTask(expected);

			//act
			var actual = await sut.ProcessQuery<IAsyncQuery<string>, string>(query);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public void ProcessQuery_WhenQueryHandlerIsNotFound_ShouldThrow(
			[Frozen]Mock<IAsyncQueryCommandHandlerFactory> asyncQueryHandlerFactory,
			IAsyncQuery<string> query,			
			AsyncCommandQueryBus sut)
		{
			//arrange
			asyncQueryHandlerFactory.Setup(a => a.Create<IAsyncQuery<string>, string>())
				.Throws<InvalidOperationException>();
			
			//act
			Func<Task> action = async () => await sut.ProcessQuery<IAsyncQuery<string>, string>(query);

			//assert
			action.ShouldThrow<InvalidOperationException>();
		}

		[Theory, AutoMoqData]
		public async Task ProcessCommand_ShouldReturnCorrectValue(
			[Frozen]Mock<IAsyncCommandHandlerFactory> asyncCommandHandlerFactory,
			Mock<IAsyncCommandHandler<IAsyncCommand>> commandHandler,
			IAsyncCommand command,
			string expected,
			AsyncCommandQueryBus sut)
		{
			//arrange
			asyncCommandHandlerFactory.Setup(a => a.Create<IAsyncCommand>()).Returns(commandHandler.Object);
			var verifiable = commandHandler.Setup(q => q.Execute(command)).ReturnsDefaultTaskVerifiable();

			//act
			await sut.ProcessCommand(command);

			//assert
			verifiable.Verify();
		}

		[Theory, AutoMoqData]
		public void ProcessCommand_WhenCommandHandlerIsNotFound_ShouldThrow(
			[Frozen]Mock<IAsyncCommandHandlerFactory> asyncCommandHandlerFactory,
			Mock<IAsyncCommandHandler<IAsyncCommand>> commandHandler,
			IAsyncCommand command,
			string expected,
			AsyncCommandQueryBus sut)
		{
			//arrange
			asyncCommandHandlerFactory.Setup(a => a.Create<IAsyncCommand>())
				.Throws<InvalidOperationException>();

			//act
			Func<Task> action = async () => await sut.ProcessCommand(command);

			//assert
			action.ShouldThrow<InvalidOperationException>();
		}
	}
}
