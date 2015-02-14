using System;
using System.Threading;

namespace Hanno.CqrsInfrastructure
{
	public interface IAsyncCommand : IAsyncParameter
	{
	}

	public interface IAsyncParameter
	{
		CancellationToken CancellationToken { get; }
	}

	public interface IAsyncCommandEvent<TEvent>
	{
	}

	/// <summary>
	/// Todo: To remove, since it violates CQS
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public class AsyncCommandWithResult<TResult>
	{
		public AsyncCommandWithResult()
		{
			HasResult = false;
		}

		/// <summary>
		/// Set the result of the command
		/// </summary>
		/// <param name="result"></param>
		/// <exception cref="InvalidOperationException">Result has already been set</exception>
		public void SetResult(TResult result)
		{
			if (HasResult)
			{
				throw new InvalidOperationException("Result has already been set");
			}
			Result = result;
			HasResult = true;
		}
		public TResult Result { get; private set; }
		public bool HasResult { get; private set; }
	}
}