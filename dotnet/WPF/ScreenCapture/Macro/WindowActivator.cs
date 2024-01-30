using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace Macro
{
    public class WindowActivator
    {
        public IntPtr?[] Hwnds { get; }
        public WindowActivator(int screenCount)
        {
            Hwnds = new IntPtr?[screenCount];

            new KeyboardHook(CallBack);
        }

        private void CallBack(object sender, KeyboardHookedEventArgs e)
        {
            if ((e.KeyCode.Equals(System.Windows.Input.Key.NumPad0) || e.KeyCode.Equals(System.Windows.Input.Key.NumPad1)) && e.UpDown == KeyboardUpDown.Down)
            {
                var activeHandle = WindowUtil.GetActiveWindowHandle().ToInt32();

                if (e.KeyCode.Equals(System.Windows.Input.Key.NumPad0))
                {
                    if ((Hwnds[0].HasValue && activeHandle == Hwnds[0].Value.ToInt32()) ||
                        (Hwnds[2].HasValue && activeHandle == Hwnds[2].Value.ToInt32()))
                    {
                        if (Hwnds[1].HasValue)
                            WindowUtil.Activate(Hwnds[1].Value);
                    }
                    else if (Hwnds[1].HasValue && activeHandle == Hwnds[1].Value.ToInt32())
                    {
                        if (Hwnds[0].HasValue)
                            WindowUtil.Activate(Hwnds[0].Value);
                    }
                }
                else
                {
                    if ((Hwnds[0].HasValue && activeHandle == Hwnds[0].Value.ToInt32()) ||
                        (Hwnds[1].HasValue && activeHandle == Hwnds[1].Value.ToInt32()))
                    {
                        if (Hwnds[2].HasValue)
                            WindowUtil.Activate(Hwnds[2].Value);
                    }
                    else if (Hwnds[2].HasValue && activeHandle == Hwnds[2].Value.ToInt32())
                    {
                        if (Hwnds[0].HasValue)
                            WindowUtil.Activate(Hwnds[0].Value);
                    }
                }
            }
            else if (e.KeyCode.Equals(System.Windows.Input.Key.RightCtrl) && e.UpDown == KeyboardUpDown.Down)
            {
                if (Hwnds[0].HasValue)
                    WindowUtil.Activate(Hwnds[0].Value);
            }
        }
    }
}
