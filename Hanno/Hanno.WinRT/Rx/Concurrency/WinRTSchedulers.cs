using System;
using System.Reactive.Concurrency;
using Hanno.Concurrency;

namespace Hanno.Rx.Concurrency
{
	public class WinRtSchedulers : ISchedulers
	{
		private readonly CoreDispatcherScheduler _coreDispatcher;
		private readonly IPriorityScheduler _dispatcher;
		private readonly IPriorityScheduler _threadPool;

		public WinRtSchedulers(CoreDispatcherScheduler coreDispatcher, IPriorityScheduler dispatcher, IPriorityScheduler threadPool)
		{
			if (coreDispatcher == null) throw new ArgumentNullException("coreDispatcher");
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			if (threadPool == null) throw new ArgumentNullException("threadPool");
			_coreDispatcher = coreDispatcher;
			_dispatcher = dispatcher;
			_threadPool = threadPool;
		}

		public IPriorityScheduler ThreadPool { get { return _threadPool; } }
		public IScheduler TaskPool { get { return TaskPoolScheduler.Default; } }
		public IScheduler Immediate { get { return ImmediateScheduler.Instance; } }

		public IPriorityScheduler Dispatcher
		{
			get { return _dispatcher; }
		}

		public IScheduler CurrentThread { get { return CurrentThreadScheduler.Instance; } }

		void ISchedulers.SafeDispatch(Action action)
		{
			if (_coreDispatcher.Dispatcher.HasThreadAccess)
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
