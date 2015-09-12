using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Xunit;

namespace Hanno.Tests.ViewModels
{
	public class ObservableViewModelBuilderTests
	{
		[Theory, AutoMoqData]
		public void Execute_ShouldReturnCorrectValue(
			Task<object> value,
			ObservableViewModelBuilder sut)
		{
			//arrange

			//act
			var actual = sut.Execute(ct => value);

			//assert
			actual.Should().BeOfType<ObservableViewModelBuilderOptions<object>>()
				  .And.Match<ObservableViewModelBuilderOptions<object>>(options => options.Source(CancellationToken.None) == value);
		}

		[Theory, AutoMoqData]
		public void ExecuteUpdatable_WithWitness_ShouldReturnCorrectValue(
			Task<object[]> value,
			ObservableViewModelBuilder sut)
		{
			//arrange

			//act
			var actual = sut.ExecuteUpdatable(ct => value, default(object));

			//assert
			actual.Should().BeOfType<UpdatableObservableViewModelBuilderOptions<object, object[]>>()
				  .And.Match<UpdatableObservableViewModelBuilderOptions<object, object[]>>(options => options.Source(CancellationToken.None) == value);
		}

		[Theory, AutoMoqData]
		public void ExecuteUpdatable_ShouldReturnCorrectValue(
			Task<object[]> value,
			ObservableViewModelBuilder sut)
		{
			//arrange

			//act
			var actual = sut.ExecuteUpdatable<object, object[]>(ct => value);

			//assert
			actual.Should().BeOfType<UpdatableObservableViewModelBuilderOptions<object, object[]>>()
				  .And.Match<UpdatableObservableViewModelBuilderOptions<object, object[]>>(options => options.Source(CancellationToken.None) == value);
		}
	}
}