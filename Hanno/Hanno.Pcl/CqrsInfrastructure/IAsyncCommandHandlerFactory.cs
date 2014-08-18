using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanno.CqrsInfrastructure
{

	public interface IAsyncCommandHandlerFactory
	{
		IAsyncCommandHandler<T> Create<T>() where T : IAsyncCommand;
	}
}
