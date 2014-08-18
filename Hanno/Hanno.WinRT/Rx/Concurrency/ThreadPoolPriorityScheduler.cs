using System;
using System.Reactive.Concurrency;
using Windows.System.Threading;
using Hanno.Concurrency;

namespace Hanno.Rx.Concurrency
{
	/// <summary>
	/// PriorityScheduler using the thread pool.
	/// </summary>
	public sealed class ThreadPoolPriorityScheduler : IPriorityScheduler
	{
		private readonly ThreadPoolScheduler _lowScheduler;
		private readonly ThreadPoolScheduler _normalScheduler;
		private readonly ThreadPoolScheduler _highScheduler;

		public ThreadPoolPriorityScheduler()
		{
			_lowScheduler = new ThreadPoolScheduler(WorkItemPriority.Low);
			_normalScheduler = new ThreadPoolScheduler(WorkItemPriority.Normal);
			_highScheduler = new ThreadPoolScheduler(WorkItemPriority.High);
		}

		public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
		{
			return _normalScheduler.Schedule(state, action);
		}

		public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			return _normalScheduler.Schedule(state, dueTime, action);
		}

		public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
		{
			return _normalScheduler.Schedule(state, dueTime, action);
		}

		public DateTimeOffset Now { get { return _normalScheduler.Now; } }
		public IScheduler SchedulerFromPriority(SchedulerPriority priority)
		{
			switch (priority)
			{
				case SchedulerPriority.Lowest:
				case SchedulerPriority.Low:
					return _lowScheduler;
				case SchedulerPriority.Normal:
					return _normalScheduler;
				case SchedulerPriority.High:
					return _highScheduler;
				default:
					throw new ArgumentOutOfRangeException("priority");
			}
		}

		public IScheduler High { get { return _highScheduler; } }
		public IScheduler Normal { get { return _normalScheduler; } }
		public IScheduler Low { get { return _lowScheduler; } }
		public IScheduler Lowest { get { return _lowScheduler; } }
	}
}