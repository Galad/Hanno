using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Navigation
{
	public interface IRequestNavigation
	{
		/// <summary>
		/// Navigate to the specified Uri
		/// </summary>		
		Task Navigate(CancellationToken ct, INavigationRequest request);
	}
}