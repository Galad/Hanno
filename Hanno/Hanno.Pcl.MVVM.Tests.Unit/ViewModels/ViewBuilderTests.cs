namespace Hanno.Tests.ViewModels
{
    //public class TestViewModel : IViewModel
    //{
    //    public void Load(bool firstLoad)
    //    {
    //    }
    //}

    //public class TestViewBuilder : ViewBuilder
    //{
    //    protected override object GetView(Type viewType)
    //    {
    //        return null;
    //    }

    //    public IEnumerable<KeyValuePair<Type, Type>> Definitions
    //    {
    //        get { return ViewDefinitions; }
    //    }
    //}

    //public class View1 { }
    //public class View2 : View1 { }

    //#region Customization
    //public class ViewBuilderCustomization : ICustomization
    //{
    //    public void Customize(IFixture fixture)
    //    {
    //        fixture.Register<ViewBuilder>(() => new TestViewBuilder());
    //    }
    //}

    //public class ViewBuilderCompositeCustomization : CompositeCustomization
    //{
    //    public ViewBuilderCompositeCustomization()
    //        : base(new AutoMoqCustomization(), new ViewBuilderCustomization())
    //    {
    //    }
    //}

    //public class ViewBuilderAutoDataAttribute : AutoDataAttribute
    //{
    //    public ViewBuilderAutoDataAttribute()
    //        : base(new Fixture().Customize(new ViewBuilderCompositeCustomization()))
    //    {
    //    }
    //}

    //public class ViewBuilderInlineAutoDataAttribute : CompositeDataAttribute
    //{
    //    public ViewBuilderInlineAutoDataAttribute(params object[] values)
    //        : base(new ViewBuilderAutoDataAttribute(), new InlineDataAttribute(values))
    //    {
    //    }
    //}
    //#endregion

    //public class ViewBuilderTests
    //{
    //    [Theory, ViewBuilderAutoData]
    //    public void RegisterViews_ShouldAddViewsToDefinitions()
    //    {
    //        var sut = new TestViewBuilder();
    //        ViewBuilder.Current = sut;
    //        var views = new[] { typeof(object), typeof(string) };
    //        var viewModelType = typeof(TestViewModel);

    //        ViewBuilder.Current.RegisterViews(views, viewModelType);

    //        var expected = views.Select(v => new KeyValuePair<Type, Type>(v, viewModelType)).ToList();
    //        sut.Definitions.ShouldAllBeEquivalentTo(expected);
    //    }

    //    [Theory, ViewBuilderAutoData]
    //    public void RegisterViews_RegisteringViewsTwice_ShouldThrow()
    //    {
    //        var sut = new TestViewBuilder();
    //        ViewBuilder.Current = sut;
    //        var views = new[] { typeof(object), typeof(string) };
    //        var viewModelType = typeof(TestViewModel);

    //        ViewBuilder.Current.RegisterViews(views, viewModelType);
    //        Action action = () => ViewBuilder.Current.RegisterViews(views, viewModelType);

    //        action.ShouldThrow<ArgumentException>();
    //    }

    //    [Theory, ViewBuilderAutoData]
    //    public void Current_ShouldReturnCurrentValue()
    //    {
    //        //arrange

    //        //act
    //        var actual = ViewBuilder.Current;

    //        //assert
    //        actual.Should().NotBeNull().And.BeOfType<DefaultViewBuilder>();
    //    }

    //    [Theory, ViewBuilderAutoData]
    //    public void Current_AssigningNewCurrent_ShouldSetValue(ViewBuilder expected)
    //    {
    //        //arrange

    //        //act
    //        ViewBuilder.Current = expected;
    //        var actual = ViewBuilder.Current;

    //        //assert
    //        actual.Should().Be(expected);
    //    }

    //    [Theory, ViewBuilderAutoData]
    //    public void ViewModelFactory_ShouldReturnCorrectValue(ViewBuilder sut)
    //    {
    //        //arrange

    //        //act
    //        var actual = sut.ViewModelFactory;

    //        //assert
    //        actual.Should().NotBeNull().And.BeOfType<DefaultViewModelFactory>();
    //    }

    //    [Theory, ViewBuilderAutoData]
    //    public void SetViewModelFactory_ShouldSetValue(
    //        ViewBuilder sut,
    //        WritablePropertyAssertion assertion)
    //    {
    //        //act
    //        assertion.VerifyProperties(() => sut.ViewModelFactory);
    //    }

    //    [Theory, ViewBuilderAutoData]
    //    public void ViewModelFactory_GuardClause(ViewBuilder sut, Fixture fixture)
    //    {
    //        //act
    //        var assertion = new GuardClauseAssertion(fixture, new ExceptionBehaviorExpectation<InvalidOperationException>());
    //        assertion.VerifyProperties(() => sut.ViewModelFactory);
    //    }
    //}

}