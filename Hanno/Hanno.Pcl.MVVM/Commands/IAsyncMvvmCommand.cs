using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Commands
{
	public interface IAsyncMvvmCommand
	{
		Func<CancellationToken, Exception, Task> ErrorTask { get; }
		bool SetDefaultError(Func<CancellationToken, Exception, Task> errorTask);
		string Name { get; }
	}

	public interface IAsyncMvvmCommand<TCommand, TObservable>
	{
		Func<TCommand, IObservable<TObservable>> Factory { get; }
		Func<IObserver<TObservable>> DoObserver { get; }
		void DecorateValueFactory(Func<TCommand, IObservable<TObservable>, IObservable<TObservable>> factoryDecorator);
		void DecorateDo(Func<IObserver<TObservable>, IObserver<TObservable>> factoryDecorator);
	}
}