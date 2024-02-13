using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;
using Windows.Management.Update;
using WindowsInput;
using WindowsInput.Native;

namespace Macro
{
    public class SubCharacterKeyMapping
    {
        private Dictionary<Key, VirtualKeyCode> KeyList { get; }
        private InputSimulator Sim { get; } = new InputSimulator();
        private bool IsEnabled { get; set; } = false;
        private IntPtr Hwnd { get; set; }

        public SubCharacterKeyMapping(Dictionary<Key, VirtualKeyCode> keyList)
        {
            KeyList = keyList;
            new KeyboardHook(CallBack);
        }

        public void SetHwnd(IntPtr hwnd)
        {
            Hwnd = hwnd;
        }

        public void MappingStart()
        {
            IsEnabled = true;
        }

        public void MappingStop()
        {
            IsEnabled = false;
        }

        private void CallBack(object sender, KeyboardHookedEventArgs e)
        {
            if (IsEnabled)
            {
                switch (e.KeyCode)
                {
                    case System.Windows.Input.Key.Left:
                        SendKey(Key.Left, e.UpDown);
                        break;
                    case System.Windows.Input.Key.Up:
                        SendKey(Key.Up, e.UpDown);
                        break;
                    case System.Windows.Input.Key.Right:
                        SendKey(Key.Right, e.UpDown);
                        break;
                    case System.Windows.Input.Key.Down:
                        SendKey(Key.Down, e.UpDown);
                        break;
                    case System.Windows.Input.Key.A:
                        SendKey(Key.A, e.UpDown);
                        break;
                    case System.Windows.Input.Key.S:
                        SendKey(Key.S, e.UpDown);
                        break;
                    case System.Windows.Input.Key.Z:
                        SendKey(Key.Z, e.UpDown);
                        break;
                    case System.Windows.Input.Key.X:
                        SendKey(Key.X, e.UpDown);
                        break;
                    case System.Windows.Input.Key.C:
                        SendKey(Key.C, e.UpDown);
                        break;
                    case System.Windows.Input.Key.V:
                        SendKey(Key.V, e.UpDown);
                        break;
                }
            }
        }

        private IntPtr prevHwnd;
        private void SendKey(Key key, KeyboardUpDown upDown)
        {
            var handle = WindowUtil.GetActiveWindowHandle();
            if (handle != Hwnd)
            {
                if (prevHwnd == Hwnd)
                {
                    foreach (var key2 in KeyList.Values)
                        Sim.Keyboard.KeyUp(key2);
                }
                prevHwnd = handle;
                return;
            }
            prevHwnd = handle;

            if (upDown is KeyboardUpDown.Down)
                Sim.Keyboard.KeyDown(KeyList[key]);
            else
                Sim.Keyboard.KeyUp(KeyList[key]);
        }
    }
}
