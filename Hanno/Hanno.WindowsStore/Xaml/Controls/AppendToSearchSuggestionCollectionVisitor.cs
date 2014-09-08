using System;
using Windows.ApplicationModel.Search;
using Hanno.Search;

namespace Hanno.WindowsStore.Xaml.Controls
{
	public sealed class AppendToSearchSuggestionCollectionVisitor : ISearchResultVisitor
	{
		private readonly SearchSuggestionCollection _collection;

		public AppendToSearchSuggestionCollectionVisitor(SearchSuggestionCollection collection)
		{
			if (collection == null) throw new ArgumentNullException("collection");
			_collection = collection;
		}

		public void Visit(SearchQuerySuggestion suggestion)
		{
			_collection.AppendQuerySuggestion(suggestion.Suggestion);
		}

		public void Visit(SearchResultSuggestion suggestion)
		{
			_collection.AppendResultSuggestion(suggestion.Text, suggestion.DetailText, suggestion.Tag, suggestion.Image, suggestion.ImageAlternateText);
		}

		public void Visit(SearchSeparator suggestion)
		{
			_collection.AppendSearchSeparator(suggestion.Label);
		}
	}
}