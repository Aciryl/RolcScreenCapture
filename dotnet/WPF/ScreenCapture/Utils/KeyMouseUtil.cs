using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Utils
{
    public class KeyMouseUtil
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private const int MAPVK_VK_TO_VSC = 0;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "MapVirtualKeyA")]
        private static extern int MapVirtualKey(int wCode, int wMapType);

        [DllImport("user32.dll", SetLastError = true)]
        private extern static void SendInput(int nInputs, Input[] pInputs, int cbsize);

        [StructLayout(LayoutKind.Sequential)]
        struct Input
        {
            public int Type;
            public InputUnion ui;
        }

        [StructLayout(LayoutKind.Explicit)]
        struct InputUnion
        {
            [FieldOffset(0)]
            public MouseInput Mouse;
            [FieldOffset(0)]
            public KeyboardInput Keyboard;
            [FieldOffset(0)]
            public HardwareInput Hardware;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct MouseInput
        {
            public int X;
            public int Y;
            public int Data;
            public int Flags;
            public int Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct KeyboardInput
        {
            public short VirtualKey;
            public short ScanCode;
            public int Flags;
            public int Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct HardwareInput
        {
            public int uMsg;
            public short wParamL;
            public short wParamH;
        }

        private const int INPUT_MOUSE = 0;
        private const int INPUT_KEYBOARD = 1;
        private const int INPUT_HARDWARE = 2;

        // 仮想キーコード
        //private const int VK_LBUTTON = 0x01;
        //private const int VK_RBUTTON = 0x02;

        // フラグ
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;

        public static void SendKey(IntPtr hwnd, Key key)
        {
            var vKey = KeyInterop.VirtualKeyFromKey(key);
            int vsc = MapVirtualKey(vKey, MAPVK_VK_TO_VSC);

            PostMessage(hwnd, WM_KEYDOWN, (IntPtr)vKey, (IntPtr)((vsc << 16) | 1));
            PostMessage(hwnd, WM_KEYUP, (IntPtr)vKey, (IntPtr)((vsc << 16) | 0));
        }

        public static void MoveMouse(int x, int y)
        {
            Input[] inputs = new Input[1];

            // マウスを移動する
            inputs[0].Type = INPUT_MOUSE;
            inputs[0].ui.Mouse.Flags = MOUSEEVENTF_MOVE;
            inputs[0].ui.Mouse.X = x;
            inputs[0].ui.Mouse.Y = y;

            SendInput(inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }

        public static void Click(bool isLeftClick, int time = 0)
        {
            Input[] inputs = new Input[1];

            // マウスを移動する
            inputs[0].Type = INPUT_MOUSE;
            inputs[0].ui.Mouse.Flags = isLeftClick ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_RIGHTDOWN;

            SendInput(inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));

            Thread.Sleep(time);

            inputs[0].ui.Mouse.Flags = isLeftClick ? MOUSEEVENTF_LEFTUP : MOUSEEVENTF_RIGHTUP;

            SendInput(inputs.Length, inputs, Marshal.SizeOf(typeof(Input)));
        }
    }
}
