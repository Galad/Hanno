using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Hanno.CqrsInfrastructure
{
	public abstract class CqrsParameterBase
	{
		private readonly CancellationToken _cancellationToken;
		protected CancellationToken CancellationTokenInternal
		{
			get { return _cancellationToken; }
		}

		protected CqrsParameterBase(CancellationToken ct)
		{
			_cancellationToken = ct;
		}

		public override string ToString()
		{
#if !DEBUG
			return base.ToString();
#else
			var properties = GetPropertyValues();
			string separator = string.Empty;
			if (!string.IsNullOrEmpty(properties))
			{
				separator = ", ";
			}
			return string.Format("{0}{1}{2}", base.ToString(), separator, GetPropertyValues());
#endif
		}

		private string GetPropertyValues()
		{
			return string.Join(", ", Properties
				.Where(p => p.Name != "CancellationToken")
				.Select(p => string.Concat(p.Name, "=", p.GetMethod.Invoke(this, new object[0]))));
		}

		private IEnumerable<PropertyInfo> Properties
		{
			get
			{
				return this.GetType()
						   .GetTypeInfo()
						   .DeclaredProperties;
			}
		}
	}
}