using System;
using System.Reactive.Concurrency;
using Hanno.Concurrency;

namespace Hanno.Rx
{
    public class WpfSchedulers : ISchedulers
    {
		private readonly DispatcherScheduler _dispatcher;

	    public WpfSchedulers(DispatcherScheduler dispatcher)
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

		IPriorityScheduler ISchedulers.ThreadPool
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		IPriorityScheduler ISchedulers.Dispatcher
		{
			get
			{
				throw new NotImplementedException();
			}
		}

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