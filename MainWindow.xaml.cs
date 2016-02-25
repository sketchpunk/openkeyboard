using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Reflection;
using System.Windows.Interop;

#region Notes
//http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731%28v=vs.85%29.aspx
//http://elegantcode.com/2011/03/02/wpf-advanced-jump-lists-using-a-single-instance-application/
#endregion

namespace OpenKeyboard{

	public partial class MainWindow : Window{
        private WindowController mWinController = null;
        private ContextMenu mAppMenu = new ContextMenu();

		public MainWindow(){ InitializeComponent(); }

        #region Window Events
			private void Window_Loaded(object sender, RoutedEventArgs e){
                mWinController = new WindowController(this);
                mWinController.DisableFocus(); //When window loses focus, prevent windows from getting focus back automaticly when clicking anything

				//Check which keyboard profile to load in.
				string[] args = Environment.GetCommandLineArgs();
				if(args.Length > 1) vLayout.Load(args[1],mainContainer,this);
				else vLayout.Load("Blender",mainContainer,this);

                CreateContextMenu();
                LoadLayoutList();

                this.MouseLeftButtonDown += MainWindow_LeftButtonDown;
            }//func

            private void MainWindow_LeftButtonDown(object sender, MouseButtonEventArgs e) {
                this.DragMove(); //Allow user to drag window around when they right click anywhere in the window.
                e.Handled = true;
            }//func
            
            protected override void OnMouseRightButtonDown(MouseButtonEventArgs e) {
                base.OnMouseRightButtonDown(e);
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
        private void OpacityMenu_Click(object sender, RoutedEventArgs e) {
                double tag = double.Parse((sender as MenuItem).Tag.ToString());
                this.Opacity = (tag / 100);
            }//func

            private void MenuItem_Click(object sender, RoutedEventArgs e) {
                string tag = (sender as MenuItem).Tag.ToString();

                switch(tag) {
                    case "{CMD_EXIT}": this.Close(); return;
                    default:
                        vLayout.Load(tag, mainContainer, this);
                        break;
                }//switch
            }//func
        #endregion

        #region Loaders
            private void CreateContextMenu() {
                mAppMenu.PlacementTarget = this;

                MenuItem mItem = new MenuItem() { Header = "Exit", Tag = "{CMD_EXIT}" };
                mItem.Click += MenuItem_Click;
                mAppMenu.Items.Add(mItem);
                mAppMenu.Items.Add(new Separator());

                MenuItem itm;
                mItem = new MenuItem() { Header = "Opacity" };
                for(int i = 100; i >= 20; i -= 10){
                    itm = new MenuItem() { Header = i.ToString() + "%", Tag = i.ToString() };
                    itm.Click += OpacityMenu_Click;
                    mItem.Items.Add(itm);
                }//for

                mAppMenu.Items.Add(mItem);
                mAppMenu.Items.Add(new Separator());
            }//func

            private void LoadLayoutList() {
                string[] ary = vLayout.GetLayoutList();
                if(ary.Length == 0) return;

                int i;
                MenuItem mItem;
                var jumpList = new vJumpList() { CategoryName = "Available Layouts", AppPath = Assembly.GetEntryAssembly().Location };

                for(i = 0; i < ary.Length; i++) {
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
