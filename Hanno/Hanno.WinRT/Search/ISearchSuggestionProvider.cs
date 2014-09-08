using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Search
{
	public interface ISearchSuggestionProvider
	{
		Task<IList<ISearchSuggestion>> GetSuggestions(CancellationToken ct, string query);
	}
}