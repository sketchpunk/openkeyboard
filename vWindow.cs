using System;
using System.Windows.Interop;
using System.Runtime.InteropServices;

using System.Windows;

namespace OpenKeyboard{
	public abstract class vWindow{
        #region Constants
            public const int WS_EX_TRANSPARENT = 0x00000020;
            public const int WS_EX_NOACTIVATE = 0x08000000;
			public const int GWL_EXSTYLE = -20;
			public const int WM_NCLBUTTONDOWN = 0x00A1;
			public const int HT_CAPTION = 0x0002;

			public const int WM_SYSCOMMAND = 0x112;
			public const int MF_BYPOSITION = 0x400;
			public const int MF_SEPARATOR = 0x800;

            public const uint TPM_LEFTALIGN = 0x0000;
            public const uint TPM_RETURNCMD = 0x0100;
        #endregion

        #region DLL Imports
            [DllImport("user32", SetLastError = true)]
			public static extern int GetWindowLong(IntPtr hwnd, int nIndex);

			[DllImport("user32", SetLastError = true)]
			public static extern int SetWindowLong(IntPtr hwnd, int nIndex, int dwNewValue);

            //................................
			[DllImport("user32", SetLastError = true)]
			public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
			
			[DllImport("user32", SetLastError = true)]
			public static extern bool InsertMenu(IntPtr hMenu,Int32 wPosition, Int32 wFlags, Int32 wIDNewItem,string lpNewItem);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool AppendMenu(IntPtr hMenu, int uFlags, int uIDNewItem, string lpNewItem);

            [DllImport("user32.dll")]
            public static extern int TrackPopupMenuEx(IntPtr hmenu, uint fuFlags, int x, int y, IntPtr hwnd, IntPtr lptpm);

            //................................
            [DllImport("user32.dll")]
            public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        #endregion
    }//cls

    public class WindowController{
        private WindowInteropHelper mHelper = null;

        public WindowController(Window win){ mHelper = new WindowInteropHelper(win); }

        #region Focus Control
            /*-----------------------------------------
            This allows the window to not gain focus when it is clicked on.
            -----------------------------------------*/
            private int mState_Focus = 0, mState_Unfocus = 0;

            public void LoadFocusControl(){
                mState_Focus = vWindow.GetWindowLong(mHelper.Handle, vWindow.GWL_EXSTYLE);
                mState_Unfocus = mState_Focus | vWindow.WS_EX_NOACTIVATE;
            }//func

            public void AllowFocus(bool isAllowed){ vWindow.SetWindowLong(mHelper.Handle, vWindow.GWL_EXSTYLE, ((isAllowed)?mState_Focus : mState_Unfocus)); }//func

            public void DisableFocus(){
                int style = vWindow.GetWindowLong(mHelper.Handle, vWindow.GWL_EXSTYLE);
                vWindow.SetWindowLong(mHelper.Handle, vWindow.GWL_EXSTYLE, style | vWindow.WS_EX_NOACTIVATE);
            }//func
        #endregion

        #region System Control Menu
            IntPtr mMenuHandle = IntPtr.Zero;
            public void LoadSystemMenu(){ mMenuHandle = vWindow.GetSystemMenu(mHelper.Handle, false); }

            public void ShowSystemMenu(int x, int y){
                int cmd = vWindow.TrackPopupMenuEx(mMenuHandle, vWindow.TPM_LEFTALIGN | vWindow.TPM_RETURNCMD, x, y, mHelper.Handle, IntPtr.Zero);
                if(cmd == 0) return;
                vWindow.PostMessage(mHelper.Handle, vWindow.WM_SYSCOMMAND, new IntPtr(cmd), IntPtr.Zero);
            }//func

            public void InsertMenu(Int32 pos, Int32 itmID, string menuTitle){ vWindow.InsertMenu(mMenuHandle, pos, vWindow.MF_BYPOSITION, itmID, menuTitle); }//func
            public void InsertMenuSep(Int32 pos, Int32 itmID) { vWindow.InsertMenu(mMenuHandle, pos, vWindow.MF_SEPARATOR | vWindow.MF_BYPOSITION, itmID, null); }//func

            /* FOLLOWING CODE IS HOW TO RESPOND TO THE MENU ITEMS CLICKED, Implement in Window
                GET Messages, Setup intercept
                protected override void OnSourceInitialized(EventArgs e) {
                    base.OnSourceInitialized(e);
                    HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
                    source.AddHook(WndProc);
                }//func

                private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
                    switch(msg) {
                        case vWindow.WM_SYSCOMMAND:
                            int wparam = wParam.ToInt32();
                            if(this.kbLayouts.ContainsKey(wparam)) vLayout.Load(this.kbLayouts[wparam], mainContainer, this);
                            break;
                    }//switch 

                    return IntPtr.Zero;
                }//func
            */
        #endregion
    }//func
}//ns
