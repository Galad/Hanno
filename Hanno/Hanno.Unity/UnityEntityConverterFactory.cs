using System;
using Microsoft.Practices.Unity;

namespace Hanno.Unity
{
	public class UnityEntityConverterFactory : IEntityConverterFactory
	{
		private readonly IUnityContainer _container;

		public UnityEntityConverterFactory(IUnityContainer container)
		{
			if (container == null) throw new ArgumentNullException("container");
			_container = container;
		}

		public IEntityConverter<TFrom, TTo> GetConverter<TFrom, TTo>()
		{
			return _container.Resolve<IEntityConverter<TFrom, TTo>>();
		}
	}
}