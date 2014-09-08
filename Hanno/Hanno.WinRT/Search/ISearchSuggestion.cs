namespace Hanno.Search
{
	public interface ISearchSuggestion
	{
		void Accept(ISearchResultVisitor visitor);
	}
}