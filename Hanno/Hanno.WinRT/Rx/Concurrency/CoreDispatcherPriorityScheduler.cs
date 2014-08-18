using System;
using System.Reactive.Concurrency;
using Windows.UI.Core;
using Hanno.Concurrency;

namespace Hanno.Rx.Concurrency
{
	public sealed class CoreDispatcherPriorityScheduler : IPriorityScheduler
	{
		private readonly CoreDispatcherScheduler _lowScheduler;
		private readonly CoreDispatcherScheduler _normalScheduler;
		private readonly CoreDispatcherScheduler _highScheduler;
		private readonly CoreDispatcherScheduler _idleScheduler;

		public CoreDispatcherPriorityScheduler(CoreDispatcher dispatcher)
		{
			_lowScheduler = new CoreDispatcherScheduler(dispatcher, CoreDispatcherPriority.Low);
			_normalScheduler = new CoreDispatcherScheduler(dispatcher, CoreDispatcherPriority.Normal);
			_highScheduler = new CoreDispatcherScheduler(dispatcher, CoreDispatcherPriority.High);
			_idleScheduler = new CoreDispatcherScheduler(dispatcher, CoreDispatcherPriority.Idle);
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
		public IScheduler High { get { return _highScheduler; } }
		public IScheduler Normal { get { return _normalScheduler; } }
		public IScheduler Low { get { return _lowScheduler; } }
		public IScheduler Idle { get { return _idleScheduler; } }
	}
}