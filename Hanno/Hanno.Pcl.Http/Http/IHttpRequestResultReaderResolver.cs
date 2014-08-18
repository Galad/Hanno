namespace Hanno.Http
{
	public interface IHttpRequestResultReaderResolver
	{
		IHttpRequestResultReader ResolveHttpRequestResultReader<TQuery>(TQuery query);
	}
}