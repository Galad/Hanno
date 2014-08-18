using System.Collections.Generic;
using Hanno.Commands;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;
using FluentAssertions;

namespace Hanno.Tests.ViewModels
{
	public class AddMvvmCommandVisitorsViewModelFactoryTests
	{
		[Theory, AutoMoqData]
		public void Sut_IsViewModelFactory(
		  AddMvvmCommandVisitorsViewModelFactory sut)
		{
			sut.Should().BeAssignableTo<IViewModelFactory>();
		}

		[Theory, AutoMoqData]
		public void Sut_TestConstructorGuadClauses(
			GuardClauseAssertion assertion)
		{
			assertion.VerifyConstructor<AddMvvmCommandVisitorsViewModelFactory>();
		}

		[Theory, AutoMoqData]
		public void ResolveViewModel_ShouldReturnCorrectValue(
			[Frozen]Mock<IViewModelFactory> innerFactory,
			AddMvvmCommandVisitorsViewModelFactory sut,
			IViewModel expected,
			object request)
		{
			//arrange
			innerFactory.Setup(f => f.ResolveViewModel(request)).Returns(expected);

			//act
			var actual = sut.ResolveViewModel(request);

			//assert
			actual.Should().Be(expected);
		}

		[Theory, AutoMoqData]
		public void ResolveViewModel_ShouldAddVisitors_ShouldReturnCorrectValue(
			[Frozen]Mock<IViewModelFactory> innerFactory,
			[Frozen]IEnumerable<IMvvmCommandVisitor> visitors,
			AddMvvmCommandVisitorsViewModelFactory sut,
			Mock<ViewModelBase> viewModel,
			Mock<ICommandBuilderProvider> commandBuilderProvider,
			object request)
		{
			//arrange
			innerFactory.Setup(f => f.ResolveViewModel(request)).Returns(() => viewModel.Object);
			viewModel.Setup(vm => vm.CommandBuilderProvider).Returns(() => commandBuilderProvider.Object);
			foreach (var mvvmCommandVisitor in visitors)
			{
				var visitor = mvvmCommandVisitor;
				commandBuilderProvider.Setup(provider => provider.AddVisitor(visitor))
									  .Verifiable();
			}

			//act
			sut.ResolveViewModel(request);

			//assert
			commandBuilderProvider.Verify();
		}

		[Theory, AutoMoqData]
		public void ReleaseViewModel_ShouldReturnCorrectValue(
			[Frozen]Mock<IViewModelFactory> innerFactory,
			AddMvvmCommandVisitorsViewModelFactory sut,
			IViewModel expected)
		{
			//arrange		

			//act
			sut.ReleaseViewModel(expected);

			//assert
			innerFactory.Verify(f => f.ReleaseViewModel(expected));
		}
	}
}