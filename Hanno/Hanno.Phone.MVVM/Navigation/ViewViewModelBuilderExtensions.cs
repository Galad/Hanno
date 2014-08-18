using System;
using Hanno.ViewModels;

namespace Hanno.Navigation
{
	public static class ViewViewModelBuilderExtensions
	{
		public static IPageDefinitionRegistry RegisterViewModel<TViewModel, TView>(this IPageDefinitionRegistry viewViewModelBuilder, string name, Uri uri = null)
			where TViewModel : IViewModel
		{
			return viewViewModelBuilder.RegisterPageDefinition(
				new PageDefinition(
					name,
					typeof(TViewModel),
					typeof(TView),
					uri
					)
				);
		}
	}
}