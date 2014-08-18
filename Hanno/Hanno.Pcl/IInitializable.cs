﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Patterns;

namespace Hanno
{
	public interface IInitializable
	{
		Task Initialize(CancellationToken ct);
	}

	public sealed class CompositeParallelInitializable : Composite<IInitializable>
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