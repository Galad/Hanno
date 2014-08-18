using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	/// <summary>
	/// Execute a command
	/// </summary>
	public interface IAsyncCommandBus
	{
		Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand;
	}
}
