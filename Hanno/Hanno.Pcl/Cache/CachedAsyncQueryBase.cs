using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hanno.CqrsInfrastructure;

namespace Hanno.Cache
{
	public abstract class CachedAsyncQueryBase<T> : IAsyncQuery<T>, ICacheInfos
	{
		protected CachedAsyncQueryBase(CancellationToken cancellationToken, string cacheKey, params KeyValuePair<string,string>[] cacheAttributes)
		{
			if (string.IsNullOrEmpty(cacheKey)) throw new ArgumentNullException("cacheKey");
			if (cacheAttributes == null) throw new ArgumentNullException("cacheAttributes");
			CancellationToken = cancellationToken;
			Attributes = cacheAttributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			Key = cacheKey;
		}

		public CancellationToken CancellationToken { get; private set; }
		public IDictionary<string, string> Attributes { get; private set; }
		public string Key { get; private set; }
	}
}