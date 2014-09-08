namespace Hanno.Search
{
	public class SearchQuerySuggestion : ISearchSuggestion
	{
		public SearchQuerySuggestion(string suggestion)
		{
			Suggestion = suggestion;
		}

		public string Suggestion { get; private set; }


		public void Accept(ISearchResultVisitor visitor)
		{
			visitor.Visit(this);
		}
	}
}