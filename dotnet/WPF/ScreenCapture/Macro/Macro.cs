using CaptureSampleCore;
using Macro.Maq;
using Reactive.Bindings;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.AccessControl;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Xml.Linq;
using Utils;
using Windows.UI;
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

        protected static List<(Actions action, object[] param)> Perspective { get; } = new List<(Actions action, object[] param)>();
        protected static List<(Actions action, object[] param)> FieldBrust { get; } = new List<(Actions action, object[] param)>();

        protected Action<States> SetState { get; }
        protected Func<States> GetState { get; }
        protected Action<string> SetMessage { get; }
        protected ScreenShot ScreenShot { get; }
        protected double LF { get; }
        protected double RF { get; }
        protected int Id { get; }

        protected double SleepMag { get; set; } = 1.2;

        public static Dictionary<(int x, int y), Dictionary<string, List<(int x, int y, byte r, byte g, byte b)>>> ItemCheckPoints { get; private set; }
        public static Dictionary<int, Dictionary<string, RewardAction>> RewardActions { get; private set; }
        private const int SamplingCount = 20;

        private const int ResultXMain = 831;
        private const int ResultYMain = 16;
        private const int ResultXSub = 681;
        private const int ResultYSub = 18;

        private const int FailedXMain = 834;
        private const int FailedYMain = 13;
        private const int FailedXSub = 674;
        private const int FailedYSub = 15;

        private const int DesideXMain = 806;
        private const int DesideYMain = 312;
        private const int DesideXSub = 646;
        private const int DesideYSub = 263;

        private const int CancelXMain = 767;
        private const int CancelYMain = 46;
        private const int CancelXSub = 607;
        private const int CancelYSub = 47;

        private static int itemID = 0;

        public enum RewardAction
        {
            None,
            Left1,
            Left2,
            Right1,
            Right2,
            Right3,
        }

        protected enum Color
        {
            White,
            Black,
            Gray,
            Orange,
            Green,
            YellowGreen,
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

        static Macro()
        {
            LoadItemData();

            Perspective.Add(ActionHelper(Actions.KeyDwon, Key.A));
            Perspective.Add(ActionHelper(Actions.KeyDwon, Key.S));
            Perspective.Add(ActionHelper(Actions.SleepF, 2));
            Perspective.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.Enter));
            Perspective.Add(ActionHelper(Actions.SleepF, 2));
            Perspective.Add(ActionHelper(Actions.KeyUp, Key.A));
            Perspective.Add(ActionHelper(Actions.KeyUp, Key.S));
            Perspective.Add(ActionHelper(Actions.SleepF, 2));
            Perspective.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.Enter));
            Perspective.Add(ActionHelper(Actions.SleepF, 2));

            //FieldBrust.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            FieldBrust.Add(ActionHelper(Actions.SleepF, 43));
            FieldBrust.Add(ActionHelper(Actions.KeyDwon, Key.Down));
            FieldBrust.Add(ActionHelper(Actions.SleepF, 1));
            FieldBrust.Add(ActionHelper(Actions.SendKeyF, Key.C));
            FieldBrust.Add(ActionHelper(Actions.SleepF, 1));
            FieldBrust.Add(ActionHelper(Actions.KeyUp, Key.Down));
            FieldBrust.AddRange(Perspective);
            FieldBrust.Add(ActionHelper(Actions.KeyDwon, Key.Z));
            FieldBrust.Add(ActionHelper(Actions.KeyDwon, Key.X));
            FieldBrust.Add(ActionHelper(Actions.SleepF, 22));
            FieldBrust.Add(ActionHelper(Actions.SendKeyF, Key.Up, 18));
            FieldBrust.Add(ActionHelper(Actions.KeyUp, Key.Z));
            FieldBrust.Add(ActionHelper(Actions.KeyUp, Key.X));
            FieldBrust.Add(ActionHelper(Actions.SleepF, 2));

            Reward.Add(ActionHelper(Actions.Reward));
            Reward.Add(ActionHelper(Actions.WaitAfterReward));
            Reward.Add(ActionHelper(Actions.WaitColor, 0, 0, Color.Black));
            Reward.Add(ActionHelper(Actions.WaitNotColor, 774, 47, Color.Black, 10));
            Reward.Add(ActionHelper(Actions.ChangeState, States.LobbyMove));
        }

        public Macro(IntPtr hwnd,
                     Dictionary<Key, VirtualKeyCode> keyList,
                     Func<States> getState,
                     Action<States> setState,
                     Action<string> setMessage,
                     ScreenShot screenShot,
                     double lf,
                     double rf,
                     MacroManager manager,
                     int id)
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

            Ready.Add(ActionHelper(Actions.WaitColor, 0, 0, Color.Black));
            Ready.Add(ActionHelper(Actions.WaitNotColor, 774, 47, Color.Black, 10));
            Ready.Add(ActionHelper(Actions.ChangeState, States.Mission));
            Id = id;
        }

        public static (int width, int height)[] LoadDataFromImage(string[] filenames)
        {
            var rand = new Random();
            var vec = new List<(int width, int height)>();

            foreach (var filename in filenames)
            {
                var itemName = Regex.Match(Path.GetFileName(filename), "(.+)_\\d.png").Result("$1");
                var (colors, width, height) = ScreenShot.BitmapToBytes(filename);
                var list = new List<(int x, int y, byte r, byte g, byte b)>();
                var minX = width;
                var minY = height;
                var maxX = 0;
                var maxY = 0;
                vec.Add((width, height));

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        var index = (y * width + x) * 4;
                        var r = colors[index + 0];
                        var g = colors[index + 1];
                        var b = colors[index + 2];
                        var a = colors[index + 3];
                        if (r != 0 || g != 0 || b != 0)
                        {
                            minX = Math.Min(minX, x);
                            minY = Math.Min(minY, y);
                            maxX = Math.Max(maxX, x);
                            maxY = Math.Max(maxY, y);
                            list.Add((x, y, r, g, b));
                        }
                    }
                }

                if (!ItemCheckPoints.ContainsKey((width, height)))
                    ItemCheckPoints[(width, height)] = new Dictionary<string, List<(int x, int y, byte r, byte g, byte b)>>();

                //if (!ItemCheckPoints[(width, height)].ContainsKey(itemName))
                    ItemCheckPoints[(width, height)][itemName] = new List<(int x, int y, byte r, byte g, byte b)>();

                for (int i = 0; i < SamplingCount; ++i)
                {
                    for (int ii = 0; ii < 100; ++ii)
                    {
                        var x = rand.Next(minX, maxX + 1);
                        var y = rand.Next(minY, maxY + 1);
                        if (!ItemCheckPoints[(width, height)][itemName].Any(tuple => tuple.x == x && tuple.y == y) &&
                            !list.Any(tuple => tuple.x == x && tuple.y == y))
                        {
                            ItemCheckPoints[(width, height)][itemName].Add((x, y, 0, 0, 0));
                            break;
                        }
                    }
                }

                if (list.Count <= SamplingCount)
                {
                    ItemCheckPoints[(width, height)][itemName].AddRange(list);
                }
                else
                {
                    for (int i = 0; i < SamplingCount; ++i)
                    {
                        var tuple = list[rand.Next(list.Count)];
                        ItemCheckPoints[(width, height)][itemName].Add(tuple);
                        list.Remove(tuple);
                    }
                }

                //for (int id = 0; id < BasicSampleApplication.ScreenCount; ++id)
                //{
                //    ChangeRewardActions(id, itemName, RewardAction.None);
                //}
            }

            SaveItemCheckPoints();

            return vec.ToArray();
        }

        private static void LoadItemData()
        {
            string path = Directory.GetCurrentDirectory();
            string filename1 = "ItemCheckPoints.dat";
            string filename2 = "RewardActions.dat";
            if (File.Exists($@"{path}\{filename1}"))
                ItemCheckPoints = LoadFromBinaryFile<Dictionary<(int x, int y), Dictionary<string, List<(int x, int y, byte r, byte g, byte b)>>>>($@"{path}\{filename1}");
            else
                ItemCheckPoints = new Dictionary<(int x, int y), Dictionary<string, List<(int x, int y, byte r, byte g, byte b)>>>();

            if (File.Exists($@"{path}\{filename2}"))
            {
                RewardActions = LoadFromBinaryFile<Dictionary<int, Dictionary<string, RewardAction>>>($@"{path}\{filename2}");
            }
            else
            {
                RewardActions = new Dictionary<int, Dictionary<string, RewardAction>>();
                for (int i = 0; i < BasicSampleApplication.ScreenCount; ++i)
                {
                    RewardActions[i] = new Dictionary<string, RewardAction>();
                    RewardActions[i]["その他"] = RewardAction.None;
                }
            }
        }

        private static void SaveItemCheckPoints()
        {
            string path = Directory.GetCurrentDirectory();
            string filename1 = "ItemCheckPoints.dat";
            SaveToBinaryFile($@"{path}\{filename1}", ItemCheckPoints);
        }

        public static void ChangeRewardActions(int id, string itemName, RewardAction action)
        {
            if (!RewardActions.ContainsKey(id) || RewardActions[id] == null)
                RewardActions[id] = new Dictionary<string, RewardAction>();

            if (!RewardActions[id].ContainsKey(itemName) || RewardActions[id][itemName] != action)
                RewardActions[id][itemName] = action;

            SaveRewardActions();
        }

        private static void SaveRewardActions()
        {
            string path = Directory.GetCurrentDirectory();
            string filename2 = "RewardActions.dat";
            SaveToBinaryFile($@"{path}\{filename2}", RewardActions);
        }

        /// <summary>
        /// オブジェクトの内容をファイルから読み込み復元する
        /// </summary>
        /// <param name="path">読み込むファイル名</param>
        /// <returns>復元されたオブジェクト</returns>
        private static T LoadFromBinaryFile<T>(string path)
        {
            T obj;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                //読み込んで逆シリアル化する
                obj = (T)bf.Deserialize(fs);
            }

            return obj;
        }

        /// <summary>
        /// オブジェクトの内容をファイルに保存する
        /// </summary>
        /// <param name="path">保存先のファイル名</param>
        /// <param name="obj">保存するオブジェクト</param>
        private static void SaveToBinaryFile(string path, object obj)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                BinaryFormatter bf = new BinaryFormatter();
                //シリアル化して書き込む
                bf.Serialize(fs, obj);
            }
        }

        protected int lobbyMoveIndex, lobbyIndex, readyIndex, missionIndex, pendingIndex, rewardIndex, errorIndex;
        public virtual void Start()
        {
            MacroManager.ErrorHandler errorHandler = () =>
            {
                if (Manager.HasError)
                {
                    SetState.Invoke(States.Error);
                }
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
                    if (Manager.HasError)
                        missionIndex = 0;
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
                case Actions a when a == Actions.SendKeyF:// || a == Actions.SendKeyFT:
                    if (actionList[index].param.Length == 1)
                    {
                        //if (a == Actions.SendKeyF)
                        {
                            if (actionList[index].param[0] is Key key)
                                SendKey(key);
                            else
                                SendKey((System.Windows.Input.Key)actionList[index].param[0]);
                        }
                        //else
                        //{
                        //    if (actionList[index].param[0] is Key key)
                        //        Task.Run(() => SendKey(key));
                        //    else
                        //        Task.Run(() => SendKey((System.Windows.Input.Key)actionList[index].param[0]));
                        //}
                    }
                    else if (actionList[index].param.Length == 2)
                    {
                        //if (a == Actions.SendKeyF)
                        if (actionList[index].param[1] is int i)
                            SendKeyF((Key)actionList[index].param[0], i);
                        else
                            SendKeyF((Key)actionList[index].param[0], (double)actionList[index].param[1]);
                        //else
                        //    Task.Run(() => SendKeyF((Key)actionList[index].param[0], (int)actionList[index].param[1]));
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case Actions a when a == Actions.KeyDwon:
                    if (actionList[index].param.Length == 1)
                    {
                        if (actionList[index].param[0] is Key key)
                            KeyDown(key);
                        else
                            KeyDown((System.Windows.Input.Key)actionList[index].param[0]);
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case Actions a when a == Actions.KeyUp:
                    if (actionList[index].param.Length == 1)
                    {
                        if (actionList[index].param[0] is Key key)
                            KeyUp(key);
                        else
                            KeyUp((System.Windows.Input.Key)actionList[index].param[0]);
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
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
                                if (Near(r, rr) && Near(g, gg) && Near(b, bb) && Near(a, aa))
                                    ++count;
                                else
                                    count = 0;

                                if (count >= endCount)
                                {
                                    tcs.SetResult(true);
                                }
                            };
                        }
                        else
                        {
                            handler = () =>
                            {
                                var (rr, gg, bb, aa) = GetColor((int)actionList[index].param[0], (int)actionList[index].param[1]);
                                if (!Near(r, rr) || !Near(g, gg) || !Near(b, bb) || !Near(a, aa))
                                    ++count;
                                else
                                    count = 0;

                                if (count >= endCount)
                                {
                                    tcs.SetResult(true);
                                }
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
                case Actions act when act == Actions.WaitReward:
                    if (actionList[index].param.Length == 0)
                    {
                        var (r, g, b, a) = ColorToByte(Color.White);
                        var clearCount = 0;
                        var failedCount = 0;
                        var endCount = 45;

                        var tcs = new TaskCompletionSource<bool>();
                        ScreenShot.UpdateEventHandler handler = () =>
                        {
                            var (rr, gg, bb, aa) = Id == 0 ? GetColor(ResultXMain, ResultYMain) : GetColor(ResultXSub, ResultYSub);

                            if (Near(r, rr) && Near(g, gg) && Near(b, bb) && Near(a, aa))
                                ++clearCount;
                            else
                                clearCount = 0;

                            if (clearCount >= endCount)
                            {
                                tcs.SetResult(true);
                            }

                            (rr, gg, bb, aa) = Id == 0 ? GetColor(FailedXMain, FailedYMain) : GetColor(FailedXSub, FailedYSub);
                            if (Near(r, rr) && Near(g, gg) && Near(b, bb) && Near(a, aa))
                                ++failedCount;
                            else
                                failedCount = 0;

                            if (failedCount >= endCount)
                            {
                                tcs.SetResult(true);
                            }
                        };

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
                case Actions act when act == Actions.Reward:
                    if (actionList[index].param.Length == 0)
                    {
                        while (true)
                        {
                            if (Manager.HasError)
                                break;

                            var color = Id == 0 ? GetColor(DesideXMain, DesideYMain) : GetColor(DesideXSub, DesideYSub);
                            if (color == ColorToByte(Color.White))
                            {
                                SendKeyF(Key.Z);
                                break;
                            }

                            var width = ScreenShot.ClientWidth;
                            var height = ScreenShot.ClientHeight;
                            var itemName = "その他";
                            var colors = new (byte r, byte g, byte b, byte a)[]
                            {
                                ColorToByte(Color.White),
                                //ColorToByte(Color.Orange),
                                //ColorToByte(Color.Green),
                                //ColorToByte(Color.YellowGreen),
                            };
                            if (ItemCheckPoints.ContainsKey((width, height)))
                            {
                                var dict = ItemCheckPoints[(width, height)];
                                foreach (var key in dict.Keys)
                                {
                                    var list = dict[key];
                                    var flag = true;
                                    foreach (var (x, y, r, g, b) in list)
                                    {
                                        var (rr, gg, bb, aa) = GetColor(x, y);
                                        if (r == 0 && g == 0 && b == 0)
                                        {
                                            foreach (var (rrr, ggg, bbb, aaa) in colors)
                                            {
                                                if (Near(rr, rrr) || Near(gg, ggg) || Near(bb, bbb))
                                                {
                                                    flag = false;
                                                    break;
                                                }
                                            }
                                            if (!flag)
                                                break;
                                        }
                                        else
                                        {
                                            if (!Near(r, rr) || !Near(g, gg) || !Near(b, bb))
                                            {
                                                flag = false;
                                                break;
                                            }
                                        }
                                    }

                                    if (flag)
                                    {
                                        itemName = key;
                                        break;
                                    }
                                }
                            }

                            if (itemName.Equals("その他"))
                            {
                                var (rr, gg, bb, aa) = ColorToByte(Color.White);
                                ScreenShot.Save($@"D:\Visual Studio Projects\rolc\RolcScreenCapture\dotnet\WPF\ScreenCapture\ItemImages\Item{++itemID}_原画_{Id}.png");
                                ScreenShot.Save($@"D:\Visual Studio Projects\rolc\RolcScreenCapture\dotnet\WPF\ScreenCapture\ItemImages\Item{itemID}_{Id}.png",
                                    tuple =>
                                    {
                                        var (r, g, b, a) = tuple;
                                        if (Near(r, rr) && Near(g, gg) && Near(b, bb) && Near(a, aa))
                                        {
                                            return (r, g, b, a);
                                        }

                                        return (0, 0, 0, 255);
                                    });
                            }

                            RewardAction action;
                            if (RewardActions[Id].ContainsKey(itemName))
                                action = RewardActions[Id][itemName];
                            else
                                action = RewardAction.None;

                            switch (action)
                            {
                                case RewardAction.None:
                                    break;
                                case RewardAction.Left1:
                                    SendKeyF(Key.Left);
                                    break;
                                case RewardAction.Left2:
                                    SendKeyF(Key.Left);
                                    SendKeyF(Key.Left);
                                    break;
                                case RewardAction.Right1:
                                    SendKeyF(Key.Right);
                                    break;
                                case RewardAction.Right2:
                                    SendKeyF(Key.Right);
                                    SendKeyF(Key.Right);
                                    break;
                                case RewardAction.Right3:
                                    SendKeyF(Key.Right);
                                    SendKeyF(Key.Right);
                                    SendKeyF(Key.Right);
                                    break;
                            }

                            SendKeyF(Key.Down);
                        }
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case Actions act when act == Actions.WaitAfterReward:
                    if (actionList[index].param.Length == 0)
                    {
                        var (r, g, b, a) = ColorToByte(Color.White);
                        var (r2, g2, b2, a2) = ColorToByte(Color.Gray);
                        var clearCount = 0;
                        var endCount = 10;

                        var tcs = new TaskCompletionSource<bool>();
                        ScreenShot.UpdateEventHandler handler = () =>
                        {
                            var (rr, gg, bb, aa) = Id == 0 ? GetColor(CancelXMain, CancelYMain) : GetColor(CancelXSub, CancelYSub);
                            if ((Near(r, rr) && Near(g, gg) && Near(b, bb) && Near(a, aa)) ||
                                (Near(r2, rr) && Near(g2, gg) && Near(b2, bb) && Near(a2, aa)))
                                ++clearCount;
                            else
                                clearCount = 0;

                            if (clearCount >= endCount)
                            {
                                tcs.SetResult(true);
                            }
                        };

                        MacroManager.ErrorHandler errorHandler = () =>
                        {
                            tcs.SetResult(false);
                        };


                        ScreenShot.UpdateEvent += handler;
                        Manager.OnError += errorHandler;

                        tcs.Task.Wait();

                        ScreenShot.UpdateEvent -= handler;
                        Manager.OnError -= errorHandler;

                        SendKeyF(Key.X);
                    }
                    else
                    {
                        ThrowActionError(name, index);
                    }
                    break;
                case Actions a when a == Actions.SendMessage:
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

            //var vk = ConvertVirtualKeyCodeToVkCode(GetVK(key));
            //KeyMouseUtil.SendKey((int)GetVK(key), Math.Max(2 * 1000 / 30, time));
            //Thread.Sleep(2 * 1000 / 30);

            var vk = GetVK(key);
            Sim.Keyboard.KeyDown(vk)
                        .Sleep(Math.Max(2 * 1000 / 30, time))
                        .KeyUp(vk)
                        .Sleep(2 * 1000 / 30);
        }
        /*
        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint uCode, MapVirtualKeyMapTypes uMapType);

        private enum MapVirtualKeyMapTypes
        {
            MAPVK_VK_TO_VSC = 0x00,
            MAPVK_VSC_TO_VK = 0x01,
            MAPVK_VK_TO_CHAR = 0x02,
            MAPVK_VSC_TO_VK_EX = 0x03,
        }

        private static int ConvertVirtualKeyCodeToVkCode(VirtualKeyCode virtualKeyCode)
        {
            int scanCode = MapVirtualKey((uint)virtualKeyCode, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
            int vkCode = MapVirtualKey((uint)scanCode, MapVirtualKeyMapTypes.MAPVK_VSC_TO_VK);

            return vkCode;
        }//*/

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
            //KeyMouseUtil.KeyDown((int)GetVK(key));
        }

        protected void KeyDown(System.Windows.Input.Key key)
        {
            KeyMouseUtil.KeyDown(Hwnd, key);
        }

        protected void KeyUp(Key key)
        {
            Sim.Keyboard.KeyUp(GetVK(key));
            //KeyMouseUtil.KeyUp((int)GetVK(key));
        }

        protected void KeyUp(System.Windows.Input.Key key)
        {
            KeyMouseUtil.KeyUp(Hwnd, key);
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
            => Sleep((int)(frame * SleepMag * 1000 / 30));

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
                        SleepF(2);

                    }
                    else
                    {
                        KeyDown(Key.X);
                        SleepF((LF - RF) * charge);
                        KeyDown(Key.Z);
                        SleepF(RF * charge);
                        KeyUp(Key.Z);
                        KeyUp(Key.X);
                        SleepF(2);
                    }
                    break;
            }
        }

        protected (byte r, byte g, byte b, byte a) GetColor(int x, int y)
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
                case Color.Orange:
                    r = 0x40;
                    g = 0xc0;
                    b = 0xff;
                    a = 0xff;
                    break;
                case Color.Green:
                    r = 0xc0;
                    g = 0xff;
                    b = 0x40;
                    a = 0xff;
                    break;
                case Color.YellowGreen:
                    r = 0x40;
                    g = 0xff;
                    b = 0xc0;
                    a = 0xff;
                    break;
            }

            return (r, g, b, a);
        }

        protected bool Near(byte c1, byte c2, byte dif = 5)
        {
            return c1 >= c2 - dif && c1 <= c2 + dif;
        }
    }
}
