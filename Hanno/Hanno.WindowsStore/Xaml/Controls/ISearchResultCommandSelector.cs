namespace Hanno.Xaml.Controls
{
	public interface ISearchResultCommandSelector
	{
		bool CanHandleTag(string tag);
		object SelectTag(string tag);
	}
}