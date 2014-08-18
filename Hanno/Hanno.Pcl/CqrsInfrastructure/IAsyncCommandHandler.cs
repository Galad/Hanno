using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{
	public interface IAsyncCommandHandler<in TCommand> where TCommand : IAsyncCommand
	{
		Task Execute(TCommand command);
	}
}