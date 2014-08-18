using System;
using System.Globalization;
using Windows.ApplicationModel.Resources;

namespace Hanno.Globalization
{
	public class ResourceLoaderService : IResources
	{
		private readonly ResourceLoader _resourceLoader;

		public ResourceLoaderService(ResourceLoader resourceLoader)
		{
			if (resourceLoader == null) throw new ArgumentNullException("resourceLoader");
			_resourceLoader = resourceLoader;
		}

		public static ResourceLoaderService Create(string name)
		{
			return new ResourceLoaderService(ResourceLoader.GetForCurrentView(name));
		}

		public static ResourceLoaderService Create()
		{
			return new ResourceLoaderService(ResourceLoader.GetForCurrentView());
		}

		public string Get(string name)
		{
			return _resourceLoader.GetString(name);
		}

		public string Get(string name, CultureInfo cultureInfo)
		{
			return null;
		}
	}
}