using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Cache
{
	public sealed class InvalidateCacheService : IInvalidateCache
	{
		private readonly ICacheService _cacheService;

		public InvalidateCacheService(ICacheService cacheService)
		{
			if (cacheService == null) throw new ArgumentNullException("cacheService");
			_cacheService = cacheService;
		}

		public async Task InvalidateCache<T>(CancellationToken ct,ICacheInfos cacheInfos)
		{
			await _cacheService.Invalidate<T>(ct, cacheInfos.Key, cacheInfos.Attributes);
		}
	}
}