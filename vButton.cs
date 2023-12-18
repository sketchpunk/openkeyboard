using System;

using System.Windows;
using System.Windows.Controls;

namespace OpenKeyboard
{
    public class vButton : Button
    {
        public KeyboardCommand KBCommand;

        // Dependency Property
        public static DependencyProperty ShiftTextProperty = DependencyProperty.Register("ShiftText", typeof(string), typeof(TextBlock), new FrameworkPropertyMetadata(""));
        public string ShiftText
        {
            get { return (string)this.GetValue(ShiftTextProperty); }
            set { this.SetValue(ShiftTextProperty, value); }
        }//prop

        public string Title
        {
            set
            {
                if (value.StartsWith("\\u")) parseUnicode(value);
                else Content = value;
            }
        }//prop

        private void parseUnicode(string txt)
        {
            int pos = 0;
            string tmp = "", final = "";

            //Check if only one unicode escaped character in the string.
            if (txt.Length == 6)
            {
                Content = (char)Int32.Parse(txt.Substring(2), System.Globalization.NumberStyles.HexNumber);
                return;
            }//if

            //More then one possible unicode characters
            while (pos < txt.Length)
            {
                //If not unicode escaped, add to final
                if (txt[pos] != '\\') { final += txt[pos]; pos++; }
                else
                { //unicode escaped, Parse it.
                    tmp = txt.Substring(pos + 2, 4);
                    final += (char)Int32.Parse(tmp, System.Globalization.NumberStyles.HexNumber);
                    pos += 6;
                }//if
            }//while

            Content = final;
        }//func

        public void RefreshButton(bool toUpper)
        {
            var txt = Content as string;

            if (txt.Length == 1)
                Content = toUpper ? txt.ToUpper() : txt.ToLower();
        }
    }//cls
}//ns
