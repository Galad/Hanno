using System.Collections.Generic;
using System.Linq;
using Hanno.Extensions;

namespace Hanno.ViewModels
{
	public sealed class EnumerableEmptyPredicateVisitor : IObservableViewModelVisitor
	{
		public void Visit<T>(ObservableViewModel<T> ovm)
		{
			ovm.ChainEmptyPredicate(t =>
			{
				var enumerable = t as IEnumerable<object>;
				if (enumerable == null)
				{
					return false;
				}
				return !enumerable.Any();
			});
		}
	}

	public sealed class NullReferenceEmptyPredicateVisitor : IObservableViewModelVisitor
	{
		public void Visit<T>(ObservableViewModel<T> ovm)
		{
			ovm.ChainEmptyPredicate(t => t == null);
		}
	}
}