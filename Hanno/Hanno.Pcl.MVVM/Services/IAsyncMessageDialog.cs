using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Services
{
	public interface IAsyncMessageDialog
	{
		Task Show(CancellationToken ct, string title, string content);
	}
}
