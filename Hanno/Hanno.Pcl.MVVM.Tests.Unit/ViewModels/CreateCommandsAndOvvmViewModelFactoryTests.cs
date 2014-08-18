using System.Windows.Input;
using Hanno.Diagnostics;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Moq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;
using FluentAssertions;

namespace Hanno.Tests.ViewModels
{
	public class CreateCommandsAndOvvmViewModelFactoryTests
	{
		public abstract class ViewModelWithCommands2 : ViewModelBase
		{
			public abstract ICommand Command1 { get; set; }
			public abstract ICommand Command2 { get; set; }
		}

		//make the class and the properties abstract in order to be able to make assertions with a mock
		public abstract class ViewModelWithCommands : ViewModelBase
		{
			public abstract ICommand Command1 { get; set; }
			public abstract ICommand Command2 { get; set; }
			public abstract IObservableProperty<string> ObservableProperty1 { get; set; }
			public abstract IObservableProperty<int> ObservableProperty2 { get; set; }
			public abstract IObservableViewModel ObservableViewModel1 { get; set; }
			public abstract IObservableViewModel ObservableViewModel2 { get; set; }
			public abstract ViewModelWithCommands2 ChildViewModel { get; set; }
		}

		[Theory, AutoMoqData]
		public void ResolveViewModel_ShouldReturnCorrectValue(
			[Frozen]Mock<IViewModelFactory> viewModelFactory,
			  CreateCommandsAndOvvmViewModelFactory sut,
			  object request,
			  IViewModel expected)
		{
			//arrange
			viewModelFactory.Setup(v => v.ResolveViewModel(request)).Returns(expected);

			//act
			var actual = sut.ResolveViewModel(request);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public void ReleaseViewModel_ShouldCallReleaseViewModel(
			[Frozen]Mock<IViewModelFactory> viewModelFactory,
			  CreateCommandsAndOvvmViewModelFactory sut,
			  IViewModel expected)
		{
			//arrange

			//act
			sut.ReleaseViewModel(expected);

			//assert
			viewModelFactory.Verify(v => v.ReleaseViewModel(expected));
		}

		[Theory, AutoMoqData]
		public void ResolveViewModel_WhenViewModelHasCommands_ShouldCallAllPropertyGetters(
			[Frozen]Mock<IViewModelFactory> viewModelFactory,
			  CreateCommandsAndOvvmViewModelFactory sut,
			  object request,
			  Mock<ViewModelWithCommands> viewModel,
			  Mock<ViewModelWithCommands2> childViewModel)
		{
			//arrange
			viewModelFactory.Setup(v => v.ResolveViewModel(request)).Returns(viewModel.Object);
			viewModel.Setup(v => v.ChildViewModel).Returns(childViewModel.Object);

			//act
			sut.ResolveViewModel(request);

			//assert
			childViewModel.VerifyGet(e => e.Command1, Times.Once());
			childViewModel.VerifyGet(e => e.Command2, Times.Once());
		}

		[Theory, AutoMoqData]
		public void ResolveViewModel_WhenViewModelHasObservableProperties_ShouldCallAllPropertyGetters(
			[Frozen]Mock<IViewModelFactory> viewModelFactory,
			  CreateCommandsAndOvvmViewModelFactory sut,
			  object request,
			  Mock<ViewModelWithCommands> expected)
		{
			//arrange
			viewModelFactory.Setup(v => v.ResolveViewModel(request)).Returns(expected.Object);

			//act
			sut.ResolveViewModel(request);

			//assert
			expected.VerifyGet(e => e.ObservableProperty1, Times.Once());
			expected.VerifyGet(e => e.ObservableProperty2, Times.Once());
		}

		[Theory, AutoMoqData]
		public void ResolveViewModel_WhenViewModelHasObservableViewModel_ShouldCallAllPropertyGetters(
			[Frozen]Mock<IViewModelFactory> viewModelFactory,
			  CreateCommandsAndOvvmViewModelFactory sut,
			  object request,
			  Mock<ViewModelWithCommands> expected)
		{
			//arrange
			viewModelFactory.Setup(v => v.ResolveViewModel(request)).Returns(expected.Object);

			//act
			sut.ResolveViewModel(request);

			//assert
			expected.VerifyGet(e => e.ObservableViewModel1, Times.Once());
			expected.VerifyGet(e => e.ObservableViewModel2, Times.Once());
		}

		[Theory, AutoMoqData]
		public void ResolveViewModel_WhenViewModelHasChild_ShouldCallAllChildPropertyGetters(
		[Frozen]Mock<IViewModelFactory> viewModelFactory,
		  CreateCommandsAndOvvmViewModelFactory sut,
		  object request,
		  Mock<ViewModelWithCommands> expected)
		{
			//arrange
			viewModelFactory.Setup(v => v.ResolveViewModel(request)).Returns(expected.Object);

			//act
			sut.ResolveViewModel(request);

			//assert
			expected.VerifyGet(e => e.ObservableViewModel1, Times.Once());
			expected.VerifyGet(e => e.ObservableViewModel2, Times.Once());
		}

		[Theory, AutoMoqData]
		public void ResolveViewModel_WhenCalled2Times_ShouldBeFasterTheSecondTime(
			[Frozen]Mock<IViewModelFactory> viewModelFactory,
			  CreateCommandsAndOvvmViewModelFactory sut,
			  object request,
			  Mock<ViewModelWithCommands> expected)
		{
			//arrange
			viewModelFactory.Setup(v => v.ResolveViewModel(request)).Returns(expected.Object);

			//act
			var firstCallTime = PerformanceTime.Measure(() => sut.ResolveViewModel(request)).DebugWriteline("First call : {0}");
			var secondCallTime = PerformanceTime.Measure(() => sut.ResolveViewModel(request)).DebugWriteline("Second call : {0}");

			//assert
			secondCallTime.Should().BeLessThan(firstCallTime);
		}
	}
}