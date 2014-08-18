using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Hanno.Services;

namespace Hanno.MVVM.Services
{
	public class MessageBoxAsyncMessageDialog : IAsyncMessageDialog
	{
		public Task Show(CancellationToken ct, string title, string content)
		{
			return Task.Run(() => { MessageBox.Show(content, title); });
		}
	}
}