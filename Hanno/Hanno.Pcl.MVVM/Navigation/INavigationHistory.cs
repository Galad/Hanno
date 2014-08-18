using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Navigation
{
	public interface INavigationHistory
	{
		IReadOnlyList<INavigationHistoryEntry> Entries { get; }
		Task Remove(CancellationToken ct, INavigationHistoryEntry entry);
		/// <summary>
		/// Insert a request in the history. The page will be created when going back
		/// </summary>
		/// <param name="index">Position where the request should be inserted. Should be less than the last entry index.</param>
		/// <param name="request">Request to insert.</param>
		void InsertRequest(int index, INavigationRequest request);
		Task Clear(CancellationToken ct);
	}
}