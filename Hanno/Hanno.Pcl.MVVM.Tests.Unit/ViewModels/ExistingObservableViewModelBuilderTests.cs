using System.Threading.Tasks;
using FluentAssertions;
using Hanno.ViewModels;
using Moq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.ViewModels
{
	public class ExistingObservableViewModelBuilderTests
	{
		[Theory, AutoData]
		public void Execute_ShouldReturnCorrectValue(Mock<IObservableViewModel> expectedViewModel)
		{
			//arrange
			var sut = new ExistingObservableViewModelBuilder(expectedViewModel.Object);

			//act
			var actual = sut.Execute(ct => Task.FromResult(new object()));

			//assert
			actual.Should().BeOfType<ExistingObservableViewModelBuilderOptions<object>>()
				  .And.Match<ExistingObservableViewModelBuilderOptions<object>>(options => options.ViewModel == expectedViewModel.Object);
		}

		[Theory, AutoData]
		public void ExecuteUpdatable_ShouldReturnCorrectValue(Mock<IObservableViewModel> expectedViewModel)
		{
			//arrange
			var sut = new ExistingObservableViewModelBuilder(expectedViewModel.Object);

			//act
			var actual = sut.ExecuteUpdatable(ct => Task.FromResult(new object[0]), default(object));

			//assert
			actual.Should().BeOfType<ExistingUpdatableObservableViewModelBuilderOptionsUpdateOn<object>>()
				  .And.Match<ExistingUpdatableObservableViewModelBuilderOptionsUpdateOn<object>>(options => options.ViewModel == expectedViewModel.Object);
		}
	}
}