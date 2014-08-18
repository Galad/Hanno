using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;
using FluentAssertions;
using System.Threading.Tasks;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class CompositeCommandHandlerTests
	{
		public class TestCommand : IAsyncCommand
		{
			public CancellationToken CancellationToken { get; private set; }
		}

		[Theory, AutoMoqData]
		public async Task Execute_ShouldCallInnerCommandHandlers(
			IEnumerable<Mock<IAsyncCommandHandler<TestCommand>>> commandHandlers,
			TestCommand command)
		{
			//arrange
			var sut = new CompositeCommandHandler<TestCommand>(commandHandlers.Select(c => c.Object));

			//act
			await sut.Execute(command);

			//assert
			foreach (var commandHandler in commandHandlers)
			{
				commandHandler.Verify(c => c.Execute(command), Times.Once());
			}
		}

		[Theory, AutoMoqData]
		public void Constructor_GuardClauses(
			GuardClauseAssertion assertions)
		{
			assertions.VerifyConstructor<CompositeCommandHandler<TestCommand>>();
		}
	}
}