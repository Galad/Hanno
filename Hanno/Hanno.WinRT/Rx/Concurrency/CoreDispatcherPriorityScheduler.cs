using System;
using System.Reactive.Concurrency;
using Windows.UI.Core;
using Hanno.Concurrency;

namespace Hanno.Rx.Concurrency
{
	/// <summary>
	/// A priority scheduler using the CoreDispatcher scheduler
	/// </summary>
	public sealed class CoreDispatcherPriorityScheduler : IPriorityScheduler
	{
		private readonly CoreDispatcherScheduler _lowScheduler;
		private readonly CoreDispatcherScheduler _normalScheduler;
		private readonly CoreDispatcherScheduler _highScheduler;
		private readonly CoreDispatcherScheduler _lowestScheduler;

		public CoreDispatcherPriorityScheduler(CoreDispatcher dispatcher)
		{
			_lowScheduler = new CoreDispatcherScheduler(dispatcher, CoreDispatcherPriority.Low);
			_normalScheduler = new CoreDispatcherScheduler(dispatcher, CoreDispatcherPriority.Normal);
			_highScheduler = new CoreDispatcherScheduler(dispatcher, CoreDispatcherPriority.High);
			_lowestScheduler = new CoreDispatcherScheduler(dispatcher, CoreDispatcherPriority.Idle);
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
					return _lowestScheduler;
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
	}
}