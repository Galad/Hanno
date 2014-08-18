
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public abstract class DelegateCommandHandler<TCommand> : IAsyncCommandHandler<TCommand> where TCommand : IAsyncCommand
	{
		protected abstract void ExecuteOverride(TCommand command);
		public virtual Task Execute(TCommand command)
		{
			return Task.Run(() => ExecuteOverride(command), command.CancellationToken);
		}
	}
}