using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hanno.Patterns;

namespace Hanno
{
	public interface IInitializable
	{
		Task Initialize(CancellationToken ct);
	}


}