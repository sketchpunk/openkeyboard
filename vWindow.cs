using System;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace OpenKeyboard{
	public abstract class vWindow{
		#region Constants
			public const int WS_EX_NOACTIVATE = 0x08000000;
			public const int GWL_EXSTYLE = -20;
		#endregion

		#region DLL Imports
			[DllImport("user32", SetLastError = true)]
			public extern static int GetWindowLong(IntPtr hwnd, int nIndex);

			[DllImport("user32", SetLastError = true)]
			public extern static int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewValue);
		#endregion
	}//cls
}//ns
