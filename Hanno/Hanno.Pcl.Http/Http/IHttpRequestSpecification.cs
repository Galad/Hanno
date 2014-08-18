namespace Hanno.Http
{
	public interface IHttpRequestSpecification
	{
		IHttpRequestBuilderOptions CreateSpecifiation(IHttpRequestBuilderFactory httpRequestMethodBuilder);
	}
}