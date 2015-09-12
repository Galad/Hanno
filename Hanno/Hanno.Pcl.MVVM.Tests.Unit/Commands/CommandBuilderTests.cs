using System;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.Commands
{
    public class CommandBuilderTests
    {
		[Theory, AutoMoqData]
        public void Execute_WithAction_ShouldReturnCorrectValue(
            CommandBuilder sut)
        {
            //arrange
            bool isActionCalled = false;
            Action action = () => isActionCalled = true;

            //act
            var actual = sut.Execute(action);

            //assert

            actual.Should().BeOfType<CommandBuilderOptions<object>>();
            actual.As<CommandBuilderOptions<object>>().Action(null);
            isActionCalled.Should().BeTrue();
        }

        [Theory, AutoMoqData]
        public void Execute_WithActionOfT_ShouldReturnCorrectValue(
            CommandBuilder sut,
            Action<string> action)
        {
            //arrange

            //act
            var actual = sut.Execute(action);

            //assert

            actual.Should().BeOfType<CommandBuilderOptions<string>>()
                  .And.Match<CommandBuilderOptions<string>>(options => options.Action == action);
        }

        [Theory, AutoMoqData]
        public void Execute_WithObservable_ShouldReturnCorrectValue(
            CommandBuilder sut,
            IObservable<string> observable)
        {
            //arrange

            //act
            var actual = sut.Execute((object _) => observable);

            //assert

            actual.Should().BeOfType<ObservableCommandBuilderOptions<object, string>>()
                  .And.Match<ObservableCommandBuilderOptions<object, string>>(options => options.Observable(null) == observable);
        }
    }
}