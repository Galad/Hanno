using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Testing.Autofixture
{
    #region Customization
    public class RxCompositeCustomization : CompositeCustomization
    {
        public RxCompositeCustomization()
            : base(new HannoCustomization(), new RxCustomization())
        {
        }
    }

    public class RxAutoDataAttribute : AutoDataAttribute
    {
        public RxAutoDataAttribute()
            : base(new Fixture().Customize(new RxCompositeCustomization()))
        {
        }
    }

    public class RxInlineAutoDataAttribute : CompositeDataAttribute
    {
        public RxInlineAutoDataAttribute(params object[] values)
            : base(new RxAutoDataAttribute(), new InlineDataAttribute(values))
        {
        }
    }
    #endregion
}