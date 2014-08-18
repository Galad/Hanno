using System.Reactive.Concurrency;

namespace Hanno.Commands
{
    public interface IObservableCommandBuilderSchedulerOptions<out TCommand, out TObservable>
    {
        IObservableCommandBuilderOptions<TCommand, TObservable> WithScheduler(IScheduler scheduler);
        IObservableCommandBuilderOptions<TCommand, TObservable> WithDefaultScheduler();
    }
}