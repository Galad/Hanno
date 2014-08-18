using System.Collections;
using System.Collections.Generic;

namespace Hanno.Navigation
{
	public interface IPageDefinitionRegistry
	{
		IPageDefinitionRegistry RegisterPageDefinition(PageDefinition pageDefinition);
		PageDefinition GetPageDefinition(string pageName);
		IEnumerable<PageDefinition> PageDefinitions { get; }
	}
}