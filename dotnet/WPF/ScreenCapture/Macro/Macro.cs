using CaptureSampleCore;
using Macro.Maq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using Utils;
using WindowsInput;
using WindowsInput.Native;
using static Macro.MacroManager;

namespace Macro
{
    public enum Key
    {
        Left,
        Up,
        Right,
        Down,
        A,
        S,
        Z,
        X,
        C,
        V,
    }

    public abstract class Macro
    {
        protected List<(Actions action, object[] param)> LobbyMove { get; } = new List<(Actions action, object[] param)>();
        protected List<(Actions action, object[] param)> Lobby { get; } = new List<(Actions action, object[] param)>();
        private List<(Actions action, object[] param)> Ready { get; } = new List<(Actions action, object[] param)>();
        protected List<(Actions action, object[] param)> Mission { get; } = new List<(Actions action, object[] param)>();
        protected List<(Actions action, object[] param)> Pending { get; } = new List<(Actions action, object[] param)>();
        private static List<(Actions action, object[] param)> Reward { get; } = new List<(Actions action, object[] param)>();
        protected List<(Actions action, object[] param)> Error { get; } = new List<(Actions action, object[] param)>();

        protected Action<States> SetState { get; }
        protected Func<States> GetState { get; }
        protected Action<string> SetMessage { get; }
        protected ScreenShot ScreenShot { get; }
        protected double LF { get; }
        protected double RF { get; }

        protected enum Color
        {
            White,
            Black,
            Gray,
        }

        protected enum Skill
        {
            Left,
            Righ,
            CS,
        }

        private IntPtr Hwnd { get; }

        private Dictionary<Key, VirtualKeyCode> KeyList { get; set; }
        private InputSimulator Sim { get; } = new InputSimulator();

        private MacroManager Manager { get; }

        public Macro(IntPtr hwnd,
                     Dictionary<Key, VirtualKeyCode> keyList,
                     Func<States> getState,
                     Action<States> setState,
                     Action<string> setMessage,
                     ScreenShot screenShot,
                     double lf,
                     double rf,
                     MacroManager manager)
        {
            var keys = new Key[]
            {
                Key.Left,
                Key.Up,
                Key.Right,
                Key.Down,
                Key.A,
                Key.S,
                Key.Z,
                Key.X,
                Key.C,
                Key.V,
            };

            foreach (var key in keys)
            {
                if (!keyList.ContainsKey(key))
                    throw new ArgumentException($"Key : {key} が登録されていません");
            }

            KeyList = keyList;
            Hwnd = hwnd;
            GetState = getState;
            SetState = setState;
            SetMessage = setMessage;
            ScreenShot = screenShot;
            LF = lf;
            RF = rf;
            Manager = manager;

            Ready.Add(ActionHelper(Actions.WaitColor, ScreenShot.ClientWidth / 2 - 1, ScreenShot.ClientHeight / 2 - 1, Color.Black));
            Ready.Add(ActionHelper(Actions.WaitNotColor, 0, 0, Color.Black));
            Ready.Add(ActionHelper(Actions.ChangeState, States.Mission));
        }

        protected int lobbyMoveIndex, lobbyIndex, readyIndex, missionIndex, pendingIndex, rewardIndex, errorIndex;
        public virtual void Start()
        {
            MacroManager.ErrorHandler errorHandler = () =>
            {
                if (Manager.HasError)
                    SetState.Invoke(States.Error);
            };
            Manager.OnError += errorHandler;

            ScreenShot.UpdateEventHandler handler = () =>
            {
                CheckError();
            };
            ScreenShot.UpdateEvent += handler;

            while (true)
            {
                if (Manager.IsCanceled)
                {
                    End(errorHandler, handler);
                    return;
                }
                else
                {
                    if (!SwitchState(errorHandler, handler))
                        return;
                }
            }
        }

        protected virtual bool SwitchState(MacroManager.ErrorHandler errorHandler, ScreenShot.UpdateEventHandler handler)
        {
            switch (GetState.Invoke())
            {
                case States s when s == States.LobbyMove:
                    if (lobbyMoveIndex < LobbyMove.Count)
                    {
                        lobbyMoveIndex = Invoke(LobbyMove, lobbyMoveIndex, "LobbyMove");
                    }
                    else
                    {
                        End(errorHandler, handler);
                        return false;
                    }
                    break;
                case States s when s == States.Lobby:
                    if (lobbyIndex < Lobby.Count)
                    {
                        lobbyIndex = Invoke(Lobby, lobbyIndex, "Lobby");
                    }
                    else
                    {
                        End(errorHandler, handler);
                        return false;
                    }
                    break;
                case States s when s == States.Ready:
                    if (readyIndex < Ready.Count)
                    {
                        readyIndex = Invoke(Ready, readyIndex, "Ready");
                    }
                    else
                    {
                        End(errorHandler, handler);
                        return false;
                    }
                    break;
                case States s when s == States.Mission:
                    if (missionIndex < Mission.Count)
                    {
                        missionIndex = Invoke(Mission, missionIndex, "Mission");
                    }
                    else
                    {
                        End(errorHandler, handler);
                        return false;
                    }
                    break;
                case States s when s == States.Pending:
                    if (pendingIndex < Pending.Count)
                    {
                        pendingIndex = Invoke(Pending, pendingIndex, "Pending");
                    }
                    else
                    {
                        End(errorHandler, handler);
                        return false;
                    }
                    break;
                case States s when s == States.Reward:
                    if (rewardIndex < Reward.Count)
                    {
                        rewardIndex = Invoke(Reward, rewardIndex, "Reward");
                    }
                    else
                    {
                        End(errorHandler, handler);
                        return false;
                    }
                    break;
                case States s when s == States.Error:
                    if (errorIndex < Error.Count)
                    {
                        errorIndex = Invoke(Error, errorIndex, "Error");
                    }
                    else
                    {
                        End(errorHandler, handler);
                        return false;
                    }
                    break;
                default:
                    throw new Exception($"無効な State です : {GetState.ToString()}");
            }

            return true;
        }

        private void End(MacroManager.ErrorHandler errorHandler, ScreenShot.UpdateEventHandler handler)
        {
            Manager.OnError -= errorHandler;
            ScreenShot.UpdateEvent -= handler;
        }

        protected virtual int Invoke(List<(Actions action, object[] param)> actionList, int index, string name)
        {
            switch (actionList[index].action)
            {
                case Actions a when a == Actions.SendKey || a == Actions.SendKeyT:
                    if (actionList[index].param.Length == 1)
                    {
                        if (a == Actions.SendKey)
                        {
                            if (actionList[index].param[0] is Key key)
                                SendKey(key);
                            else
                                SendKey((System.Windows.Input.Key)actionList[index].param[0]);
                        }
                        else
                        {
                            if (actionList[index].param[0] is Key key)
                                Task.Run(() => SendKey(key));
                            else
                                Task.Run(() => SendKey((System.Windows.Input.Key)actionList[index].param[0]));
                        }
                    }
                    else if (actionList[index].param.Length == 2)
                    {
                        if (a == Actions.SendKey)
                            SendKey((Key)actionList[index].param[0], (int)actionList[index].param[1]);
                        else
                            Task.Run(() => SendKey((Key)actionList[index].param[0], (int)actionList[index].param[1]));
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case Actions a when a == Actions.KeyDwon:
                    if (actionList[index].param.Length == 1)
                        KeyDown((Key)actionList[index].param[0]);
                    else
                        ThrowActionError(name, index);
                    break;
                case Actions a when a == Actions.KeyUp:
                    if (actionList[index].param.Length == 1)
                        KeyUp((Key)actionList[index].param[0]);
                    else
                        ThrowActionError(name, index);
                    break;
                case Actions a when a == Actions.Charge || a == Actions.ChargeT:
                    if (actionList[index].param.Length == 2)
                    {
                        if (a == Actions.Charge)
                        {
                            if (actionList[index].param[1] is int)
                                Charge((Skill)actionList[index].param[0], (int)actionList[index].param[1]);
                            else
                                Charge((Skill)actionList[index].param[0], (double)actionList[index].param[1]);
                        }
                        else
                        {
                            if (actionList[index].param[1] is int)
                                Task.Run(() => Charge((Skill)actionList[index].param[0], (int)actionList[index].param[1]));
                            else
                                Task.Run(() => Charge((Skill)actionList[index].param[0], (double)actionList[index].param[1]));
                        }
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case Actions a when a == Actions.SleepF:
                    if (actionList[index].param.Length == 1)
                    {
                        if (actionList[index].param[0] is int)
                            SleepF((int)actionList[index].param[0]);
                        else
                            SleepF((double)actionList[index].param[0]);
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case Actions a when a == Actions.ChangeState:
                    // param : 0 -> 次の State / 1 -> index をリセットするか (true : リセットする)
                    if (actionList[index].param.Length == 1 || actionList[index].param.Length == 2)
                    {
                        SetState.Invoke((States)actionList[index].param[0]);
                        if (actionList[index].param.Length == 1 || (actionList[index].param.Length == 2 && (bool)actionList[index].param[1]))
                            index = -1;
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case Actions act when act == Actions.WaitColor || act == Actions.WaitNotColor:
                    // param : 0 -> x / 1 -> y / 2 -> color / 3 -> count
                    if (actionList[index].param.Length == 3 || actionList[index].param.Length == 4)
                    {
                        var endCount = actionList[index].param.Length == 3 ? 1 : (int)actionList[index].param[3];
                        var count = 0;
                        var (r, g, b, a) = ColorToByte((Color)actionList[index].param[2]);

                        var tcs = new TaskCompletionSource<bool>();
                        ScreenShot.UpdateEventHandler handler = null;

                        if (act == Actions.WaitColor)
                        {
                            handler = () =>
                            {
                                var (rr, gg, bb, aa) = GetColor((int)actionList[index].param[0], (int)actionList[index].param[1]);
                                if (r == rr && g == gg && b == bb && a == aa)
                                    ++count;
                                else
                                    count = 0;

                                if (count >= endCount)
                                    tcs.SetResult(true);
                            };
                        }
                        else
                        {
                            handler = () =>
                            {
                                var (rr, gg, bb, aa) = GetColor((int)actionList[index].param[0], (int)actionList[index].param[1]);
                                if (r != rr || g != gg || b != bb || a != aa)
                                    ++count;
                                else
                                    count = 0;

                                if (count >= endCount)
                                    tcs.SetResult(true);
                            };
                        }

                        MacroManager.ErrorHandler errorHandler = () =>
                        {
                            tcs.SetResult(false);
                        };


                        ScreenShot.UpdateEvent += handler;
                        Manager.OnError += errorHandler;

                        tcs.Task.Wait();

                        ScreenShot.UpdateEvent -= handler;
                        Manager.OnError -= errorHandler;
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case Actions a when a == Actions.SetMessage:
                    if (actionList[index].param.Length == 1)
                        SetMessage.Invoke((string)actionList[index].param[0]);
                    else
                        ThrowActionError(name, index);
                    break;
            }

            ++index;

            return index;
        }

        protected virtual void CheckError()
        {

        }

        public void Activate(bool change = false)
        {
            WindowUtil.Activate(Hwnd, change);
        }

        protected static (Actions action, object[] param) ActionHelper(Actions action, params object[] param)
        {
            return (action, param);
        }

        protected void ThrowActionError(string actionName, int index)
        {
            throw new Exception($"{actionName}[{index}]のパラメータが不正です");
        }

        protected void SendKeyF(Key key, double frame = 0)
            => SendKey(key, (int)(frame * 1000 / 30));
        protected void SendKey(Key key, int time = 0)
        {
            if (time < 0)
                throw new ArgumentException("time は 0 以上の数にしてください");

            var vk = GetVK(key);
            Sim.Keyboard.KeyDown(vk)
                        .Sleep(Math.Max(1 * 1000 / 30, time))
                        .KeyUp(vk)
                        .Sleep(2 * 1000 / 30);
        }

        //protected void SendKeyF(System.Windows.Input.Key key, double frame = 0)
        //    => SendKey(key, (int)(frame * 1000 / 30));
        protected void SendKey(System.Windows.Input.Key key)
        {
            //if (time < 0)
            //    throw new ArgumentException("time は 0 以上の数にしてください");

            KeyMouseUtil.SendKey(Hwnd, key);
        }

        protected void KeyDown(Key key)
        {
            Sim.Keyboard.KeyDown(GetVK(key));
        }

        protected void KeyUp(Key key)
        {
            Sim.Keyboard.KeyUp(GetVK(key));
        }

        protected VirtualKeyCode GetVK(Key key)
        {
            VirtualKeyCode vk;

            if (KeyList.ContainsKey(key))
                vk = KeyList[key];
            else
                throw new ArgumentException($"指定されたキーが見つかりません : {key.ToString()}");

            return vk;
        }

        protected void SleepF(double frame)
            => Sleep((int)(frame * 1.2 * 1000 / 30));

        protected void Sleep(int time)
        {
            Sim.Keyboard.Sleep(time);
        }

        protected void Charge(Skill skill, double charge)
        {
            switch (skill)
            {
                case Skill.Left:
                    SendKeyF(Key.X, LF * charge);
                    break;
                case Skill.Righ:
                    SendKeyF(Key.Z, RF * charge);
                    break;
                case Skill.CS:
                    if (LF < RF)
                    {
                        KeyDown(Key.Z);
                        SleepF((RF - LF) * charge);
                        KeyDown(Key.X);
                        SleepF(LF * charge);
                        KeyUp(Key.Z);
                        KeyUp(Key.X);
                    }
                    else
                    {
                        KeyDown(Key.X);
                        SleepF((LF - RF) * charge);
                        KeyDown(Key.Z);
                        SleepF(RF * charge);
                        KeyUp(Key.Z);
                        KeyUp(Key.X);
                    }
                    break;
            }
        }

        protected (int r, int g, int b, int a) GetColor(int x, int y)
        {
            return ScreenShot.GetPixels(x, y);
        }
        private (byte r, byte g, byte b, byte a) ColorToByte(Color color)
        {
            byte r = 0, g = 0, b = 0, a = 0;
            switch (color)
            {
                case Color.Black:
                    r = g = b = 0;
                    a = 255;
                    break;
                case Color.White:
                    r = g = b = a = 255;
                    break;
                case Color.Gray:
                    r = g = b = 128;
                    a = 255;
                    break;
            }

            return (r, g, b, a);
        }
    }
}
