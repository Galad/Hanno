using System.Collections.Generic;

namespace Hanno.Cache
{
	public interface ICacheInfos
	{
		IDictionary<string, string> Attributes { get; }
		string Key { get; }
	}
}