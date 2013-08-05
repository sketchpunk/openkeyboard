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
		
		public MainWindow(){ InitializeComponent(); }

		#region Create JumpList
			private void CreateJumpList(){
				string[] ary = vLayout.GetLayoutList();
				if(ary.Length == 0) return;
			
				JumpList jumpList = new JumpList();
				JumpList.SetJumpList(Application.Current, jumpList);

				for(int i=0; i < ary.Length; i++){
					JumpTask jTask = new JumpTask();
					jTask.Title = System.IO.Path.GetFileNameWithoutExtension(ary[i]);
					jTask.Description = "Load this Keyboard Layout";
					jTask.ApplicationPath = Assembly.GetEntryAssembly().Location;
					jTask.Arguments = System.IO.Path.GetFileName(ary[i]);
				
					jumpList.JumpItems.Add(jTask);
				}//for
				jumpList.Apply();
			}//func
		#endregion

		#region Window Events
		private void Window_Activated(object sender, EventArgs e){}
			private void Window_MouseDown(object sender, MouseButtonEventArgs e){}//func

			private void Window_Loaded(object sender, RoutedEventArgs e){
				this.wih = new WindowInteropHelper(this);

				this.focusStyle = vWindow.GetWindowLong(this.wih.Handle,vWindow.GWL_EXSTYLE);
				this.unfocusStyle = this.focusStyle | vWindow.WS_EX_NOACTIVATE;

				string[] args = Environment.GetCommandLineArgs();
				if(args.Length > 1) vLayout.Load(args[1],mainContainer,this);
				else vLayout.Load("Default.xml",mainContainer,this);
			
				CreateJumpList();
			}//func

			private void Window_Deactivated(object sender, EventArgs e){
				//When window loses focus, prevent windows from getting focus back automaticly when clicking anything
				vWindow.SetWindowLong(this.wih.Handle, vWindow.GWL_EXSTYLE, this.unfocusStyle);
			}//func

			/*GET Messages, Setup intercept from OS to control the focus of the window*/
			protected override void OnSourceInitialized(EventArgs e){
				base.OnSourceInitialized(e);
				HwndSource source = PresentationSource.FromVisual(this) as HwndSource;
				source.AddHook(WndProc);
			}//func

			private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled){
				if(msg == 0x00A1){ //WM_NCLBUTTONDOWN 
					Console.WriteLine(wParam);
					if(wParam.ToInt64() == 0x0002){ //HT_CAPTION , only set focus if clicking on the title bar.
						Console.WriteLine(wParam);
						Console.WriteLine(msg);
						vWindow.SetWindowLong(this.wih.Handle, vWindow.GWL_EXSTYLE, this.focusStyle);
						this.Activate();
					}//if
				}//if
				return IntPtr.Zero;
			}//func
		#endregion		 
	}//cls
}//ns
