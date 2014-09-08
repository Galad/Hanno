using System;

namespace Hanno.Search
{
	public class SearchSeparator : ISearchSuggestion
	{
		public string Label { get; private set; }

		public SearchSeparator(string label)
		{
			if (string.IsNullOrEmpty(label)) throw new ArgumentNullException("label");
			Label = label;
		}

		public void Accept(ISearchResultVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}