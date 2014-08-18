using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanno.Commands;

namespace Hanno.ViewModels
{
	public sealed class AddMvvmCommandVisitorsViewModelFactory : IViewModelFactory
	{
		private readonly IViewModelFactory _innerViewModelFactory;
		private readonly IEnumerable<IMvvmCommandVisitor> _visitors;

		public AddMvvmCommandVisitorsViewModelFactory(IViewModelFactory innerViewModelFactory, IEnumerable<IMvvmCommandVisitor> visitors)
		{
			if (innerViewModelFactory == null) throw new ArgumentNullException("innerViewModelFactory");
			if (visitors == null) throw new ArgumentNullException("visitors");
			_innerViewModelFactory = innerViewModelFactory;
			_visitors = visitors;
		}

		public IViewModel ResolveViewModel(object request)
		{
			var viewModel = _innerViewModelFactory.ResolveViewModel(request);
			foreach (var mvvmCommandVisitor in _visitors)
			{
				viewModel.CommandBuilderProvider.AddVisitor(mvvmCommandVisitor);
			}
			return viewModel;
		}

		public void ReleaseViewModel(IViewModel viewModel)
		{
			_innerViewModelFactory.ReleaseViewModel(viewModel);
		}
	}
}
