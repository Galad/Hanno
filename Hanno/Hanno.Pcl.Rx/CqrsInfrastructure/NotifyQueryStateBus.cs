using System;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public class NotifyQueryStateBus : IAsyncQueryBus, IQueryStateEvents
	{
		private readonly IAsyncQueryBus _queryBus;
		private readonly Subject<IAsyncQuery<object>> _queryStarted = new Subject<IAsyncQuery<object>>();
		private readonly Subject<Tuple<IAsyncQuery<object>, object>> _queryEnded = new Subject<Tuple<IAsyncQuery<object>, object>>();
		private readonly Subject<Tuple<IAsyncQuery<object>, Exception>> _queryError = new Subject<Tuple<IAsyncQuery<object>, Exception>>();

		public NotifyQueryStateBus(
			IAsyncQueryBus queryBus)
		{
			if (queryBus == null) throw new ArgumentNullException("queryBus");
			_queryBus = queryBus;
		}

		[DebuggerStepThrough]
		public Task<TResult> ProcessQuery<TQuery, TResult>(TQuery query) where TQuery : IAsyncQuery<object>
		{
			try
			{
				_queryStarted.OnNext(query);
				var result = _queryBus.ProcessQuery<TQuery, TResult>(query);
				_queryEnded.OnNext(new Tuple<IAsyncQuery<object>, object>(query, result));
				return result;
			}
			catch (Exception ex)
			{
				_queryError.OnNext(new Tuple<IAsyncQuery<object>, Exception>(query, ex));
				throw;
			}
		}

		#region CommandStateEvents
		public IObservable<IAsyncQuery<object>> ObserveQueryStarted()
		{
			return _queryStarted;
		}

		public IObservable<Tuple<IAsyncQuery<object>, object>> ObserveQueryEnded()
		{
			return _queryEnded;
		}

		public IObservable<Tuple<IAsyncQuery<object>, Exception>> ObserveQueryError()
		{
			return _queryError;
		}
		#endregion

	}
}