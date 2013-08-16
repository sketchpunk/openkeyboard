using System;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace OpenKeyboard{
	public abstract class vWindow{
		#region Constants
			public const int WS_EX_NOACTIVATE = 0x08000000;
			public const int GWL_EXSTYLE = -20;
			public const int WM_NCLBUTTONDOWN = 0x00A1;
			public const int HT_CAPTION = 0x0002;

			public const int WM_SYSCOMMAND = 0x112;
			public const int MF_BYPOSITION = 0x400;
			public const int MF_SEPARATOR = 0x800;
		#endregion

		#region DLL Imports
			[DllImport("user32", SetLastError = true)]
			public extern static int GetWindowLong(IntPtr hwnd, int nIndex);

			[DllImport("user32", SetLastError = true)]
			public extern static int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewValue);

			[DllImport("user32", SetLastError = true)]
			public extern static IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
			
			[DllImport("user32", SetLastError = true)]
			public extern static bool InsertMenu(IntPtr hMenu,Int32 wPosition, Int32 wFlags, Int32 wIDNewItem,string lpNewItem);    
		#endregion
	}//cls
}//ns
