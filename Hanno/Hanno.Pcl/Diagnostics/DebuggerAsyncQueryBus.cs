using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Diagnostics
{
	public class DebuggerAsyncQueryBus : IAsyncQueryBus
	{
		private readonly IAsyncQueryBus _innerQueryBus;
		private readonly ConditionalWeakTable<Exception, Exception> _catchedExceptions = new ConditionalWeakTable<Exception, Exception>(); 

		public DebuggerAsyncQueryBus(IAsyncQueryBus innerQueryBus)
		{
			if (innerQueryBus == null) throw new ArgumentNullException("innerQueryBus");
			_innerQueryBus = innerQueryBus;
		}

		[DebuggerStepThrough]
		public async Task<TResult> ProcessQuery<TQuery, TResult>(TQuery query) where TQuery : IAsyncQuery<object>
		{
			Debug.WriteLine("[QUERY]   [START] {0}", query);
			try
			{
				var result = await _innerQueryBus.ProcessQuery<TQuery, TResult>(query);
				Debug.WriteLine("[QUERY]   [END]   {0}", query);
				return result;
			}
			catch (Exception ex)
			{
				//prevent writing an exception multiple times
				Exception existing;
				if (!_catchedExceptions.TryGetValue(ex, out existing))
				{
					_catchedExceptions.Add(ex, ex);
					Debug.WriteLine("[QUERY]   [ERROR] {0}\n{1}", query, ex);
				}
				throw;
			}
		}
	}
}