using System;
using System.Collections.Generic;
using System.Threading;

namespace Hanno.CqrsInfrastructure
{
	public class EnumerableAsyncCommand<T> : AsyncCommandBase where T : IAsyncCommand
	{
		public EnumerableAsyncCommand(IEnumerable<T> commands, CancellationToken ct)
			: base(ct)
		{
			if (commands == null) throw new ArgumentNullException("commands");
			Commands = commands;
		}

		public IEnumerable<T> Commands { get; private set; }
	}
}