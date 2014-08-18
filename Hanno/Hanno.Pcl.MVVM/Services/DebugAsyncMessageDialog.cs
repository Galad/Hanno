using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hanno.Services
{
	public class DebugAsyncMessageDialog : IAsyncMessageDialog
	{
		private readonly IAsyncMessageDialog _innerMessageDialog;

		public DebugAsyncMessageDialog(IAsyncMessageDialog innerMessageDialog)
		{
			if (innerMessageDialog == null) throw new ArgumentNullException("innerMessageDialog");
			_innerMessageDialog = innerMessageDialog;
		}

		public async Task Show(CancellationToken ct, string title, string content)
		{
			Debug.WriteLine("Display async message dialog.\nTitle : {0}\nContent:{1}");
			await _innerMessageDialog.Show(ct, title, content);
		}
	}
}
