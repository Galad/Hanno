using System;
using System.Reactive.Concurrency;

namespace Hanno.Concurrency
{
	public class SingleSchedulerPriorityScheduler : IPriorityScheduler
	{
		private readonly IScheduler _scheduler;

		public SingleSchedulerPriorityScheduler(IScheduler scheduler)
		{
			if (scheduler == null) throw new ArgumentNullException("scheduler");
			_scheduler = scheduler;
		}

		public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
		{
			return _scheduler.Schedule(state, action);
		}

		public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			return _scheduler.Schedule(state, dueTime, action);
		}

		public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			return _scheduler.Schedule(state, dueTime, action);
		}

		public DateTimeOffset Now { get { return _scheduler.Now; } }
		public IScheduler High { get { return _scheduler; } }
		public IScheduler Normal { get { return _scheduler; } }
		public IScheduler Low { get { return _scheduler; } }
	}
}