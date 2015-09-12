using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.ViewModels
{
    public class ExistingObservableViewModelBuilderOptionsTests
    {
        [Theory, AutoMoqData]
        public void ToViewModel_ShouldReturnCorrectValue(
            [Frozen]Mock<IObservableViewModel> viewModel
            )
        {
            //arrange
			viewModel.As<IObservableViewModel<object>>();
	        var sut = new ExistingObservableViewModelBuilderOptions<object>(viewModel.Object);

            //act
            var actual = sut.ToViewModel();

            //assert
            actual.Should().Be(viewModel.Object);
        }
    }
}