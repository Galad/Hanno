using System;
using System.Reactive.Concurrency;
using Hanno.Concurrency;

namespace Hanno.Rx
{
    public class WpfSchedulers : ISchedulers
    {
		private readonly DispatcherScheduler _dispatcher;
	    private readonly IPriorityScheduler _priorityDispatcher;
		private readonly IPriorityScheduler _threadPool;

	    public WpfSchedulers(DispatcherScheduler dispatcher, IPriorityScheduler priorityDispatcher, IPriorityScheduler threadPool)
	    {
			if (dispatcher == null) throw new ArgumentNullException("dispatcher");
			if (priorityDispatcher == null) throw new ArgumentNullException("priorityDispatcher");
			if (threadPool == null) throw new ArgumentNullException("threadPool");
			_dispatcher = dispatcher;
			_threadPool = threadPool;
			_priorityDispatcher = priorityDispatcher;
	    }
	    
        public IScheduler TaskPool { get { return TaskPoolScheduler.Default; } }
        public IScheduler Immediate { get { return ImmediateScheduler.Instance; } }
		
	    public IScheduler CurrentThread { get { return CurrentThreadScheduler.Instance; } }

		IPriorityScheduler ISchedulers.ThreadPool
		{
			get { return _threadPool; }
		}

		IPriorityScheduler ISchedulers.Dispatcher
		{
			get { return _priorityDispatcher; }
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