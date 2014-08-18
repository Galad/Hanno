namespace Hanno.CqrsInfrastructure
{
	public interface IAsyncQueryCommandHandlerFactory
	{
		IAsyncQueryHandler<TQuery, TResult> Create<TQuery, TResult>() where TQuery : IAsyncQuery<object>;
	}
}