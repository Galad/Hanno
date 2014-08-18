using System;
using System.Reactive.Concurrency;
using Hanno.Concurrency;

namespace Hanno
{
    public interface ISchedulers
    {
		IPriorityScheduler ThreadPool { get; }
        IScheduler TaskPool { get; }
        IScheduler Immediate { get; }
        IPriorityScheduler Dispatcher { get; }
        IScheduler CurrentThread { get; }
        /// <summary>
        /// Schedule an action on the UI thread
        /// </summary>
        /// <param name="action"></param>
        void SafeDispatch(Action action);
    }
}