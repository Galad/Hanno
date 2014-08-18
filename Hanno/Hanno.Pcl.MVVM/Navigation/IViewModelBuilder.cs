using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hanno.ViewModels;

namespace Hanno.Navigation
{
	public interface IViewModelBuilder
	{
		Task<IViewModel> BuildViewModel(Type viewModelType, IEnumerable<KeyValuePair<string, string>> parameters);
		Task<IViewModel> BuildViewModel<T>(Type viewModelType, IEnumerable<KeyValuePair<string, string>> parameters, T entity);
	}
}