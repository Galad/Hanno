using System;
using System.Reactive.Concurrency;

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

		public IScheduler ThreadPool { get { return ThreadPoolScheduler.Instance; } }
		public IScheduler TaskPool { get { return TaskPoolScheduler.Default; } }
		public IScheduler Immediate { get { return ImmediateScheduler.Instance; } }

		public IScheduler Dispatcher
		{
			get { return _dispatcher; }
		}

		public IScheduler CurrentThread { get { return CurrentThreadScheduler.Instance; } }

		void ISchedulers.SafeDispatch(Action action)
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