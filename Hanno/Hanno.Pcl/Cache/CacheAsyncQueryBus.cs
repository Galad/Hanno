using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;
using Hanno.Extensions;
using Hanno.Serialization;

namespace Hanno.Cache
{
	public class CacheAsyncQueryBus : IAsyncQueryBus
	{
		private readonly IAsyncQueryBus _asyncQueryBus;
		private readonly ICacheService _cacheService;
		private readonly TimeSpan _defaultAge;

		public CacheAsyncQueryBus(IAsyncQueryBus asyncQueryBus, ICacheService cacheService, TimeSpan defaultAge)
		{
			if (asyncQueryBus == null) throw new ArgumentNullException("asyncQueryBus");
			if (cacheService == null) throw new ArgumentNullException("cacheService");
			_asyncQueryBus = asyncQueryBus;
			_cacheService = cacheService;
			_defaultAge = defaultAge;
		}
		
		public async Task<TResult> ProcessQuery<TQuery, TResult>(TQuery query) where TQuery : IAsyncQuery<object>
		{
			if (query.IsAssignableTo<ICacheInfos>())
			{
				var cacheInfo = (ICacheInfos)query;
				return await _cacheService.ExecuteWithCache(
					query.CancellationToken,
					cacheInfo.Key,
					() => _asyncQueryBus.ProcessQuery<TQuery, TResult>(query),
					_defaultAge,
					cacheInfo.Attributes);
			}
			return await _asyncQueryBus.ProcessQuery<TQuery, TResult>(query);
		}
	}
}