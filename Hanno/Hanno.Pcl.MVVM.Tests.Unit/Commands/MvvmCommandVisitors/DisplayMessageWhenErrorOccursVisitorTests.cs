using System;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Commands;
using Hanno.Commands.MvvmCommandVisitors;
using Hanno.Globalization;
using Hanno.Services;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;
using FluentAssertions;

namespace Hanno.Tests.Commands.MvvmCommandVisitors
{
	public class DisplayMessageWhenErrorOccursVisitorTests
	{
		[Theory, AutoMoqData]
		public async Task Visit_ShouldCallSetDefaultError(
			[Frozen]Mock<IResources> resources,
			[Frozen]Mock<IAsyncMessageDialog> asyncMessageDialog,
			DisplayMessageWhenErrorOccursVisitor sut,
			Mock<IAsyncMvvmCommand> command,
			string commandName,
			string expectedTitle,
			string expectedContent)
		{
			//arrange
			Func<CancellationToken, Exception, Task> expectedTask = null;
			command.Setup(c => c.Name).Returns(commandName);
			command.Setup(c => c.SetDefaultError(It.IsAny<Func<CancellationToken, Exception, Task>>()))
				.Callback((Func<CancellationToken, Exception, Task> task) => expectedTask = task);
			resources.Setup(r => r.Get(string.Concat(commandName, DisplayMessageWhenErrorOccursVisitor.TitleResourceKeySuffix)))
					 .Returns(expectedTitle);
			resources.Setup(r => r.Get(string.Concat(commandName, DisplayMessageWhenErrorOccursVisitor.ContentResourceKeySuffix)))
					 .Returns(expectedContent);

			//act
			sut.Visit(command.Object);
			await expectedTask(CancellationToken.None, new Exception());

			//assert
			asyncMessageDialog.Verify(dialog => dialog.Show(It.IsAny<CancellationToken>(), expectedTitle, expectedContent));
		}

		[Theory, AutoMoqData]
		public void Visit_ErrorIsNotDefaultAndResourcesMissing_ShouldCallSetDefaultError(
			DisplayMessageWhenErrorOccursVisitor sut,
			Mock<IAsyncMvvmCommand> command,
			string commandName)
		{
			//arrange
			Func<CancellationToken, Exception, Task> expectedTask = null;
			command.Setup(c => c.Name).Returns(commandName);
			command.Setup(c => c.SetDefaultError(It.IsAny<Func<CancellationToken, Exception, Task>>()))
				   .Returns(false);

			//act
			sut.Visit(command.Object);

			//assert
			command.Verify(c => c.SetDefaultError(It.IsAny<Func<CancellationToken, Exception, Task>>()));
		}

		[Theory, AutoMoqData]
		public void Visit_WhenSettingDefaultErrorAndResourcesMissing_ShouldThrow(
            [Frozen]Mock<IResources> resources,
            DisplayMessageWhenErrorOccursVisitor sut,
			Mock<IAsyncMvvmCommand> command,            
			string commandName)
		{
			//arrange
			command.Setup(c => c.Name).Returns(commandName);
			command.Setup(c => c.SetDefaultError(It.IsAny<Func<CancellationToken, Exception, Task>>()))
				   .Returns(true);
            resources.Setup(r => r.Get(It.IsAny<string>())).Returns(string.Empty);

			//act
			Action action = () => sut.Visit(command.Object);

			//assert
			var expectedTitleKey = string.Concat(commandName, DisplayMessageWhenErrorOccursVisitor.TitleResourceKeySuffix);
			var expectedTitleContent = string.Concat(commandName, DisplayMessageWhenErrorOccursVisitor.ContentResourceKeySuffix);
			action.ShouldThrow<InvalidOperationException>()
				  .And.Should().Match((Exception ex) => ex.Message.Contains(expectedTitleKey) && ex.Message.Contains(expectedTitleContent), "Message does not contains missing resource keys");
		}
	}
}