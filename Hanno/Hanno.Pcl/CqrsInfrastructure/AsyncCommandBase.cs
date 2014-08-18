using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Hanno.CqrsInfrastructure
{
	public abstract class AsyncCommandBase : CqrsParameterBase, IAsyncCommand
	{
		protected AsyncCommandBase(CancellationToken ct)
			: base(ct)
		{
		}

		public CancellationToken CancellationToken { get { return CancellationTokenInternal; } }
	}
}