using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hanno.Services;

namespace Hanno.MVVM.Services
{
	public class StringResources : IStringResources
	{
		public string GetResource(string key)
		{
			return key;
		}
	}
}
