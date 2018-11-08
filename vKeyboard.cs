using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Timers;

namespace OpenKeyboard {
    public struct KeyboardCommand {
        public string[] KBKeys;
        public string[] KBShKeys;
        public string SendString;
        public string shSendString;
    }//struct

    public struct KeyItem {
        public int code;
        public bool isUpLast;
        public string extendCode;

        public KeyItem(int c, bool isLast) { code = c; isUpLast = isLast; extendCode = null; }//func
        public KeyItem(int c) { code = c; isUpLast = false; extendCode = null; }//func
        public KeyItem(string exCode) { code = 0; isUpLast = false; extendCode = exCode; }//func
    }//func

    public abstract class vKeyboard {
        #region Keyboard Code Dictionary - NOTE:Maybe remove and make it part of the XML
        //http://msdn.microsoft.com/en-us/library/windows/desktop/dd375731%28v=vs.85%29.aspx
        //http://www.kbdedit.com/manual/low_level_vk_list.html
        private static Dictionary<string, KeyItem> KeyDict = new Dictionary<string, KeyItem>(){
            {"LSHIFT",new KeyItem(0xA0,true)}   ,{"RSHIFT",new KeyItem(0xA1,true)}  ,{"SHIFT",new KeyItem(0x10,true)}
            ,{"ALT",new KeyItem(0x12,true)}     ,{"LALT",new KeyItem(0xA4,true)}    ,{"RALT",new KeyItem(0xA5,true)}
            ,{"LCTRL",new KeyItem(0xA2,true)}   ,{"RCTRL",new KeyItem(0xA3,true)}
            ,{"PGUP",new KeyItem(0x21,true)}    ,{"PGDOWN",new KeyItem(0x22,true)}
            ,{"HOME",new KeyItem(0x24,true)}    ,{"END",new KeyItem(0x23,true)}
            ,{"LWIN",new KeyItem(0x5B,true)}    ,{"RWIN",new KeyItem(0x5C,true)}
			
            ,{"NUM0",new KeyItem(0x60)}
            ,{"NUM1",new KeyItem(0x61)}
            ,{"NUM2",new KeyItem(0x62)}
            ,{"NUM3",new KeyItem(0x63)}
            ,{"NUM4",new KeyItem(0x64)}
            ,{"NUM5",new KeyItem(0x65)}
            ,{"NUM6",new KeyItem(0x66)}
            ,{"NUM7",new KeyItem(0x67)}
            ,{"NUM8",new KeyItem(0x68)}
            ,{"NUM9",new KeyItem(0x69)}
            ,{"DECIMAL",new KeyItem(0x6E)}
                

            ,{"OEM2",new KeyItem(0xBF)} // ù §
			,{"OEM1",new KeyItem(0xBA)} // è é [
			,{"OEM4",new KeyItem(0xDB)} // ' ?
			,{"OEM6",new KeyItem(0xDD)} // ì ^
			,{"OEM5",new KeyItem(0xDC)} // | \
			,{"OEM7",new KeyItem(0xDE)} // à ° #
            ,{"OEMMINUS",new KeyItem(0xBD)} // - _
            ,{"OEMPERIOD",new KeyItem(0xBE)} // . :
            ,{"OEM3",new KeyItem(0xC0)} // ò ç @
            ,{"OEMPLUS",new KeyItem(0xBb)} // + * ]
            ,{"OEM102",new KeyItem(0xE2)} // < >

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
            ,{"{<}",new KeyItem("{<}")},{"{>}",new KeyItem("{>}")},{"{\"}",new KeyItem("{\"}")},{"{£}",new KeyItem("{£}")},{"{/}",new KeyItem("{/}")}
            ,{"{=}",new KeyItem("{=}")},{"{€}",new KeyItem("{€}")},{"{[}",new KeyItem("{[}")},{"{]}",new KeyItem("{]}")}

            ,{"TILDE",new KeyItem(0xC0)}

            ,{"MUTE",new KeyItem(0xAD)},{"VOLUP",new KeyItem(0xAF)},{"VOLDOWN",new KeyItem(0xAE)}
            ,{"NEXTRACK",new KeyItem(0xB0)},{"PREVRACK",new KeyItem(0xB1)},{"MEDIASTOP",new KeyItem(0xB2)},{"MEDIAPAUSE",new KeyItem(0xB3)}
        };
        #endregion

        private static bool mIsShiftActive = false;
        public static bool isShiftActive {
            set { mIsShiftActive = value; }
            get { return mIsShiftActive; }
        }//func

        #region Constants
        private const int KEYEVENTF_EXTENDEDKEY = 0x1;
        private const int KEYEVENTF_KEYUP = 0x2;
        private const int KEYEVENTF_KEYDOWN = 0x0;
        private const int KE_KEYUP = KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP;
        #endregion

        #region DLL Imports
        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);
        #endregion

        public static void PressKey(int key, bool up) {
            //if(up) keybd_event((byte)key, 0x45, KE_KEYUP, (UIntPtr)0);
            //else keybd_event((byte)key, 0x45, KEYEVENTF_EXTENDEDKEY, (UIntPtr)0);

            uint scanCode = MapVirtualKey((uint)key, 0);

            if(up) keybd_event((byte)key, (byte)scanCode, KEYEVENTF_KEYUP, (UIntPtr)0);
            else keybd_event((byte)key, (byte)scanCode, KEYEVENTF_KEYDOWN, (UIntPtr)0);
        }//func

        public static void ProcessCommand(KeyboardCommand kbCmd) {
            List<int> aryLast = new List<int>();

            //........................................................
            //if shift and shSendString exists, then send that instead.
            if(isShiftActive && !String.IsNullOrEmpty(kbCmd.shSendString)) {
                System.Windows.Forms.SendKeys.SendWait(kbCmd.SendString);
                isShiftActive = false;
                return;
            }//if

            //........................................................
            //if SendString exists, then send that instead.
            if(!String.IsNullOrEmpty(kbCmd.SendString)) {
                System.Windows.Forms.SendKeys.SendWait(kbCmd.SendString);
                return;
            }//if

            //........................................................
            //User clicked shift, So just toggle shift state.
            if(kbCmd.KBKeys != null && kbCmd.KBKeys.Length == 1 && (kbCmd.KBKeys[0] == "RSHIFT" || kbCmd.KBKeys[0] == "LSHIFT")) {
                vKeyboard.isShiftActive = (vKeyboard.isShiftActive) ? false : true;
                return;
            }//if

            //Process the array of keys to execute
            KeyItem ki;
            String[] aryKey = kbCmd.KBKeys;

            //did the user press shift before
            if(isShiftActive) {
                if(kbCmd.KBShKeys != null) { //if there is a shift code, close shift and use those codes instead.
                    aryKey = kbCmd.KBShKeys;
                    isShiftActive = false;
                } else PressKey(KeyDict["LSHIFT"].code, false); //Call shift	
            }//if

            if(aryKey != null) {
                for(int i = 0; i < aryKey.Length; i++) {
                    if(!KeyDict.ContainsKey(aryKey[i])) continue;
                    ki = KeyDict[aryKey[i]];

                    if(ki.extendCode == null) {
                        PressKey(ki.code, false);
         
                        if(ki.isUpLast){ aryLast.Add(ki.code); }
                        else PressKey(ki.code, true);
                    } else {
                        System.Windows.Forms.SendKeys.SendWait(ki.extendCode);
                    }//if
                }//for
            }//if

            //Some keys must be pressed up last, do it in reverse order
            if(aryLast.Count > 0) {
                for(int i = aryLast.Count - 1; i >= 0; i--){
                    PressKey(aryLast[i], true);
                }//if
            }//if

            //user pressed shift before, close key call and reset
            if(isShiftActive) {
                PressKey(KeyDict["LSHIFT"].code, true);
                isShiftActive = false;
            }//if
        }//func
    }//cls

    public abstract class KeyLoopHandler{
        private static KeyboardCommand mKBCommand;
        private static Timer mTimer = null;
        private static bool mIsTimerOn = false;

        public static void EndKeypress(){ StopTimer(); }//func
        public static void BeginKeypress(KeyboardCommand cmd){
            if(mIsTimerOn) return;
            vKeyboard.ProcessCommand(cmd);

            mKBCommand = cmd;
            StartTimer();
        }//for

        private static void StopTimer() { mTimer.Stop(); mIsTimerOn = false; }
        private static void StartTimer() {
            if(mTimer == null) {
                mTimer = new Timer();
                mTimer.Interval = 200;
                mTimer.Elapsed += new ElapsedEventHandler(onTick);
            } else if(mIsTimerOn) return;

            mTimer.Start();
            mIsTimerOn = true;
        }//func
        
        private static void onTick(object sender, ElapsedEventArgs e) {vKeyboard.ProcessCommand(mKBCommand); }
    }//cls
}//ns
