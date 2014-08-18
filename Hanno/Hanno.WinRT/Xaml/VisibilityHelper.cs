using Windows.UI.Xaml;
namespace Hanno.Xaml
{
	public static class VisibilityHelper
	{
		public static Visibility InverseVisibility(Visibility visibility)
		{
			return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
		}
	}
}