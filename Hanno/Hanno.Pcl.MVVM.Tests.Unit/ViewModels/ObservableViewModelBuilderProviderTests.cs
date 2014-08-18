using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Ploeh.AutoFixture.Idioms;
using Xunit.Extensions;

namespace Hanno.Tests.ViewModels
{
    public class ObservableViewModelBuilderProviderTests
    {
        [Theory, AutoMoqData]
        public void Get_TestGuardClauses(
            GuardClauseAssertion assertion,
          ObservableViewModelBuilderProvider sut)
        {
            assertion.Verify(() => sut.Get(""));
        }

        [Theory, AutoMoqData]
        public void Get_GettingASingleBuilder_ShouldReturnCorrectValue(
		  ObservableViewModelBuilderProvider sut,
            string name)
        {
            //arrange

            //act
            var actual = sut.Get(name);

            //assert
			actual.Should().BeOfType<ObservableViewModelBuilder>();
        }

        [Theory, AutoMoqData]
        public void Get_GettingABuilderTwice_ShouldReturnCorrectValue(
		  ObservableViewModelBuilderProvider sut,
            string name)
        {
            //arrange

            //act
            var vm = sut.Get(name).Execute(ct => Task.FromResult(new object())).ToViewModel();
            var second = sut.Get(name);

            //assert
            second.Should().BeOfType<ExistingObservableViewModelBuilder>()
                  .And.Match<ExistingObservableViewModelBuilder>(s => s.ViewModel == (IObservableViewModel)vm);
        }
    }
}