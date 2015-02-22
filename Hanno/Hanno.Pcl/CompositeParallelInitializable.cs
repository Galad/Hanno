using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Patterns;

namespace Hanno
{
	public sealed class CompositeParallelInitializable : Composite<IInitializable>, IInitializable
	{
		public CompositeParallelInitializable(IEnumerable<IInitializable> instances) : base(instances)
		{
		}

		public Task Initialize(CancellationToken ct)
		{
			return Task.WhenAll(this.Select(c => c.Initialize(ct)));
		}
	}
}
