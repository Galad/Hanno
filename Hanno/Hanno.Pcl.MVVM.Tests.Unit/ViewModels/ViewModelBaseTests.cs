using System;
using System.Linq;
using FluentAssertions;
using Hanno.Navigation;
using Hanno.Testing.Autofixture;
using Hanno.ViewModels;
using Moq;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;
using Ploeh.AutoFixture.Kernel;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Tests.ViewModels
{
    #region Customization
    public class ViewModelBaseCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Behaviors.Remove(fixture.Behaviors.First(transformation => transformation is ThrowingRecursionBehavior));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            //fixture.Customize<Mock<IViewModelServices>>(d => d.Do(m => m.Setup(mm => mm.Schedulers).Returns(fixture.Create<ISchedulers>())));
        }
    }

    public class ViewModelBaseCompositeCustomization : CompositeCustomization
    {
        public ViewModelBaseCompositeCustomization()
            : base(new HannoCustomization(), new ViewModelBaseCustomization())
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
        public TestViewModel(IViewModelServices services)
            : base(services)
        {
        }

        public ViewModelBase CallRegisterChild(ViewModelBase viewModel)
        {
            return RegisterChild(viewModel);
        }
    }

    public class ViewModelBaseTests
    {
        [Theory, InlineData(typeof(GuardClauseAssertion)), InlineData(typeof(WritablePropertyAssertion))]
        public void VerifyProperties(
            Type assertionType
            )
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var sut = fixture.Create<TestViewModel>();
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

        [Theory, ViewModelBaseAutoData]
        public void Initialize_ShouldCallChildrenInitialize(
         TestViewModel sut,
            Mock<ViewModelBase> child,
            INavigationRequest request)
        {
            //arrange
            child.As<IViewModel>();
            sut.CallRegisterChild(child.Object);

            //act
            sut.Initialize(request);

            //assert
            child.As<IViewModel>().Verify(c => c.Initialize(request), Times.Once());
        }

        [Theory, ViewModelBaseAutoData]
        public void Register_WhenAlreadyInitialized_ShouldCallChildrenInitialize(
         TestViewModel sut,
            Mock<ViewModelBase> child,
            INavigationRequest request)
        {
            //arrange
            child.As<IViewModel>();
            sut.Initialize(request);

            //act
            sut.CallRegisterChild(child.Object);

            //assert
            child.As<IViewModel>().Verify(c => c.Initialize(request), Times.Once());
        }

        [Theory, ViewModelBaseAutoData]
        public void Initialize_GuardClauses(
            TestViewModel sut,
         GuardClauseAssertion assertion)
        {
            assertion.Verify((INavigationRequest request) => sut.Initialize(request));
        }
    }
}