using System.Threading;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Cache
{
	public interface IInvalidateCache
	{
		/// <summary>
		/// Invalidate all cached result for the specified infos.
		/// </summary>
		/// <returns></returns>
		Task InvalidateCache<T>(CancellationToken ct, ICacheInfos cacheInfos);
	}
}