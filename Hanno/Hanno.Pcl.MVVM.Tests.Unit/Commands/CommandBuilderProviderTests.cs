using System;
using System.Windows.Input;
using FluentAssertions;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Commands
{
	#region Customization
	public class CommandBuilderProviderCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Register<Func<Action<ICommand>, ISchedulers, string, ICommandBuilder>>(() =>
				((action, schedulers, arg3) => new CommandBuilder(action, schedulers, arg3)));
		}
	}

	public class CommandBuilderProviderAutoDataCompositeCustomization : CompositeCustomization
	{
		public CommandBuilderProviderAutoDataCompositeCustomization()
			: base(new AutoMoqCustomization(), new CommandBuilderProviderCustomization())
		{
		}
	}

	public class CommandBuilderProviderAutoDataAttribute : AutoDataAttribute
	{
		public CommandBuilderProviderAutoDataAttribute()
			: base(new Fixture().Customize(new CommandBuilderProviderAutoDataCompositeCustomization()))
		{
		}
	}

	public class CommandBuilderProviderAutoDataInlineAttribute : CompositeDataAttribute
	{
		public CommandBuilderProviderAutoDataInlineAttribute(params object[] values)
			: base(new InlineDataAttribute(values), new CommandBuilderProviderAutoDataAttribute())
		{
		}
	}
	#endregion

	public class CommandBuilderProviderTests
	{
		[Theory, AutoMoqData]
		public void Get_UsingDefaultBuilderFactory_ShouldReturnCorrectValue(
			string name,
			ISchedulers schedulers)
		{
			//arrange
			var sut = new CommandBuilderProvider(schedulers);

			//act
			var actual = sut.Get(name);

			//assert
			actual.Should().NotBeNull().And.BeOfType<CommandBuilder>();
		}

		[Theory, AutoMoqData]
		public void Get_UsingBuilderFactory_ShouldReturnCorrectValue(
			string name,
			ISchedulers schedulers,
			ICommandBuilder expected)
		{
			//arrange
			var sut = new CommandBuilderProvider(schedulers, (action, schedulers1, arg3) => expected);

			//act
			var actual = sut.Get(name);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, CommandBuilderProviderAutoData]
		public void Get_CalledTwice_ShouldReturnCorrectValue(
			string name,
		  CommandBuilderProvider sut)
		{
			//arrange

			//act
			var expected = sut.Get(name).Execute(() => { }).ToCommand();
			var actual = sut.Get(name).Execute(() => { }).ToCommand();

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public void Get_ShouldCallAccept(
			string name,
			ISchedulers schedulers,
			Mock<ICommandBuilder> commandBuilder,
			Mock<IMvvmCommand> command,
			IMvvmCommandVisitor visitor)
		{
			//arrange
			Action<ICommand> action = null;
			var sut = new CommandBuilderProvider(schedulers, (action1, schedulers1, n) =>
			{
				action = action1;
				return commandBuilder.Object;
			});

			sut.AddVisitor(visitor);

			//act
			sut.Get("");
			action(command.As<ICommand>().Object);

			//assert
			command.Verify(c => c.Accept(visitor));
		}


		[Theory, AutoMoqData]
		public void CopyVisitors_ShouldCallAddVisitor(
		  CommandBuilderProvider sut,
		  IMvvmCommandVisitor[] visitors,
		  Mock<ICommandBuilderProvider> other)
		{
			//arrange
			foreach (var mvvmCommandVisitor in visitors)
			{
				sut.AddVisitor(mvvmCommandVisitor);
				other.Setup(c => c.AddVisitor(mvvmCommandVisitor)).Verifiable();
			}

			//act
			sut.CopyVisitors(other.Object);

			//assert
			other.Verify();
		}

		
	}
}