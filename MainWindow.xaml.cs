using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

#region Notes
//http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731%28v=vs.85%29.aspx
//http://elegantcode.com/2011/03/02/wpf-advanced-jump-lists-using-a-single-instance-application/

//http://fortawesome.github.io/Font-Awesome/cheatsheet/
#endregion

namespace OpenKeyboard
{
    public delegate void RefreshButton(bool toUpper);
    public partial class MainWindow : Window
    {
        private WindowController mWinController = null;
        private ContextMenu mAppMenu;

        public event RefreshButton RefreshButton;
        public MainWindow() { InitializeComponent(); }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            //Set the window style to noactivate.
            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(helper.Handle, GWL_EXSTYLE,
                GetWindowLong(helper.Handle, GWL_EXSTYLE) | WS_EX_NOACTIVATE);
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_NOACTIVATE = 0x08000000;

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        #region Window Events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            mWinController = new WindowController(this);
            mWinController.DisableFocus(); //When window loses focus, prevent windows from getting focus back automatically when clicking anything

            //Check which keyboard profile to load in.
            string[] args = Environment.GetCommandLineArgs();
            string layoutName = (args.Length > 1) ? args[1] : "Default";

            if (!vLayout.Load(layoutName, mainContainer, this))
            {
                MessageBox.Show("Error loading layout:" + layoutName);
                Close();
                return;
            }//if

            if (Properties.Settings.Default.ContextMenu)
            {
                mAppMenu = new ContextMenu();

                CreateContextMenu();

                LoadLayoutList();
            }
        }//func

        bool inDrag = false;
        Point anchorPoint;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            anchorPoint = PointToScreen(e.GetPosition(this));
            inDrag = true;
            CaptureMouse();
            e.Handled = true;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (inDrag)
            {
                Point currentPoint = PointToScreen(e.GetPosition(this));
                Left = Left + currentPoint.X - anchorPoint.X;
                Top = Top + currentPoint.Y - anchorPoint.Y;
                anchorPoint = currentPoint;
            }
            
            RefreshButton.Invoke(vKeyboard.isShiftActive || System.Windows.Forms.Control.IsKeyLocked(System.Windows.Forms.Keys.CapsLock));
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (inDrag)
            {
                ReleaseMouseCapture();
                inDrag = false;
                e.Handled = true;
            }
        }

        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            if (mAppMenu != null)
                mAppMenu.IsOpen = true; //Show context menu if user right clicks anywhere in the window.
        }//func

        /*
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e){
            base.OnMouseLeftButtonDown(e);
            this.DragMove(); //Allow user to drag window around when they right click anywhere in the window.
        }//func
        */

        //private void Window_Activated(object sender, EventArgs e){}
        //private void Window_MouseDown(object sender, MouseButtonEventArgs e) { }//func
        //private void Window_Deactivated(object sender, EventArgs e){}//func
        #endregion

        #region Menu Events
        private void OpacityMenu_Click(object sender, RoutedEventArgs e)
        {
            double tag = double.Parse((sender as MenuItem).Tag.ToString());
            Opacity = tag / 100;
        }//func

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string tag = (sender as MenuItem).Tag.ToString();

            switch (tag)
            {
                case "{CMD_EXIT}": Close(); return;
                default:
                    vLayout.Load(tag, mainContainer, this);
                    break;
            }//switch
        }//func
        #endregion

        #region Loaders
        private void CreateContextMenu()
        {
            mAppMenu.PlacementTarget = this;

            MenuItem mItem = new MenuItem() { Header = "Exit", Tag = "{CMD_EXIT}" };
            mItem.Click += MenuItem_Click;
            mAppMenu.Items.Add(mItem);
            mAppMenu.Items.Add(new Separator());

            MenuItem itm;
            mItem = new MenuItem() { Header = "Opacity" };
            for (int i = 100; i >= 20; i -= 10)
            {
                itm = new MenuItem() { Header = i.ToString() + "%", Tag = i.ToString() };
                itm.Click += OpacityMenu_Click;
                mItem.Items.Add(itm);
            }//for

            mAppMenu.Items.Add(mItem);
            mAppMenu.Items.Add(new Separator());
        }//func

        private void LoadLayoutList()
        {
            string[] ary = vLayout.GetLayoutList();
            if (ary.Length == 0) return;

            int i;
            MenuItem mItem;
            var jumpList = new vJumpList() { CategoryName = "Available Layouts", AppPath = Assembly.GetEntryAssembly().Location };

            for (i = 0; i < ary.Length; i++)
            {
                //Add Item to context menu
                mItem = new MenuItem() { Header = ary[i], Tag = ary[i] };
                mItem.Click += MenuItem_Click;
                mAppMenu.Items.Add(mItem);

                //Create JumpList Item
                jumpList.AddTask(ary[i], "Load this Keyboard Layout", ary[i]);
            }//for

            jumpList.Apply();
        }//for
        #endregion
    }//cls
}//ns
