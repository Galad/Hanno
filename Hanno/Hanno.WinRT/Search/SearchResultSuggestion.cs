using System;
using Windows.Storage.Streams;

namespace Hanno.Search
{
	public class SearchResultSuggestion : ISearchSuggestion
	{
		public string Text { get; set; }
		public string DetailText { get; set; }
		public string Tag { get; set; }
		public IRandomAccessStreamReference Image { get; set; }
		public string ImageAlternateText { get; set; }

		public SearchResultSuggestion(string text, string detailText, string tag, IRandomAccessStreamReference image, string imageAlternateText)
		{
			if (text == null) throw new ArgumentNullException("text");
			Text = text;
			DetailText = detailText;
			Tag = tag;
			Image = image;
			ImageAlternateText = imageAlternateText;
		}

		public void Accept(ISearchResultVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}