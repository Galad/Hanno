using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hanno.Xaml.Controls
{
	public class RegexSearchResultCommandSelector : ISearchResultCommandSelector
	{
		private readonly Func<string, object> _tagSelector;
		public string CanHandleTagPattern { get; set; }
		public string SelectTagPattern { get; set; }

		public RegexSearchResultCommandSelector(Func<string, object> tagSelector = null)
		{
			if (tagSelector == null)
			{
				tagSelector = s => s;
			}
			_tagSelector = tagSelector;
		}

		public bool CanHandleTag(string tag)
		{
			return Regex.IsMatch(tag, CanHandleTagPattern);
		}

		public object SelectTag(string tag)
		{
			return _tagSelector(Regex.Match(tag, SelectTagPattern).Groups.OfType<Group>().FirstOrDefault(g => g.GetType() != typeof(Match)).Value);
		}
	}
}