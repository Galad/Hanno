using System;
using Hanno.Extensions;

namespace System.Windows
{
	public static class FrameworkElementExtensions
	{
		public static T FindFirstParentOfType<T>(this FrameworkElement element) where T : FrameworkElement
		{
			if (element == null) throw new ArgumentNullException("element");
			while (element.Parent != null && !element.Parent.IsAssignableTo<T>())
			{
				element = (FrameworkElement)element.Parent;
			}
			return (T)element.Parent;
		}
	}
}