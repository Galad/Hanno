using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hanno.CqrsInfrastructure;
using Microsoft.Practices.Unity;

namespace Hanno.Unity
{
	public class AsyncQueryConvention : RegistrationConvention
	{
		private readonly IEnumerable<Assembly> _assemblies;

		public AsyncQueryConvention(IEnumerable<Assembly> assemblies)
		{
			if (assemblies == null) throw new ArgumentNullException("assemblies");
			_assemblies = assemblies;
		}

		public override IEnumerable<Type> GetTypes()
		{
			return AllClasses.FromAssemblies(_assemblies)
							 .Select(Type => new { Type, TypeInfo = Type.GetTypeInfo() })
							 .Where(t =>
							 {
								 return t.TypeInfo
								         .ImplementedInterfaces
								         .Where(type1 => type1.GenericTypeArguments.Any())
								         .Select(type1 => new {Type = type1, TypeDefinition = type1.GetGenericTypeDefinition()})
								         .Any(type1 => type1.TypeDefinition == typeof (IAsyncQueryHandler<,>) &&
								                       t.TypeInfo.Assembly != typeof (IAsyncQueryHandler<,>).GetTypeInfo().Assembly &&
								                       !t.TypeInfo.IsAbstract &&
								                       t.TypeInfo.IsPublic &&
								                       t.TypeInfo.DeclaredConstructors.Count() == 1);
							 })
							 .Select(t => t.Type);
		}

		public override Func<Type, IEnumerable<Type>> GetFromTypes()
		{
			return type => type.GetTypeInfo().ImplementedInterfaces.Where(type1 => type1.GenericTypeArguments.Any() && type1.GetGenericTypeDefinition() == typeof(IAsyncQueryHandler<,>));
		}

		public override Func<Type, string> GetName()
		{
			return null;
		}

		public override Func<Type, LifetimeManager> GetLifetimeManager()
		{
			return null;
		}

		public override Func<Type, IEnumerable<InjectionMember>> GetInjectionMembers()
		{
			return null;
		}
	}
}