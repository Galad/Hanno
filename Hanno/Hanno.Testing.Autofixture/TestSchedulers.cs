using System;
using System.Reactive.Concurrency;
using Hanno.Concurrency;
using Hanno.Rx;
using Microsoft.Reactive.Testing;

namespace Hanno.Testing.Autofixture
{
    public class TestSchedulers : ThrowingTestScheduler, ISchedulers, IPriorityScheduler
    {
		IPriorityScheduler ISchedulers.ThreadPool { get { return this; } }
        IScheduler ISchedulers.TaskPool { get { return this; } }
        IScheduler ISchedulers.Immediate { get { return ImmediateScheduler.Instance; } }
		IPriorityScheduler ISchedulers.Dispatcher { get { return this; } }
        IScheduler ISchedulers.CurrentThread { get { return this; } }

        public void SafeDispatch(Action action)
        {
            ImmediateScheduler.Instance.Schedule(action);
        }

	    public IScheduler High { get { return this; } }
		public IScheduler Normal { get { return this; } }
		public IScheduler Low { get { return this; } }
    }
}