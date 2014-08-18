using System;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;
using FluentAssertions;

namespace Hanno.Tests.CqrsInfrastructure
{
	public class NotifyEventsAsyncCommandBusTests
	{
		[Theory, AutoMoqData]
		public async Task ProcessCommand_ShouldCallNotifyComplete(
			[Frozen]Mock<ICommandEvents> commandEvents,
			[Frozen]Mock<IAsyncCommandBus> commandBus,
			NotifyEventsAsyncCommandBus sut,
			Mock<IAsyncCommand> command)
		{
			//arrange
			command.As<IAsyncCommandEvent<string>>();
			commandBus.Setup(c => c.ProcessCommand(command.Object)).Returns(Task.FromResult(true));

			//act
			await sut.ProcessCommand(command.Object);

			//assert
			commandEvents.Verify(c => c.NotifyComplete(command.As<IAsyncCommandEvent<string>>().Object));
		}

		[Theory, AutoMoqData]
		public async Task ProcessCommand_WhenErrorOccurrs_ShouldCallNotifyError(
			[Frozen]Mock<ICommandEvents> commandEvents,
			[Frozen]Mock<IAsyncCommandBus> commandBus,
			NotifyEventsAsyncCommandBus sut,
			Mock<IAsyncCommand> command,
			Exception ex)
		{
			//arrange
			command.As<IAsyncCommandEvent<string>>();
			commandBus.Setup(c => c.ProcessCommand(command.Object)).Returns(() => Task.Run(() => { throw ex; }));

			//act
			try
			{
				await sut.ProcessCommand(command.Object);
			}
			catch (Exception)
			{
				//we don't want to know about exception in this test
			}

			//assert
			commandEvents.Verify(c => c.NotifyError(command.As<IAsyncCommandEvent<string>>().Object, ex));
		}

		[Theory, AutoMoqData]
		public void ProcessCommand_WhenErrorOccurrs_ShouldThrow(
			[Frozen]Mock<ICommandEvents> commandEvents,
			[Frozen]Mock<IAsyncCommandBus> commandBus,
			NotifyEventsAsyncCommandBus sut,
			Mock<IAsyncCommand> command,
			Exception ex)
		{
			//arrange
			command.As<IAsyncCommandEvent<string>>();
			commandBus.Setup(c => c.ProcessCommand(command.Object)).Returns(() => Task.Run(() => { throw ex; }));

			//act
			Func<Task> task = () =>  sut.ProcessCommand(command.Object);

			//assert
			task.ShouldThrow<Exception>()
			    .And.Should().Be(ex, "Exception was not the exception originally thrown");
		}

		[Theory, AutoMoqData]
		public async Task ProcessCommand_CallProcessCommand(
			[Frozen]Mock<ICommandEvents> commandEvents,
			[Frozen]Mock<IAsyncCommandBus> commandBus,
			NotifyEventsAsyncCommandBus sut,
			IAsyncCommand command)
		{
			//arrange
			commandBus.Setup(c => c.ProcessCommand(command)).Returns(Task.FromResult(true));

			//act
			await sut.ProcessCommand(command);

			//assert
			commandBus.Verify(c => c.ProcessCommand(command), Times.Once());
		}
	}
}