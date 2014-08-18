using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public class ExecuteOnSchedulerQueryBus : ExecuteOnSchedulerBase, IAsyncQueryBus
	{
		private readonly IAsyncQueryBus _queryBus;

		public ExecuteOnSchedulerQueryBus(IAsyncQueryBus queryBus)
		{
			if (queryBus == null) throw new ArgumentNullException("queryBus");
			_queryBus = queryBus;
		}

		[DebuggerStepThrough]
		public Task<TResult> ProcessQuery<TQuery, TResult>(TQuery query) where TQuery : IAsyncQuery<object>
		{
			IScheduler scheduler;
			if (Schedulers.TryGetValue(typeof(TQuery), out scheduler))
			{
				return scheduler.Run(() => _queryBus.ProcessQuery<TQuery, TResult>(query));
			}
			if (DefaultScheduler != null)
			{
				return DefaultScheduler.Run(() => _queryBus.ProcessQuery<TQuery, TResult>(query));
			}
			return _queryBus.ProcessQuery<TQuery, TResult>(query);
		}
	}
}