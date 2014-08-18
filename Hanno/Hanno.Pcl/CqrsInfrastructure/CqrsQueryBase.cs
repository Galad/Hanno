using System.Threading;

namespace Hanno.CqrsInfrastructure
{
	public abstract class AsyncQueryBase<TResult> : CqrsParameterBase, IAsyncQuery<TResult>
	{
		protected AsyncQueryBase(CancellationToken ct)
			: base(ct)
		{
		}

		public CancellationToken CancellationToken { get { return CancellationTokenInternal; } }
	}
}