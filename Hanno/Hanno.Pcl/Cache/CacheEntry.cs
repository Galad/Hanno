using System;
using System.Collections.Generic;

namespace Hanno.Cache
{
	public class CacheEntry<T>
	{
		public Guid Id { get; private set; }
		public string CacheKey { get; private set; }
		public IDictionary<string, string> Attributes { get; private set; }
		public DateTimeOffset DateCreated { get; private set; }
		public T Value { get; set; }

		public CacheEntry(Guid id, string cacheKey, IDictionary<string,string> attributes, DateTimeOffset dateCreated, T value)
		{
			if (cacheKey == null) throw new ArgumentNullException("cacheKey");
			if (attributes == null) throw new ArgumentNullException("attributes");
			if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty", "id");
			Id = id;
			CacheKey = cacheKey;
			Attributes = attributes;
			DateCreated = dateCreated;
			Value = value;
		}
	}
}