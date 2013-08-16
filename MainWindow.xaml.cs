using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using System.Reflection;

using System.Windows.Interop;
using System.Runtime.InteropServices;


//Helpful sites
//http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731%28v=vs.85%29.aspx
//http://elegantcode.com/2011/03/02/wpf-advanced-jump-lists-using-a-single-instance-application/

namespace OpenKeyboard{

	public partial class MainWindow : Window{
		private WindowInteropHelper wih = null;
		private int focusStyle = 0;
		private int unfocusStyle = 0;
		private Dictionary<int,string> kbLayouts = new Dictionary<int,string>();
		
		public MainWindow(){ InitializeComponent(); }

		#region Create JumpList
			private void CreateKBList(){
				string[] ary = vLayout.GetLayoutList();
				if(ary.Length == 0) return;

				//................................................
				//Window Context Menu
				IntPtr hMenu = vWindow.GetSystemMenu(this.wih.Handle, false);
				
				//Win8 Window Jumplist
				JumpList jumpList = new JumpList();
				JumpList.SetJumpList(Application.Current, jumpList);

				//................................................
				//Loop through file
				int i;
				string kbName = "",fileName = "", asmPath = Assembly.GetEntryAssembly().Location;
				
				for(i=0; i < ary.Length; i++){
					//Save command handler id for menu and path for later system messaging listening
					fileName = System.IO.Path.GetFileName(ary[i]);
					kbName = System.IO.Path.GetFileNameWithoutExtension(ary[i]);
					this.kbLayouts.Add(1000+i+1,fileName);

					//Create items in context menu
					vWindow.InsertMenu(hMenu, i, vWindow.MF_BYPOSITION, 1000+i+1, kbName);

					//Create jumplist item
					JumpTask jTask = new JumpTask();
					jTask.Title = kbName;
					jTask.Description = "Load this Keyboard Layout";
					jTask.ApplicationPath = asmPath;
					jTask.Arguments = fileName;
					jumpList.JumpItems.Add(jTask);
				}//for

				//................................................
				//Create Seperator
				vWindow.InsertMenu(hMenu,i,vWindow.MF_SEPARATOR | vWindow.MF_BYPOSITION,1000+i+1,null);

				//Apply Jump Items
				jumpList.Apply();
			}//for
		#endregion

		#region Window Events
			private void Window_Activated(object sender, EventArgs e){}
			private void Window_MouseDown(object sender, MouseButtonEventArgs e){}//func

			private void Window_Loaded(object sender, RoutedEventArgs e){
				//Setup focus styles
				this.wih = new WindowInteropHelper(this);
				this.focusStyle = vWindow.GetWindowLong(this.wih.Handle,vWindow.GWL_EXSTYLE);
				this.unfocusStyle = this.focusStyle | vWindow.WS_EX_NOACTIVATE;

				//Check which keyboard profile to load in.
				string[] args = Environment.GetCommandLineArgs();
				if(args.Length > 1) vLayout.Load(args[1],mainContainer,this);
				else vLayout.Load("Default.xml",mainContainer,this);
			
				CreateKBList();
			}//func

			private void Window_Deactivated(object sender, EventArgs e){
				//When window loses focus, prevent windows from getting focus back automaticly when clicking anything
				vWindow.SetWindowLong(this.wih.Handle, vWindow.GWL_EXSTYLE, this.unfocusStyle);
			}//func
		#endregion
	 
		#region Window Messaging
			/*GET Messages, Setup intercept from OS to control the focus of the window*/
			protected override void OnSourceInitialized(EventArgs e){
				base.OnSourceInitialized(e);
				HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
				source.AddHook(WndProc);
			}//func

			private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled){
				switch(msg){
					//............................................................
					case vWindow.WM_NCLBUTTONDOWN:
						if(wParam.ToInt64() == vWindow.HT_CAPTION){ //only set focus if clicking on the title bar.
							vWindow.SetWindowLong(this.wih.Handle, vWindow.GWL_EXSTYLE, this.focusStyle);
							this.Activate();
						}//if
					break;
					
					//............................................................
					case vWindow.WM_SYSCOMMAND:
						int wparam = wParam.ToInt32();
						if(this.kbLayouts.ContainsKey(wparam)) vLayout.Load(this.kbLayouts[wparam],mainContainer,this);
					break;
				}//switch
				return IntPtr.Zero;
			}//func
		#endregion
	}//cls
}//ns
