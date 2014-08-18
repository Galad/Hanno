using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Reactive.Concurrency
{
	public static class SchedulerExtensions
	{
		public static Task Run(this IScheduler scheduler, Func<Task> task)
		{
			if (scheduler == null) throw new ArgumentNullException("scheduler");
			if (task == null) throw new ArgumentNullException("task");

			var taskCompletionSource = new TaskCompletionSource<Unit>();

			scheduler.ScheduleAsync(async (s, ct) =>
			{
				try
				{
					await task();
					taskCompletionSource.TrySetResult(Unit.Default);
				}
				catch (Exception ex)
				{
					taskCompletionSource.TrySetException(ex);
				}
			});
			return taskCompletionSource.Task;
		}

		public static Task<T> Run<T>(this IScheduler scheduler, Func<Task<T>> task)
		{
			if (scheduler == null) throw new ArgumentNullException("scheduler");
			if (task == null) throw new ArgumentNullException("task");

			var taskCompletionSource = new TaskCompletionSource<T>();

			scheduler.ScheduleAsync(async (s, ct) =>
			{
				try
				{
					taskCompletionSource.TrySetResult(await task());

				}
				catch (Exception ex)
				{
					taskCompletionSource.TrySetException(ex);
				}
			});
			return taskCompletionSource.Task;
		}

		public static Task<T> Run<T>(this IScheduler scheduler, Func<T> func)
		{
			if (scheduler == null) throw new ArgumentNullException("scheduler");
			if (func == null) throw new ArgumentNullException("func");
			var taskCompletionSource = new TaskCompletionSource<T>();

			scheduler.Schedule(() =>
			{
				try
				{
					taskCompletionSource.TrySetResult(func());

				}
				catch (Exception ex)
				{
					taskCompletionSource.TrySetException(ex);
				}
			});
			return taskCompletionSource.Task;
		}

		public static Task Run(this IScheduler scheduler, Action action)
		{
			return scheduler.Run(() =>
			{
				action();
				return Unit.Default;
			});
		}
	}
}
