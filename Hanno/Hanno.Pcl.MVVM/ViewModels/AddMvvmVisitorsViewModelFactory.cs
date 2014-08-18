using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanno.Commands;

namespace Hanno.ViewModels
{
	public sealed class AddMvvmVisitorsViewModelFactory : IViewModelFactory
	{
		private readonly IViewModelFactory _innerViewModelFactory;
		private readonly IEnumerable<IMvvmCommandVisitor> _visitors;
		private readonly IEnumerable<IObservableViewModelVisitor> _ovmVisitors;

		public AddMvvmVisitorsViewModelFactory(
			IViewModelFactory innerViewModelFactory, 
			IEnumerable<IMvvmCommandVisitor> visitors,
			IEnumerable<IObservableViewModelVisitor> ovmVisitors)
		{
			if (innerViewModelFactory == null) throw new ArgumentNullException("innerViewModelFactory");
			if (visitors == null) throw new ArgumentNullException("visitors");
			if (ovmVisitors == null) throw new ArgumentNullException("ovmVisitors");
			_innerViewModelFactory = innerViewModelFactory;
			_visitors = visitors;
			_ovmVisitors = ovmVisitors;
		}

		public IViewModel ResolveViewModel(object request)
		{
			var viewModel = _innerViewModelFactory.ResolveViewModel(request);
			foreach (var mvvmCommandVisitor in _visitors)
			{
				viewModel.CommandBuilderProvider.AddVisitor(mvvmCommandVisitor);
			}
			foreach (var ovmVisitor in _ovmVisitors)
			{
				viewModel.OvmBuilderProvider.AddVisitor(ovmVisitor);
			}
			return viewModel;
		}

		public void ReleaseViewModel(IViewModel viewModel)
		{
			_innerViewModelFactory.ReleaseViewModel(viewModel);
		}
	}
}
