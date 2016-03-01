using System;
using System.IO;

namespace OpenKeyboard {
    public abstract class vLogger{
        public static void Exception(string errCode, Exception e) { Exception(errCode, e, ""); }
        public static void Exception(string errCode, Exception e, string custom) {
            try {
                int i = 0;
                do {
                    Console.WriteLine("ERROR " + errCode + " : " + e.Message + " : " + e.StackTrace + "\n\r" + custom);
                    FileAppend(String.Format("<err code=\"{0}\" dt=\"{1}\" ver=\"{5}\"><msg>{2}</msg><trace>{3}</trace><custom>{4}</custom></err>"
                        , (e.InnerException != null || i > 0) ? errCode + "." + i.ToString() : errCode
                        , GetDate()
                        , EscapeXML(e.Message)
                        , EscapeXML(e.StackTrace)
                        , EscapeXML(custom)
                        , System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
                    ));
                    i++; e = e.InnerException;
                } while(e != null);
            } catch(Exception ex) {
                System.Windows.MessageBox.Show(String.Format("ERROR ON SAVING ERROR : <err code=\"{0}\" dt=\"{1}\"><msg>{2}</msg><trace>{3}</trace><custom>{4}</custom></err>"
                    , errCode
                    , GetDate()
                    , EscapeXML(e.Message)
                    , EscapeXML(e.StackTrace)
                    , EscapeXML(custom)
                ));
            }//try
        }//func

        #region Support Function
            public static string GetLogFilePath() {
                string rtn = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
                if(!rtn.EndsWith("\\")) rtn += "\\";
                return rtn + "error_log.txt";
            }//func

            public static string GetDate() { return DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"); }
            public static string EscapeXML(string txt) { return System.Security.SecurityElement.Escape(txt); }

            public static Boolean FileAppend(string txt) { return FileAppend(txt, 1); }
            public static Boolean FileAppend(string txt, int step) {
                string path = GetLogFilePath();
                StreamWriter sw = null;
                Boolean stat = true;
            Console.WriteLine(path);
                try {
                    sw = new StreamWriter(path, true);
                    sw.WriteLine(txt);
                } catch(Exception ex) {
                    //Error saving to Progress files, Try to save it to AppData before we quit.
                    if(step <= 1) stat = FileAppend(txt, 2);
                    else {
                        System.Windows.MessageBox.Show("ERROR ON SAVING ERROR : " + txt + " :: " + ex.Message);
                        stat = false;
                    }//if
                } finally {
                    if(sw != null) { sw.Close(); sw = null; }
                }//try

                return stat;
            }//func
        #endregion
    }//cls
}//ns