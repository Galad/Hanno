using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Hanno.Search;

namespace TestUniversalApp
{
	public class SearchResultProvider : ISearchSuggestionProvider
	{
		public async Task<IList<ISearchSuggestion>> GetSuggestions(CancellationToken ct, string query)
		{
			var icon = RandomAccessStreamReference.CreateFromUri(new Uri("http://upload.wikimedia.org/wikipedia/commons/thumb/e/e2/Hanno.raffael.jpg/300px-Hanno.raffael.jpg"));
			return new ISearchSuggestion[]
			{
				new SearchResultSuggestion("Result1", "Result 1 details",  "Result1,0DABE9AF-4C9F-4C09-AC30-17DE5F451A52", icon, "icon"), 
				new SearchResultSuggestion("Result2", "Result 2 details",  "Result2,0DABE9AF-4C9F-4C09-AC30-17DE5F451A52", icon, "icon"), 
			};
		}
	}
}
