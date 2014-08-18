using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Hanno.CqrsInfrastructure;

namespace TestWindowsPhone
{
	public class BingQuery : AsyncQueryBase<string>
	{
		public BingQuery(CancellationToken ct) : base(ct)
		{
		}

		public string Query { get; set; }
	}
}
