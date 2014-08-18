using System;
using Hanno.Storage;
using Microsoft.Practices.Unity;

namespace TestUniversalApp.Composition.Factories
{
	public class UnityAsyncDataTableFactory : IAsyncDataTableFactory
	{
		private readonly IUnityContainer _container;
		private readonly string _resolutionName;

		public UnityAsyncDataTableFactory(
			IUnityContainer container,
			string resolutionName)
		{
			if (container == null) throw new ArgumentNullException("container");
			_container = container;
			_resolutionName = resolutionName;
		}

		public IAsyncDataTable<TKey, TValue> GetAsyncTable<TKey, TValue>()
		{
			if (string.IsNullOrEmpty(_resolutionName))
			{
				return _container.Resolve<IAsyncDataTable<TKey, TValue>>();
			}
			return _container.Resolve<IAsyncDataTable<TKey, TValue>>(_resolutionName);
		}

		public void Release<TKey, TValue>(IAsyncDataTable<TKey, TValue> table)
		{
			var disposable = table as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
	}
}