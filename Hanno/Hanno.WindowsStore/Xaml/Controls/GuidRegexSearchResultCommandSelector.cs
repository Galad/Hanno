using System;

namespace Hanno.Xaml.Controls
{
	public class GuidRegexSearchResultCommandSelector : RegexSearchResultCommandSelector
	{
		public GuidRegexSearchResultCommandSelector()
			: base(s => Guid.Parse(s))
		{
		}
	}
}