using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Testing.Autofixture
{
	public static class TaskExtensions
	{
		private const int TaskTimeout = 100000;

		public static void WaitForCompletion(this Task task)
		{
			using (var r = new ManualResetEventSlim(false))
			{
				Task.Run(async () =>
				{
					await task;
					r.Set();
				});
				r.Wait(TaskTimeout);
			}
		}

		public static void WaitForCompletion<TResult>(this Task<TResult> task, out TResult result)
		{
			var resultOut = default(TResult);
			using (var r = new ManualResetEventSlim(false))
			{
				Task.Run(async () =>
				{
					resultOut = await task;
					r.Set();
				});
				r.Wait(TaskTimeout);
			}
			result = resultOut;
		}
	}
}
