using System;

namespace Hanno.Navigation
{
	public class PageDefinition
	{
		public readonly Type PageType;
		public readonly Type ViewModelType;
		public readonly string ViewName;
		public readonly Uri Uri;

		public PageDefinition(string viewName, Type viewModelType, Type pageType, Uri uri)
		{
			if (viewName == null) throw new ArgumentNullException("viewName");
			if (viewModelType == null) throw new ArgumentNullException("viewModelType");
			if (pageType == null) throw new ArgumentNullException("pageType");
			ViewName = viewName;
			ViewModelType = viewModelType;
			PageType = pageType;
			Uri = uri;
		}
	}
}