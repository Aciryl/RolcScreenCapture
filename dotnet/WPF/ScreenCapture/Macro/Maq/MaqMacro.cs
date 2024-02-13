using CaptureSampleCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace Macro.Maq
{
    public abstract class MaqMacro : Macro
    {
        protected Manager Manager { get; }

        public MaqMacro(IntPtr hwnd,
                        Dictionary<Key, VirtualKeyCode> keyList,
                        Func<States> getState,
                        Action<States> setState,
                        Action<string> setMessage,
                        ScreenShot screenShot,
                        double lf,
                        double rf,
                        Manager manager,
                        int id)
            : base(hwnd, keyList, getState, setState, setMessage, screenShot, lf, rf, manager, id)
        {
            Manager = manager;
        }

        protected override int Invoke(List<(Actions action, object[] param)> actionList, int index, string name)
        {
            switch (actionList[index].action)
            {
                case MaqActions a when a == MaqActions.WaitLyricaState:
                    if (actionList[index].param.Length == 1)
                    {
                        var state = (States)actionList[index].param[0];
                        
                        if (Manager.LyricaState != state)
                        {
                            var tcs = new TaskCompletionSource<bool>();

                            Manager.StateChangeHandler handler = () =>
                            {
                                if (Manager.LyricaState == state)
                                    tcs.SetResult(true);
                            };

                            MacroManager.ErrorHandler errorHandler = () =>
                            {
                                tcs.SetResult(false);
                            };

                            Manager.OnLyricaStateChanged += handler;
                            Manager.OnError += errorHandler;

                            tcs.Task.Wait();

                            Manager.OnLyricaStateChanged -= handler;
                            Manager.OnError -= errorHandler;
                        }
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case MaqActions a when a == MaqActions.WaitKurumiState:
                    if (actionList[index].param.Length == 1)
                    {
                        var state = (States)actionList[index].param[0];
                        
                        if (Manager.KurumiState != state)
                        {
                            var tcs = new TaskCompletionSource<bool>();

                            Manager.StateChangeHandler handler = () =>
                            {
                                if (Manager.KurumiState == state)
                                    tcs.SetResult(true);
                            };

                            MacroManager.ErrorHandler errorHandler = () =>
                            {
                                tcs.SetResult(false);
                            };

                            Manager.OnKurumiStateChanged += handler;
                            Manager.OnError += errorHandler;

                            tcs.Task.Wait();

                            Manager.OnKurumiStateChanged -= handler;
                            Manager.OnError -= errorHandler;
                        }
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case MaqActions a when a == MaqActions.WaitAliceState:
                    if (actionList[index].param.Length == 1)
                    {
                        var state = (States)actionList[index].param[0];
                        
                        if (Manager.AliceState != state)
                        {
                            var tcs = new TaskCompletionSource<bool>();

                            Manager.StateChangeHandler handler = () =>
                            {
                                if (Manager.AliceState == state)
                                    tcs.SetResult(true);
                            };

                            MacroManager.ErrorHandler errorHandler = () =>
                            {
                                tcs.SetResult(false);
                            };

                            Manager.OnAliceStateChanged += handler;
                            Manager.OnError += errorHandler;

                            tcs.Task.Wait();

                            Manager.OnAliceStateChanged -= handler;
                            Manager.OnError -= errorHandler;
                        }
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case MaqActions a when a == MaqActions.WaitLyricaMessage:
                    if (actionList[index].param.Length == 1)
                    {
                        var message = (string)actionList[index].param[0];
                        
                        if (!Manager.LyricaMessage.Equals(message ?? string.Empty))
                        {
                            var tcs = new TaskCompletionSource<bool>();

                            Manager.MessageHandler handler = () =>
                            {
                                if (Manager.LyricaMessage.Equals(message))
                                    tcs.SetResult(true);
                            };

                            MacroManager.ErrorHandler errorHandler = () =>
                            {
                                tcs.SetResult(false);
                            };

                            Manager.OnLyricaMessage += handler;
                            Manager.OnError += errorHandler;

                            tcs.Task.Wait();

                            Manager.OnLyricaMessage -= handler;
                            Manager.OnError -= errorHandler;
                        }
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case MaqActions a when a == MaqActions.WaitKurumiMessage:
                    if (actionList[index].param.Length == 1)
                    {
                        var message = (string)actionList[index].param[0];

                        if (!Manager.KurumiMessage.Equals(message ?? string.Empty))
                        {
                            var tcs = new TaskCompletionSource<bool>();

                            Manager.MessageHandler handler = () =>
                            {
                                if (Manager.KurumiMessage.Equals(message))
                                    tcs.SetResult(true);
                            };

                            MacroManager.ErrorHandler errorHandler = () =>
                            {
                                tcs.SetResult(false);
                            };

                            Manager.OnKurumiMessage += handler;
                            Manager.OnError += errorHandler;

                            tcs.Task.Wait();

                            Manager.OnKurumiMessage -= handler;
                            Manager.OnError -= errorHandler;
                        }
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case MaqActions a when a == MaqActions.WaitAliceMessage:
                    if (actionList[index].param.Length == 1)
                    {
                        var message = (string)actionList[index].param[0];

                        if (!Manager.AliceMessage.Equals(message ?? string.Empty))
                        {
                            var tcs = new TaskCompletionSource<bool>();

                            Manager.MessageHandler handler = () =>
                            {
                                if (Manager.AliceMessage.Equals(message))
                                    tcs.SetResult(true);
                            };

                            MacroManager.ErrorHandler errorHandler = () =>
                            {
                                tcs.SetResult(false);
                            };

                            Manager.OnAliceMessage += handler;
                            Manager.OnError += errorHandler;

                            tcs.Task.Wait();

                            Manager.OnAliceMessage -= handler;
                            Manager.OnError -= errorHandler;
                        }
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
            }

            return base.Invoke(actionList, index, name);
        }
    }
}
