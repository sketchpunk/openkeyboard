using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace OpenKeyboard{
	public struct KeyItem{
		public int code;
		public bool isUpLast;
		public string extendCode;

		public KeyItem(int c,bool isLast){ code = c; isUpLast = isLast; extendCode = null; }//func
		public KeyItem(int c){ code = c; isUpLast = false; extendCode = null; }//func
		public KeyItem(string exCode){ code = 0; isUpLast = false; extendCode = exCode; }//func
	}//func

	public abstract class vKeyboard{
		#region Keyboard Code Dictionary - NOTE:Maybe remove and make it part of the XML
		//http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731%28v=vs.85%29.aspx
		private static Dictionary<string,KeyItem> KeyDict = new Dictionary<string,KeyItem>(){
			{"LSHIFT",new KeyItem(0xA0,true)}	,{"RSHIFT",new KeyItem(0xA1,true)}
			,{"LCTRL",new KeyItem(0xA2,true)}	,{"RCTRL",new KeyItem(0xA3,true)}
			,{"PGUP",new KeyItem(0x21,true)}	,{"PGDOWN",new KeyItem(0x22,true)}
			,{"HOME",new KeyItem(0x24,true)}	,{"END",new KeyItem(0x23,true)}
			
			//LWIN - 0x5B, RWIN 0x5C
			//NUMPAD0 - 0x60, NUMPAD1 - 0x61, NUMPAD2 - 0x62, NUMPAD3 - 0x63, NUMPAD4 - 0x64, NUMPAD5 - 0x65, NUMPAD6 - 0x66, NUMPAD7 - 0x67, NUMPAD8 - 0x68, NUMPAD9 - 0x69
			//  + 0xBB  ,0xBC  -0xBD  .0xBE ?0xBF  0xDB  0xDC
			
			,{"OEM2",new KeyItem(0xBF)} // /?
			,{"OEM1",new KeyItem(0xBA)} //;:
			,{"OEM4",new KeyItem(0xDB)} //[{
			,{"OEM6",new KeyItem(0xDD)} //]}
			,{"OEM5",new KeyItem(0xDC)} //|\
			,{"OEM7",new KeyItem(0xDE)} //'"

			,{"COMMA",new KeyItem(0xBC)},{"PERIOD",new KeyItem(0xBE)}
			,{"SPACE",new KeyItem(0x20)},{"ENTER",new KeyItem(0x0D)},{"CAPSLOCK",new KeyItem(0x14)}
			,{"DEL",new KeyItem(0x2E)},{"BACKSPACE",new KeyItem(0x08)},{"TAB",new KeyItem(0x09)}
			
			,{"LEFT",new KeyItem(0x25)},{"UP",new KeyItem(0x26)},{"RIGHT",new KeyItem(0x27)},{"DOWN",new KeyItem(0x28)}

			,{"F1",new KeyItem(0x70)},{"F2",new KeyItem(0x71)},{"F3",new KeyItem(0x72)},{"F4",new KeyItem(0x73)},{"F5",new KeyItem(0x74)}
			,{"F6",new KeyItem(0x75)},{"F7",new KeyItem(0x76)},{"F8",new KeyItem(0x77)},{"F9",new KeyItem(0x78)},{"F10",new KeyItem(0x79)}
			,{"F11",new KeyItem(0x7A)},{"F12",new KeyItem(0x7B)}
			
			,{"0",new KeyItem(0x30)},{"1",new KeyItem(0x31)},{"2",new KeyItem(0x32)},{"3",new KeyItem(0x33)},{"4",new KeyItem(0x34)}
			,{"5",new KeyItem(0x35)},{"6",new KeyItem(0x36)},{"7",new KeyItem(0x37)},{"8",new KeyItem(0x38)},{"9",new KeyItem(0x39)}

			,{"a",new KeyItem(0x41)},{"b",new KeyItem(0x42)},{"c",new KeyItem(0x43)},{"d",new KeyItem(0x44)},{"e",new KeyItem(0x45)}
			,{"f",new KeyItem(0x46)},{"g",new KeyItem(0x47)},{"h",new KeyItem(0x48)},{"i",new KeyItem(0x49)},{"j",new KeyItem(0x4A)}
			,{"k",new KeyItem(0x4B)},{"l",new KeyItem(0x4C)},{"m",new KeyItem(0x4D)},{"n",new KeyItem(0x4E)},{"o",new KeyItem(0x4F)}
			,{"p",new KeyItem(0x50)},{"q",new KeyItem(0x51)},{"r",new KeyItem(0x52)},{"s",new KeyItem(0x53)},{"t",new KeyItem(0x54)}
			,{"u",new KeyItem(0x55)},{"v",new KeyItem(0x56)},{"w",new KeyItem(0x57)},{"x",new KeyItem(0x58)},{"y",new KeyItem(0x59)}
			,{"z",new KeyItem(0x5A)}
			
			,{"{!}",new KeyItem("{!}")},{"{@}",new KeyItem("{@}")},{"{#}",new KeyItem("{#}")},{"{$}",new KeyItem("{$}")},{"{%}",new KeyItem("{%}")}
			,{"{^}",new KeyItem("{^}")},{"{&}",new KeyItem("{&}")},{"{*}",new KeyItem("{*}")},{"{(}",new KeyItem("{(}")},{"{)}",new KeyItem("{)}")}
			,{"{<}",new KeyItem("{<}")},{"{>}",new KeyItem("{>}")}
			
			,{"TILDE",new KeyItem(0xC0)}

			,{"MUTE",new KeyItem(0xAD)},{"VOLUP",new KeyItem(0xAF)},{"VOLDOWN",new KeyItem(0xAE)}
			,{"NEXTRACK",new KeyItem(0xB0)},{"PREVRACK",new KeyItem(0xB1)},{"MEDIASTOP",new KeyItem(0xB2)},{"MEDIAPAUSE",new KeyItem(0xB3)}
		};
		#endregion


		private static bool mIsShiftActive = false;
		public static bool isShiftActive{
			set{mIsShiftActive = value;}
			get{return mIsShiftActive;}
		}//func

		#region Constants
			private const int KEYEVENTF_EXTENDEDKEY = 0x1;
			private const int KEYEVENTF_KEYUP = 0x2;
			private const int KE_KEYUP = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
		#endregion

		#region DLL Imports
			[DllImport("user32.dll", SetLastError = true)]
			private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
		#endregion

		public static void PressKey(int key,bool up) {
			if(up)	keybd_event((byte) key, 0x45, KE_KEYUP, (UIntPtr) 0);
			else	keybd_event((byte) key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr) 0);
		}//func

		public static void OnKeyPress(Object sender, RoutedEventArgs e){
			vButton btn = (vButton)sender;
			List<int> aryLast = new List<int>();

			//if shift and shSendString exists, then send that instead.
			if(isShiftActive && !String.IsNullOrEmpty(btn.shSendString)){
				System.Windows.Forms.SendKeys.SendWait(btn.SendString);
				isShiftActive = false;
				return;
			}//if

			//if SendString exists, then send that instead.
			if(!String.IsNullOrEmpty(btn.SendString)){
				System.Windows.Forms.SendKeys.SendWait(btn.SendString);
				return;
			}//if

			//User clicked shift, So just toggle shift state.
			if(btn.KBKeys.Length == 1 && (btn.KBKeys[0] == "RSHIFT" || btn.KBKeys[0] == "LSHIFT")){
				vKeyboard.isShiftActive = (vKeyboard.isShiftActive)?false:true;
				return;
			}//if

			//Process the array of keys to execute
			KeyItem ki;
			String[] aryKey = btn.KBKeys;

			//did the user press shift before
			if(isShiftActive){
				if(btn.KBShKeys != null){ //if there is a shift code, close shift and use those codes instead.
					aryKey = btn.KBShKeys;
					isShiftActive = false;
				}else PressKey(KeyDict["LSHIFT"].code,false); //Call shift	
			}//if

			for(int i=0; i < aryKey.Length; i++){
				if(! KeyDict.ContainsKey(aryKey[i])) continue;
				ki = KeyDict[aryKey[i]];
	
				if(ki.extendCode == null){
					PressKey(ki.code,false);
					if(ki.isUpLast) aryLast.Add(ki.code);
					else PressKey(ki.code,true);
				}else{
					System.Windows.Forms.SendKeys.SendWait(ki.extendCode);
				}//if
			}//for

			//Some keys must be pressed up last, do it in reverse order
			if(aryLast.Count > 0){
				for(int i=aryLast.Count-1; i >= 0; i--) PressKey(aryLast[i],true);
			}//if

			//user pressed shift before, close key call and reset
			if(isShiftActive){
				PressKey(KeyDict["LSHIFT"].code,true);
				isShiftActive = false;
			}//if
		}//func
	}//cls
}//ns
