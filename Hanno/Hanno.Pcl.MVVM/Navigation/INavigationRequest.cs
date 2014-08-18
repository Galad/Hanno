using System.Collections.Generic;

namespace Hanno.Navigation
{
	public interface INavigationRequest
	{
		string PageName { get; }
		IDictionary<string, string> Parameters { get; }
		T Entity<T>();
	}

	public class NavigationRequest : INavigationRequest
	{
		private readonly object _entity;

		public NavigationRequest(string pageName, IDictionary<string, string> parameters, object entity = null)
		{
			Parameters = parameters;
			PageName = pageName;
			_entity = entity;
		}

		public string PageName { get; private set; }
		public IDictionary<string, string> Parameters { get; private set; }

		public T Entity<T>()
		{
			return (T)_entity;
		}
	}
	
}