using System;
using System.Collections.Generic;
using System.Threading;

namespace Hanno.CqrsInfrastructure
{
	public class EnumerableArrayResultAsyncQuery<T, TResult> : AsyncQueryBase<TResult[]> where T : IAsyncQuery<object>
	{
		public EnumerableArrayResultAsyncQuery(IEnumerable<T> queries, CancellationToken ct)
			: base(ct)
		{
			if (queries == null) throw new ArgumentNullException("queries");
			Queries = queries;
		}

		public IEnumerable<T> Queries { get; private set; }
	}
}