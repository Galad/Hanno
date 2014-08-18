using System;
using Hanno.CqrsInfrastructure;
using Microsoft.Practices.Unity;

namespace TestUniversalApp.Composition.Factories
{
	//public class UnityCommandQueryHandlerFactory : IAsyncCommandHandlerFactory, IAsyncQueryCommandHandlerFactory
	//{
	//	private readonly IUnityContainer _unityContainer;

	//	public UnityCommandQueryHandlerFactory(IUnityContainer unityContainer)
	//	{
	//		if (unityContainer == null) throw new ArgumentNullException("unityContainer");
	//		_unityContainer = unityContainer;
	//	}

	//	public IAsyncCommandHandler<T> Create<T>() where T : IAsyncCommand
	//	{
	//		return _unityContainer.Resolve<IAsyncCommandHandler<T>>();
	//	}

	//	public IAsyncQueryHandler<TQuery, TResult> Create<TQuery, TResult>() where TQuery : IAsyncQuery<object>
	//	{
	//		return _unityContainer.Resolve<IAsyncQueryHandler<TQuery, TResult>>();
	//	}
	//}
}