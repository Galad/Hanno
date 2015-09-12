using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit2;
using Xunit;

namespace Hanno.Testing.Autofixture
{
    public class HannoCustomization : CompositeCustomization
    {
        public HannoCustomization() :
            base(            
            new HannoAutoConfiguredMoqCustomization(),
            new IoCustomization())
        {
        }
    }

    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(new Fixture().Customize(new HannoCustomization()))
        {
        }
    }

    public class InlineAutoMoqDataAttribute : CompositeDataAttribute
    {
        public InlineAutoMoqDataAttribute(params object[] values) : base(new InlineDataAttribute(values), new AutoMoqDataAttribute())
        {
        }
    }
}