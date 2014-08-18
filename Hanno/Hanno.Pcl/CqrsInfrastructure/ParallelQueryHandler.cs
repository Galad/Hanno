using System;
using System.Linq;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public class ParallelQueryHandler<TQuery, TResult> : IAsyncQueryHandler<EnumerableArrayResultAsyncQuery<TQuery, TResult>, TResult[]>
		where TQuery : IAsyncQuery<object>
    {
		private readonly IAsyncQueryHandler<TQuery, TResult> _queryHandler;

		public ParallelQueryHandler(IAsyncQueryHandler<TQuery, TResult> queryHandler)
        {
            if (queryHandler == null) throw new ArgumentNullException("queryHandler");
            _queryHandler = queryHandler;
        }

		public Task<TResult[]> Execute(EnumerableArrayResultAsyncQuery<TQuery, TResult> queries)
		{
			if (queries == null) throw new ArgumentNullException("queries");
			var tasks = queries.Queries
			                   .Select(query => _queryHandler.Execute(query));
			return Task.WhenAll(tasks);
		}
    }
}