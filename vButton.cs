using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;

namespace OpenKeyboard{
	public class vButton : Button{
		public string[] KBKeys = null;
		public string[] KBShKeys = null;
		public string SendString = "";

		// Dependency Property
		public static DependencyProperty ShiftTextProperty = DependencyProperty.Register("ShiftText", typeof(string), typeof(TextBlock), new FrameworkPropertyMetadata(""));
		public string ShiftText{
			get { return (string)this.GetValue(ShiftTextProperty); }
			set { this.SetValue(ShiftTextProperty, value); } 
		}//prop
	}//cls
}//ns
