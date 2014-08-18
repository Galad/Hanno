using System;
using System.Collections.Generic;
using System.Linq;

namespace Hanno.Navigation
{
	public class PageDefinitionRegistry : IPageDefinitionRegistry
	{
		private readonly IDictionary<string, PageDefinition> _pageDefinitions = new Dictionary<string, PageDefinition>();

		public IPageDefinitionRegistry RegisterPageDefinition(PageDefinition pageDefinition)
		{
			if (pageDefinition == null) throw new ArgumentNullException("pageDefinition");
			_pageDefinitions[pageDefinition.ViewName] = pageDefinition;
			return this;
		}

		public PageDefinition GetPageDefinition(string pageName)
		{
			return _pageDefinitions[pageName];
		}

		public IEnumerable<PageDefinition> PageDefinitions { get { return _pageDefinitions.Select(kvp => kvp.Value); } }
	}
}