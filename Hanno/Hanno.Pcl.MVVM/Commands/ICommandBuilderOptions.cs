using System;

namespace Hanno.Commands
{
    public interface ICommandBuilderOptions<out T, out TNext> : ICommandBuilderOptions<TNext>
    {
        TNext CanExecute(Func<T, bool> predicate);
    }

    public interface ICommandBuilderOptions<out TNext> : ICommandBuilderToCommand
    {
        TNext CanExecute(Func<bool> predicate);
        TNext CanExecute(IObservable<bool> observablePredicate);
    }
}