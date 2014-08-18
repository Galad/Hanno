using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Diagnostics
{
	public class PerformanceAsyncQueryBus : IAsyncQueryBus
	{
		private readonly IAsyncQueryBus _innerQueryBus;

		public PerformanceAsyncQueryBus(IAsyncQueryBus innerQueryBus)
		{
			if (innerQueryBus == null) throw new ArgumentNullException("innerQueryBus");
			_innerQueryBus = innerQueryBus;
		}

		[DebuggerStepThrough]
		public async Task<TResult> ProcessQuery<TQuery, TResult>(TQuery query) where TQuery : IAsyncQuery<object>
		{

			var stopwatch = Stopwatch.StartNew();
			try
			{
				return await _innerQueryBus.ProcessQuery<TQuery, TResult>(query);
			}
			finally
			{
				stopwatch.Stop();
				Debug.WriteLine("Query {1} time : {0}", stopwatch.Elapsed, typeof (TQuery));
			}
		}
	}
}