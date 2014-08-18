using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using FluentAssertions;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.Commands
{
    public class ExistingCommandBuilderTests
    {
        [Theory, AutoMoqData]
        public void Command_ShouldReturnCorrectValue(
            [Frozen] ICommand existingCommand,
            ExistingCommandBuilder sut)
        {
            //assert
            sut.Command.Should().Be(existingCommand);
        }

        [Theory, AutoMoqData]
        public void Execute_WithAction_ShouldReturnCorrectValue(
            [Frozen] ICommand command,
            ExistingCommandBuilder sut)
        {
            //arrange

            //act
            var actual = sut.Execute(() => { });

            //assert
            actual.Should().BeOfType<ExistingCommandBuilderOptions<object, object>>()
                  .And.Match<ExistingCommandBuilderOptions<object, object>>(options => options.Command == command);
        }

        [Theory, AutoMoqData]
        public void Execute_WithActionOfT_ShouldReturnCorrectValue(
            [Frozen] ICommand command,
            ExistingCommandBuilder sut)
        {
            //arrange

            //act
            var actual = sut.Execute<string>(a => { });

            //assert
            actual.Should().BeOfType<ExistingCommandBuilderOptions<string, object>>()
                  .And.Match<ExistingCommandBuilderOptions<string, object>>(options => options.Command == command);
        }

        [Theory, AutoMoqData]
        public void Execute_WithObservable_ShouldReturnCorrectValue(
            [Frozen] ICommand command,
            ExistingCommandBuilder sut)
        {
            //arrange

            //act
	        var actual = sut.Execute((object _) => Observable.Return("a"));

            //assert
            actual.Should().BeOfType<ExistingCommandBuilderOptions<object, string>>()
                  .And.Match<ExistingCommandBuilderOptions<object, string>>(options => options.Command == command);
        }
    }

    public class ExistingCommandBuilderOptionsTests
    {
        [Theory, AutoMoqData]
        public void Command_ShouldReturnCorrectValue(
            [Frozen] ICommand command,
            ExistingCommandBuilderOptions<object, object> sut)
        {
            //assert
            sut.Command.Should().Be(command);
        }

        [Theory, AutoMoqData]
        public void ToCommand_ShouldReturnCorrectValue(
            [Frozen] ICommand command,
            ExistingCommandBuilderOptions<object, object> sut)
        {
            //arrange

            //act
            var actual = sut.ToCommand();

            //assert
            actual.Should().Be(command);
        }

    }
}