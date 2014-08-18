using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hanno.Validation.Rules;
using Microsoft.Practices.Unity;

namespace Hanno.Unity
{
	public class ValidationRuleConvention : RegistrationConvention
	{
		private readonly IDictionary<Assembly, string> _prefixMapping;

		public ValidationRuleConvention(IDictionary<Assembly, string> prefixMapping)
		{
			if (prefixMapping == null) throw new ArgumentNullException("prefixMapping");
			_prefixMapping = prefixMapping;
		}

		public override IEnumerable<Type> GetTypes()
		{
			var types = AllClasses.FromAssemblies(_prefixMapping.Keys)
								  .Where(t => typeof(ValidationRule).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()) && t != typeof(CompositeValidationRule<>));
			return types;
		}

		public override Func<Type, IEnumerable<Type>> GetFromTypes()
		{
			return t => t.GetTypeInfo().ImplementedInterfaces;
		}

		public override Func<Type, string> GetName()
		{
			return type => string.Format("{0}.{1}", _prefixMapping[type.GetTypeInfo().Assembly], type.Name);
		}

		public override Func<Type, LifetimeManager> GetLifetimeManager()
		{
			return _ => new TransientLifetimeManager();
		}

		public override Func<Type, IEnumerable<InjectionMember>> GetInjectionMembers()
		{
			return null;
		}
	}
}