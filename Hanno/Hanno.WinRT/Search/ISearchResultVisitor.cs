namespace Hanno.Search
{
	public interface ISearchResultVisitor
	{
		void Visit(SearchQuerySuggestion suggestion);
		void Visit(SearchResultSuggestion suggestion);
		void Visit(SearchSeparator suggestion);
	}
}