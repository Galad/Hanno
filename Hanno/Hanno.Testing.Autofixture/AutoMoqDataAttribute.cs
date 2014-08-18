using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Hanno.Testing.Autofixture
{
    public class AutoMoqDataAttribute:AutoDataAttribute
    {
        public AutoMoqDataAttribute():base(new Fixture().Customize(new AutoMoqCustomization()).Customize(new AsyncMoqCustomization()))
        {
        }
    }

	public class InlineAutoMoqDataAttribute : CompositeDataAttribute
	{
		public InlineAutoMoqDataAttribute(params object[] values):base(new InlineDataAttribute(values), new AutoMoqDataAttribute())
		{
			
		}
	}
}