using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Moq;
using Moq.Protected;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class DelegateCommandHandlerTests
	{
		public class TestCommand : AsyncCommandBase
		{
			public TestCommand(CancellationToken ct) : base(ct)
			{
			}
		}

		public class TestCommandHandler : DelegateCommandHandler<TestCommand>
		{
			protected override void ExecuteOverride(TestCommand command)
			{
				throw new System.NotImplementedException();
			}
		}

		[Theory, AutoData]
		public async Task Execute_ShouldCallExecuteOverride(
			Mock<DelegateCommandHandler<TestCommand>> sut,
			TestCommand command,
			object expected)
		{
			//arrange

			//act
			await sut.Object.Execute(command);

			//assert
			sut.Protected().Verify("ExecuteOverride", Times.Once(), command);
		}

		[Theory, AutoMoqData]
		public void Sut_ShouldBeAsyncCommandHandler(
			DelegateCommandHandler<TestCommand> sut,
			TestCommand command,
			object expected)
		{
			sut.Should().BeAssignableTo<IAsyncCommandHandler<TestCommand>>();
		}
	}
}