using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace ABSoftware
{
    public class Keyboard
    {
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int vKeys);

        [DllImport("USER32.dll")]
        public static extern short GetKeyState(int nVirtKey);

        [DllImport("user32.dll")]
        static extern void keybd_event(int nVirtKey, byte bScan, uint dwFlags, int dwExtraInfo);

        static List<int> KEYS_CONSTS { get { return ScanKeysConsts(); } }

        static bool[] LastKeysStamp;

        public struct Keys
        {
            public const int VK_LBUTTON = 0x01;
            public const int VK_RBUTTON = 0x02;
            public const int VK_CANCEL = 0x03;
            public const int VK_MBUTTON = 0x04;
            public const int VK_XBUTTON1 = 0x05;
            public const int VK_XBUTTON2 = 0x06;
            public const int VK_BACK = 0x08;
            public const int VK_TAB = 0x09;
            public const int VK_CLEAR = 0x0C;
            public const int VK_RETURN = 0x0D;
            public const int VK_SHIFT = 0x10;
            public const int VK_CONTROL = 0x11;
            public const int VK_MENU = 0x12;
            public const int VK_PAUSE = 0x13;
            public const int VK_CAPITAL = 0x14;
            public const int VK_KANA = 0x15;
            public const int VK_HANGUEL = 0x15;
            public const int VK_HANGUL = 0x15;
            public const int VK_IME_ON = 0x16;
            public const int VK_JUNJA = 0x17;
            public const int VK_FINAL = 0x18;
            public const int VK_HANJA = 0x19;
            public const int VK_KANJI = 0x19;
            public const int VK_IME_OFF = 0x1A;
            public const int VK_ESCAPE = 0x1B;
            public const int VK_CONVERT = 0x1C;
            public const int VK_NONCONVERT = 0x1D;
            public const int VK_ACCEPT = 0x1E;
            public const int VK_MODECHANGE = 0x1F;
            public const int VK_SPACE = 0x20;
            public const int VK_PRIOR = 0x21;
            public const int VK_NEXT = 0x22;
            public const int VK_END = 0x23;
            public const int VK_HOME = 0x24;
            public const int VK_LEFT = 0x25;
            public const int VK_UP = 0x26;
            public const int VK_RIGHT = 0x27;
            public const int VK_DOWN = 0x28;
            public const int VK_SELECT = 0x29;
            public const int VK_PRINT = 0x2A;
            public const int VK_EXECUTE = 0x2B;
            public const int VK_SNAPSHOT = 0x2C;
            public const int VK_INSERT = 0x2D;
            public const int VK_DELETE = 0x2E;
            public const int VK_HELP = 0x2F;
            public const int ALPHA0 = 0x30;
            public const int ALPHA1 = 0x31;
            public const int ALPHA2 = 0x32;
            public const int ALPHA3 = 0x33;
            public const int ALPHA4 = 0x34;
            public const int ALPHA5 = 0x35;
            public const int ALPHA6 = 0x36;
            public const int ALPHA7 = 0x37;
            public const int ALPHA8 = 0x38;
            public const int ALPHA9 = 0x39;
            public const int A = 0x41;
            public const int B = 0x42;
            public const int C = 0x43;
            public const int D = 0x44;
            public const int E = 0x45;
            public const int F = 0x46;
            public const int G = 0x47;
            public const int H = 0x48;
            public const int I = 0x49;
            public const int J = 0x4A;
            public const int K = 0x4B;
            public const int L = 0x4C;
            public const int M = 0x4D;
            public const int N = 0x4E;
            public const int O = 0x4F;
            public const int P = 0x50;
            public const int Q = 0x51;
            public const int R = 0x52;
            public const int S = 0x53;
            public const int T = 0x54;
            public const int U = 0x55;
            public const int V = 0x56;
            public const int W = 0x57;
            public const int X = 0x58;
            public const int Y = 0x59;
            public const int Z = 0x5A;
            public const int VK_LWIN = 0x5B;
            public const int VK_RWIN = 0x5C;
            public const int VK_APPS = 0x5D;
            public const int VK_SLEEP = 0x5F;
            public const int VK_NUMPAD0 = 0x60;
            public const int VK_NUMPAD1 = 0x61;
            public const int VK_NUMPAD2 = 0x62;
            public const int VK_NUMPAD3 = 0x63;
            public const int VK_NUMPAD4 = 0x64;
            public const int VK_NUMPAD5 = 0x65;
            public const int VK_NUMPAD6 = 0x66;
            public const int VK_NUMPAD7 = 0x67;
            public const int VK_NUMPAD8 = 0x68;
            public const int VK_NUMPAD9 = 0x69;
            public const int VK_MULTIPLY = 0x6A;
            public const int VK_ADD = 0x6B;
            public const int VK_SEPARATOR = 0x6C;
            public const int VK_SUBTRACT = 0x6D;
            public const int VK_DECIMAL = 0x6E;
            public const int VK_DIVIDE = 0x6F;
            public const int VK_F1 = 0x70;
            public const int VK_F2 = 0x71;
            public const int VK_F3 = 0x72;
            public const int VK_F4 = 0x73;
            public const int VK_F5 = 0x74;
            public const int VK_F6 = 0x75;
            public const int VK_F7 = 0x76;
            public const int VK_F8 = 0x77;
            public const int VK_F9 = 0x78;
            public const int VK_F10 = 0x79;
            public const int VK_F11 = 0x7A;
            public const int VK_F12 = 0x7B;
            public const int VK_F13 = 0x7C;
            public const int VK_F14 = 0x7D;
            public const int VK_F15 = 0x7E;
            public const int VK_F16 = 0x7F;
            public const int VK_F17 = 0x80;
            public const int VK_F18 = 0x81;
            public const int VK_F19 = 0x82;
            public const int VK_F20 = 0x83;
            public const int VK_F21 = 0x84;
            public const int VK_F22 = 0x85;
            public const int VK_F23 = 0x86;
            public const int VK_F24 = 0x87;
            public const int VK_NUMLOCK = 0x90;
            public const int VK_SCROLL = 0x91;
            public const int VK_LSHIFT = 0xA0;
            public const int VK_RSHIFT = 0xA1;
            public const int VK_LCONTROL = 0xA2;
            public const int VK_RCONTROL = 0xA3;
            public const int VK_LMENU = 0xA4;
            public const int VK_RMENU = 0xA5;
            public const int VK_BROWSER_BACK = 0xA6;
            public const int VK_BROWSER_FORWARD = 0xA7;
            public const int VK_BROWSER_REFRESH = 0xA8;
            public const int VK_BROWSER_STOP = 0xA9;
            public const int VK_BROWSER_SEARCH = 0xAA;
            public const int VK_BROWSER_FAVORITES = 0xAB;
            public const int VK_BROWSER_HOME = 0xAC;
            public const int VK_VOLUME_MUTE = 0xAD;
            public const int VK_VOLUME_DOWN = 0xAE;
            public const int VK_VOLUME_UP = 0xAF;
            public const int VK_MEDIA_NEXT_TRACK = 0xB0;
            public const int VK_MEDIA_PREV_TRACK = 0xB1;
            public const int VK_MEDIA_STOP = 0xB2;
            public const int VK_MEDIA_PLAY_PAUSE = 0xB3;
            public const int VK_LAUNCH_MAIL = 0xB4;
            public const int VK_LAUNCH_MEDIA_SELECT = 0xB5;
            public const int VK_LAUNCH_APP1 = 0xB6;
            public const int VK_LAUNCH_APP2 = 0xB7;
            public const int VK_OEM_1 = 0xBA;
            public const int VK_OEM_PLUS = 0xBB;
            public const int VK_OEM_COMMA = 0xBC;
            public const int VK_OEM_MINUS = 0xBD;
            public const int VK_OEM_PERIOD = 0xBE;
            public const int VK_OEM_2 = 0xBF;
            public const int VK_OEM_3 = 0xC0;
            public const int VK_OEM_4 = 0xDB;
            public const int VK_OEM_5 = 0xDC;
            public const int VK_OEM_6 = 0xDD;
            public const int VK_OEM_7 = 0xDE;
            public const int VK_OEM_8 = 0xDF;
            public const int VK_OEM_102 = 0xE2;
            public const int VK_PROCESSKEY = 0xE5;
            public const int VK_PACKET = 0xE7;
            public const int VK_ATTN = 0xF6;
            public const int VK_CRSEL = 0xF7;
            public const int VK_EXSEL = 0xF8;
            public const int VK_EREOF = 0xF9;
            public const int VK_PLAY = 0xFA;
            public const int VK_ZOOM = 0xFB;
            public const int VK_NONAME = 0xFC;
            public const int VK_PA1 = 0xFD;
            public const int VK_OEM_CLEAR = 0xFE;
        }

        static List<int> ScanKeysConsts()
        {
            List<int> l = new List<int>();
            Type KeysType = typeof(Keys);
            FieldInfo[] fields = KeysType.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo fi = fields[i];
                l.Add((int)fi.GetValue(fi.Name));
            }
            return l;
        }

        public static uint KEYEVENTF_KEYDOWN = 0x0000;
        public static uint KEYEVENTF_KEYUP = 0x0002;
        public static uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        public static void PressKey(int VK_KEY)
        {
            keybd_event(VK_KEY, 0x45, KEYEVENTF_KEYDOWN, 0);
        }

        public static void ReleaseKey(int VK_KEY)
        {
            keybd_event(VK_KEY, 0x45, KEYEVENTF_KEYUP, 0);
        }

        public static void UpdateKeys()
        {
            int KeysConstsCount = KEYS_CONSTS.Count;

            bool newLKS = false; //the 'LastKeysStamp' was just created

            if (LastKeysStamp == null)
            {
                LastKeysStamp = new bool[KeysConstsCount];
                newLKS = true;
            }

            bool[] NewKeysStamp = new bool[KeysConstsCount];

            for (int i = 0; i < KeysConstsCount; i++)
            {
                int currentVK_KEY = GetVK(i);
                bool newState = GetAsyncKeyState(currentVK_KEY) != 0;
                bool prevState = false;
                NewKeysStamp[i] = newState;
                if (newLKS)
                {
                    if (!prevState && newState)
                    {
                        if (OnKeyDown != null)
                            OnKeyDown.Invoke(GetVKName(currentVK_KEY), currentVK_KEY);
                    }
                }
                else
                {
                    prevState = LastKeysStamp[i];
                    if (!prevState && newState)
                    {
                        if (OnKeyDown != null)
                            OnKeyDown.Invoke(GetVKName(currentVK_KEY), currentVK_KEY);
                    }
                    else if (prevState && !newState)
                    {
                        if (OnKeyUp != null)
                            OnKeyUp.Invoke(GetVKName(currentVK_KEY), currentVK_KEY);
                    }
                    else if (prevState && newState)
                    {
                        if (OnKeyHeld != null)
                            OnKeyHeld.Invoke(GetVKName(currentVK_KEY), currentVK_KEY);
                    }
                }
            }
            NewKeysStamp.CopyTo(LastKeysStamp, 0);
        }

        public static string GetVKName(int VK_KEY)
        {
            Type KeysType = typeof(Keys);
            FieldInfo[] fields = KeysType.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                string Name = fields[i].Name;
                int value = (int)fields[i].GetValue(Name);
                if (value.Equals(VK_KEY))
                    return Name;
                continue;
            }
            return null;
        }

        public static int GetVK(string VK_Name)
        {
            Type KeysType = typeof(Keys);
            FieldInfo[] fields = KeysType.GetFields();
            for (int i = 0; i < fields.Length; i++)
            {
                string Name = fields[i].Name;
                int value = (int)fields[i].GetValue(Name);
                if (Name.Equals(VK_Name))
                    return value;
                continue;
            }
            return -1;
        }

        public static int GetVK(int VK_ID)
        {
            Type KeysType = typeof(Keys);
            FieldInfo[] fields = KeysType.GetFields();
            string Name = fields[VK_ID].Name;
            int value = (int)fields[VK_ID].GetValue(Name);
            return value;
        }

        public delegate void KeyEvent(string VK_Name, int VK_KEY);

        public static event KeyEvent OnKeyDown;
        public static event KeyEvent OnKeyUp;
        public static event KeyEvent OnKeyHeld;
    }
}
