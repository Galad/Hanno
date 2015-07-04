using System;
using System.Reactive.Concurrency;
using Hanno.Concurrency;

namespace Hanno.Phone.Rx.Concurrency
{
	public class WindowsPhoneSchedulers : ISchedulers
	{
		private readonly DispatcherScheduler _dispatcher;

		public WindowsPhoneSchedulers(DispatcherScheduler dispatcher)
		{
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			_dispatcher = dispatcher;
		}

		public IPriorityScheduler ThreadPool { get { return new SingleSchedulerPriorityScheduler(ThreadPoolScheduler.Instance); } }
		public IScheduler TaskPool { get { return TaskPoolScheduler.Default; } }
		public IScheduler Immediate { get { return ImmediateScheduler.Instance; } }

		public IPriorityScheduler Dispatcher
		{
			get { return new SingleSchedulerPriorityScheduler(_dispatcher); }
		}

		public IScheduler CurrentThread { get { return CurrentThreadScheduler.Instance; } }

		public void SafeDispatch(Action action)
		{
			if (_dispatcher.Dispatcher.CheckAccess())
			{
				action();
			}
			else
			{
				_dispatcher.Schedule(action);
			}
		}
	}
}