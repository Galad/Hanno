using System.Windows.Input;

namespace Hanno.Commands
{
	public interface IMvvmCommandVisitor
	{
		void Visit(IAsyncMvvmCommand command);
		void Visit(ICommand command);
		void Visit<TCommand, TObservable>(IAsyncMvvmCommand<TCommand, TObservable> command);
	}
}