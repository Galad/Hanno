using System;

namespace Hanno.Xaml.Controls
{
	[Flags]
	public enum VirtualKeyModifiers2
	{
		// Summary:
		//     No virtual key modifier.
		None = 0,
		//
		// Summary:
		//     The Ctrl (control) virtual key.
		Control = 1,
		//
		// Summary:
		//     The Menu virtual key.
		Menu = 2,
		//
		// Summary:
		//     The Shift virtual key.
		Shift = 4,
		//
		// Summary:
		//     The Windows virtual key.
		Windows = 8,
	}
}