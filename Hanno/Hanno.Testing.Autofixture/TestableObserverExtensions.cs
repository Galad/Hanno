using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using System.Reactive.Linq;
using Xunit;

namespace Hanno.Testing.Autofixture
{
    public static class TestableObserverExtensions
    {
        public static IEnumerable<Exception> Errors<T>(this ITestableObserver<T> observer)
        {
            return observer.Messages.Where(m => m.Value.Kind == System.Reactive.NotificationKind.OnError).Select(r => r.Value.Exception);
        }

        public static IEnumerable<T> Values<T>(this ITestableObserver<T> observer)
        {
            return observer.Messages.Where(m => m.Value.Kind == System.Reactive.NotificationKind.OnNext).Select(r => r.Value.Value);
        }

        public static IEnumerable<Recorded<Notification<T>>> Completed<T>(this ITestableObserver<T> observer)
        {
            return observer.Messages.Where(m => m.Value.Kind == System.Reactive.NotificationKind.OnCompleted);
        }

        public static void ShouldBeAsyncData<T>(this ITestableObserver<T> observer)
        {
            observer.Errors().Should().BeEmpty("An error occured in the observable");
            observer.Values().Should().HaveCount(1);
            observer.Completed().Should().HaveCount(1);
        }

        public static ITestableObserver<T> AssertExceptions<T>(this ITestableObserver<T> observer)
        {
            var errors = observer.Errors();
            if (errors.Any())
            {
                Assert.True(false, string.Format("An exception occured in the observable sequence : {0}, {1}\n{2}",
                                                 errors.First().GetType(),
                                                 errors.First().Message,
                                                 errors.First().StackTrace));
            }
            return observer;
        }
    }
}
