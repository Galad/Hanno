using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Hanno.Diagnostics;
using Hanno.Extensions;
using Hanno.Navigation;

namespace Hanno.ViewModels
{
	public sealed class CreateCommandsAndOvvmViewModelFactory : IViewModelFactory
	{
		private readonly IViewModelFactory _innerViewModelFactory;
		private readonly IPageDefinitionRegistry _pageDefinitionRegistry;
		private readonly Dictionary<Type, Tuple<IList<MethodInfo>, IList<MethodInfo>>> _knownTypes = new Dictionary<Type, Tuple<IList<MethodInfo>, IList<MethodInfo>>>();

		public CreateCommandsAndOvvmViewModelFactory(IViewModelFactory innerViewModelFactory, IPageDefinitionRegistry pageDefinitionRegistry)
		{
			if (innerViewModelFactory == null) throw new ArgumentNullException("innerViewModelFactory");
			if (pageDefinitionRegistry == null) throw new ArgumentNullException("pageDefinitionRegistry");
			_innerViewModelFactory = innerViewModelFactory;
			_pageDefinitionRegistry = pageDefinitionRegistry;

			foreach (var pageDefinition in _pageDefinitionRegistry.PageDefinitions)
			{
				FindGetters(pageDefinition.ViewModelType);
			}
		}

		private Tuple<IList<MethodInfo>, IList<MethodInfo>> FindGetters(Type viewModelType)
		{
			Tuple<IList<MethodInfo>, IList<MethodInfo>> methodInfos;
			if (!_knownTypes.TryGetValue(viewModelType, out methodInfos))
			{
				var commandTypeInfo = typeof(ICommand).GetTypeInfo();
				var observablePropertyType = typeof(IObservableProperty<>);
				var ovvmTypeTypeInfo = typeof(IObservableViewModel).GetTypeInfo();
				var propertieGetters = viewModelType.GetTypeInfo()
												.DeclaredProperties
												.Where(p => commandTypeInfo.IsAssignableFrom(p.PropertyType.GetTypeInfo()) ||
															ovvmTypeTypeInfo.IsAssignableFrom(p.PropertyType.GetTypeInfo()) ||
															IsGenericTypeDefinitionProperty(p.PropertyType, observablePropertyType))
												.Select(p => p.GetMethod)
												.ToList();
				
				//find inner view models  recursively
				var viewModelTypeInfo = typeof(IViewModel).GetTypeInfo();
				var childrenGetters = viewModelType.GetTypeInfo()
												   .DeclaredProperties
												   .Where(p => viewModelTypeInfo.IsAssignableFrom(p.PropertyType.GetTypeInfo()))
												   .Select(p => p.GetMethod)
												   .ToList();

				methodInfos = new Tuple<IList<MethodInfo>, IList<MethodInfo>>(propertieGetters, childrenGetters);
				_knownTypes.Add(viewModelType, methodInfos);
				var childrenTypes = childrenGetters.Where(m => m.ReturnType != viewModelType).Select(m => m.ReturnType).ToArray();
				
				//foreach (var childrenType in childrenTypes)
				//{
				//	var innerGetter = FindGetters(childrenType);
				//	propertieGetters.AddRange(innerGetter.Item1);
				//	childrenGetters.AddRange(innerGetter.Item2);
				//}
			}
			return methodInfos;
		}

		private bool IsGenericTypeDefinitionProperty(Type type, Type genericType)
		{
			if (!type.GetTypeInfo().IsGenericType)
			{
				return false;
			}
			var genericTypeDefinition = type.GetGenericTypeDefinition();
			return genericTypeDefinition == genericType;
		}

		public IViewModel ResolveViewModel(object request)
		{
			var viewModel = _innerViewModelFactory.ResolveViewModel(request);
			var type = viewModel.GetType();
#if DEBUG
			PerformanceTime.Measure(() =>
			{
#endif
				var getters = FindGetters(type);
				InvokeProperties(viewModel, getters);
#if DEBUG
			}).DebugWriteline();
#endif
			return viewModel;
		}

		private void InvokeProperties(object viewModel, Tuple<IList<MethodInfo>, IList<MethodInfo>> getters)
		{
			foreach (var methodInfo in getters.Item1)
			{
				methodInfo.Invoke(viewModel, new object[0]);
			}
			foreach (var methodInfo in getters.Item2)
			{
				var obj = methodInfo.Invoke(viewModel, new object[0]);
				if (obj == null)
				{
					continue;
				}
				var objGetters = FindGetters(methodInfo.ReturnType);
				InvokeProperties(obj, objGetters);
			}
		}

		public void ReleaseViewModel(IViewModel viewModel)
		{
			_innerViewModelFactory.ReleaseViewModel(viewModel);
		}
	}
}