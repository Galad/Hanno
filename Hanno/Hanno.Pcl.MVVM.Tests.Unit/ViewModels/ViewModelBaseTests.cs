using System;
using System.Linq;
using FluentAssertions;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Tests.ViewModels
{
	#region Customization
	public class ViewModelBaseCustomization : ICustomization
	{
		public void Customize(IFixture fixture)
		{
			fixture.Behaviors.Remove(fixture.Behaviors.First(transformation => transformation is ThrowingRecursionBehavior));
			fixture.Behaviors.Add(new OmitOnRecursionBehavior());
			fixture.Customize<Mock<ViewModelBase>>(d => d.Do(vm => vm.Object.Services = fixture.Create<IViewModelServices>()));
			fixture.Customize<Mock<IViewModelServices>>(d => d.Do(m => m.Setup(mm => mm.Schedulers).Returns(fixture.Create<ISchedulers>())));
			//fixture.Customize<TestViewModel>(d => d.Do(vm => vm.Services = fixture.Create<IViewModelServices>()));
		}
	}

	public class ViewModelBaseCompositeCustomization : CompositeCustomization
	{
		public ViewModelBaseCompositeCustomization()
			: base(new AutoMoqCustomization(), new ViewModelBaseCustomization())
		{
		}
	}

	public class ViewModelBaseAutoDataAttribute : AutoDataAttribute
	{
		public ViewModelBaseAutoDataAttribute()
			: base(new Fixture().Customize(new ViewModelBaseCompositeCustomization()))
		{
		}
	}

	public class ViewModelBaseInlineAutoDataAttribute : CompositeDataAttribute
	{
		public ViewModelBaseInlineAutoDataAttribute(params object[] values)
			: base(new InlineDataAttribute(values), new ViewModelBaseAutoDataAttribute())
		{
		}
	}
	#endregion

	public class TestViewModel : ViewModelBase
	{
		public ViewModelBase CallRegisterChild(ViewModelBase viewModel)
		{
			return RegisterChild(viewModel);
		}
	}

	public class ViewModelBaseTests
	{
		[Theory, ViewModelBaseInlineAutoData(typeof(GuardClauseAssertion)), ViewModelBaseInlineAutoData(typeof(WritablePropertyAssertion))]
		public void VerifyProperties(
			Type assertionType,
			TestViewModel sut,
			Fixture fixture)
		{
			//arrange
			var assertion = (IIdiomaticAssertion)fixture.Create(assertionType, new SpecimenContext(fixture));

			//act
			assertion.VerifyProperties(() => sut.OvmBuilderProvider, () => sut.CommandBuilderProvider);

			//assert
		}

		[Theory, ViewModelBaseAutoData]
		public void RegisterChild_ShouldRegisterChild(
			TestViewModel sut,
			ViewModelBase child)
		{
			//arrange

			//act
			sut.CallRegisterChild(child);

			//assert
			sut.ViewModels.Should().Contain(child);
		}

		[Theory, ViewModelBaseAutoData]
		public void Dispose_ShouldDisposeChildren(
			TestViewModel sut,
			Mock<ViewModelBase> child)
		{
			//arrange
			sut.CallRegisterChild(child.Object);

			//act
			sut.Dispose();

			//assert
			child.Verify(@base => @base.Dispose());
		}

		[Theory, ViewModelBaseAutoData]
		public void RegisterChild_ShouldReturnViewModel(
		 TestViewModel sut,
			Mock<ViewModelBase> child)
		{
			//arrange

			//act
			var actual = sut.CallRegisterChild(child.Object);

			//assert
			actual.Should().Be(child.Object);
		}
	}
}