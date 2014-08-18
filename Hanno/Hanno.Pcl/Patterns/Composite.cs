using System.Collections.Generic;

namespace Hanno.Patterns
{
	public abstract class Composite<T> : List<T>
	{
		protected Composite(IEnumerable<T> instances) : base(instances)
		{
		}

		protected Composite(params T[] instances) : base(instances)
		{
		}
	}
}