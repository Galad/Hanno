using System;
using System.Threading.Tasks;
using Hanno.CqrsInfrastructure;

namespace Hanno.Http
{
	/// <summary>
	/// A query / command bus, which executes http queries and commands.
	/// </summary>
	public class DefaultHttpRequestBus : IAsyncQueryBus, IAsyncCommandBus
	{
		private readonly IHttpRequestResolver _httpRequestResolver;
		private readonly IHttpRequestResultReaderResolver _httpRequestResultReaderProvider;


		public DefaultHttpRequestBus(
			IHttpRequestResolver httpRequestResolver,
			IHttpRequestResultReaderResolver httpRequestResultReaderProvider
			)
		{
			if (httpRequestResolver == null) throw new ArgumentNullException("httpRequestResolver");
			if (httpRequestResultReaderProvider == null) throw new ArgumentNullException("httpRequestResultReaderProvider");
			_httpRequestResolver = httpRequestResolver;
			_httpRequestResultReaderProvider = httpRequestResultReaderProvider;
		}

		public Task<TResult> ProcessQuery<TQuery, TResult>(TQuery query) where TQuery : IAsyncQuery<object>
		{
			return ProcessQueryInternal<TQuery, TResult>(query);
		}

		public Task ProcessCommand<TCommand>(TCommand command) where TCommand : IAsyncCommand
		{
			return ProcessQueryInternal<TCommand, NoResponse>(command);
		}

		private async Task<TResult> ProcessQueryInternal<TQuery, TResult>(TQuery query) where TQuery : IAsyncParameter
		{
			var request = await _httpRequestResolver.ResolveHttpRequest(query);
			var requestResult = await request.Execute(query.CancellationToken);
			var reader = _httpRequestResultReaderProvider.ResolveHttpRequestResultReader(query);
			return await reader.Read<TResult>(requestResult, query.CancellationToken);
		}
	}
}