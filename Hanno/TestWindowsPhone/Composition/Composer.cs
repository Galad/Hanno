using System;
using Castor.Composition;
using Microsoft.Practices.Unity;

namespace TestWindowsPhone.Composition
{
	public class Composer
	{
		public readonly IUnityContainer Container;
		public readonly IUnityModule[] Modules;

		public Composer(IUnityContainer container, params IUnityModule[] modules)
		{
			if (container == null) throw new ArgumentNullException("container");
			if (modules == null) throw new ArgumentNullException("modules");
			Container = container;
			Modules = modules;
		}

		public void Register()
		{
			foreach (var module in Modules)
			{
				module.Register(Container);
			}
		}
	}
}