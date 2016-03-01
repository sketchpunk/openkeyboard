using System;
using System.Windows;
using System.Windows.Threading;

namespace OpenKeyboard{
	public partial class App : Application{
        public void App_Startup(object sender, StartupEventArgs e) {
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
        }//event

        public void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            if(e.Exception.InnerException != null) vLogger.Exception("app.UnhandledException", e.Exception.InnerException);
            else vLogger.Exception("app.UnhandledException", e.Exception);

            e.Handled = true;
            Application.Current.Shutdown();
        }//func
    }//cls
}//ns
