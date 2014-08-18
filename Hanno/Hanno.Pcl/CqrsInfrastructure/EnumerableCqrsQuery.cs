using System.Collections.Generic;
using System.Threading;

namespace Hanno.CqrsInfrastructure
{
	public class EnumerableAsyncQuery<T, TResult> : AsyncQueryBase<TResult> where T : IAsyncQuery<TResult>
	{
		public EnumerableAsyncQuery(IEnumerable<T> queries, CancellationToken ct)
			: base(ct)
		{
			Queries = queries;
		}

		public IEnumerable<T> Queries { get; private set; }
	}
}