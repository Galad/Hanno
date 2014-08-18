using System;
using System.Reactive.Disposables;
using Microsoft.Reactive.Testing;

namespace Hanno.Testing.Autofixture
{
    public class ThrowingTestScheduler : TestScheduler
    {
        public ITestableObserver<T> Start<T>(Func<IObservable<T>> create, long created, long subscribed, long disposed)
        {
            return base.Start(create, created, subscribed, disposed).AssertExceptions();
        }

        public ITestableObserver<T> Start<T>(Func<IObservable<T>> create, long disposed)
        {
            return base.Start(create, disposed).AssertExceptions();
        }

        public ITestableObserver<T> Start<T>(Func<IObservable<T>> create)
        {
            return base.Start(create).AssertExceptions();
        }
    }

    public static class TestSchedulerExtensions
    {
        public static void Start(this TestScheduler scheduler, Action subscribe, Action disposeAction)
        {
            long created = 100L;
            long subscribed = 200L;
            long disposed = 1000L;
            scheduler.ScheduleAbsolute((object)null, created, (s, state) => Disposable.Empty);
            scheduler.ScheduleAbsolute((object)null, subscribed, (s, state) =>
            {
                subscribe();
                return Disposable.Empty;
            });
            scheduler.ScheduleAbsolute((object)null, disposed, (s, state) =>
            {
                disposeAction();
                return Disposable.Empty;
            });
            scheduler.Start();
        }
    }
}