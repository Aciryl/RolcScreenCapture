using CaptureSampleCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace Macro.Maq
{
    public class Alice : MaqMacro
    {
        public Alice(IntPtr hwnd,
                     Dictionary<Key, VirtualKeyCode> keyList,
                     Func<States> getState, Action<States> setState,
                     Action<string> setMessage,
                     ScreenShot screenShot,
                     double lf,
                     double rf,
                     Manager manager,
                     int id)
            : base(hwnd, keyList, getState, setState, setMessage, screenShot, lf, rf, manager, id)
        {
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.C));
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Lobby.Add(ActionHelper(Actions.ChangeState, States.Ready));

            // 移動
            Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.AddRange(Perspective);
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.V, 5));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 40));
            Mission.Add(ActionHelper(Actions.SendMessage, "Moved"));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up, 30 * 2));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 15));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));

            Mission.Add(ActionHelper(MaqActions.WaitKurumiMessage, "Blessing_Alice"));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 5));

            // 移動
            Mission.Add(ActionHelper(MaqActions.WaitKurumiMessage, "Buff"));
            Mission.Add(ActionHelper(Actions.SleepF, 10));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 15));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up, 15));
            Mission.Add(ActionHelper(Actions.SendMessage, "Moved2"));
            Mission.Add(ActionHelper(MaqActions.WaitKurumiMessage, "Moved2"));
            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "Moved2"));

            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            Mission.Add(ActionHelper(Actions.SleepF, 45));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 5));
            Mission.Add(ActionHelper(Actions.SleepF, 45));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));

            Mission.Add(ActionHelper(Actions.SendKeyF, Key.V, 5));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Left));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Down, 8));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Left));

            Mission.Add(ActionHelper(Actions.SendMessage, "Moved3"));

            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "FieldBrust"));
            //Mission.Add(ActionHelper(Actions.SleepF, 55));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.F8));
            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "FieldBrust2"));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up));
            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "FieldBrust3"));

            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Right));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            Mission.Add(ActionHelper(Actions.SleepF, 5));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Right));
            Mission.Add(ActionHelper(Actions.SleepF, 5));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 50));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 10));
            //Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            //Mission.Add(ActionHelper(Actions.SleepF, 5));
            //Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 5));
            //Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));
            //Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            //Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 5));
            //Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));


            Mission.Add(ActionHelper(Actions.SendMessage, "Buff2"));
            Mission.Add(ActionHelper(MaqActions.WaitKurumiMessage, "Buff2"));
            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "Buff2"));

            Mission.Add(ActionHelper(Actions.SleepF, 12));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));

            Mission.Add(ActionHelper(Actions.SleepF, 30 * 8 + 2));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            Mission.Add(ActionHelper(Actions.SleepF, 25));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            //Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            //Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));

            Mission.Add(ActionHelper(Actions.WaitReward));
            Mission.Add(ActionHelper(Actions.ChangeState, States.Reward));

            LobbyMove.Add(ActionHelper(MaqActions.WaitKurumiState, States.Lobby));
            LobbyMove.AddRange(Perspective);
            LobbyMove.Add(ActionHelper(Actions.SendKeyF, Key.Right, 5));
            LobbyMove.Add(ActionHelper(Actions.SendKeyF, Key.Up, 25));
            LobbyMove.Add(ActionHelper(Actions.KeyDwon, Key.Right));
            LobbyMove.Add(ActionHelper(Actions.KeyDwon, Key.Down));
            LobbyMove.Add(ActionHelper(Actions.SleepF, 2));
            LobbyMove.Add(ActionHelper(Actions.KeyUp, Key.Right));
            LobbyMove.Add(ActionHelper(Actions.SleepF, 3));
            LobbyMove.Add(ActionHelper(Actions.KeyUp, Key.Down));
            LobbyMove.Add(ActionHelper(Actions.KeyDwon, Key.Right));
            LobbyMove.Add(ActionHelper(Actions.SleepF, 5));
            LobbyMove.Add(ActionHelper(Actions.SendKeyF, Key.Up, 5));
            LobbyMove.Add(ActionHelper(Actions.SleepF, 23 - 10));
            LobbyMove.Add(ActionHelper(Actions.KeyUp, Key.Right));

            LobbyMove.Add(ActionHelper(Actions.ChangeState, States.Lobby));

            Error.Add(ActionHelper(Actions.KeyUp, Key.Left));
            Error.Add(ActionHelper(Actions.KeyUp, Key.Up));
            Error.Add(ActionHelper(Actions.KeyUp, Key.Right));
            Error.Add(ActionHelper(Actions.KeyUp, Key.Down));
            Error.Add(ActionHelper(Actions.KeyUp, Key.A));
            Error.Add(ActionHelper(Actions.KeyUp, Key.S));
            Error.Add(ActionHelper(Actions.KeyUp, Key.Z));
            Error.Add(ActionHelper(Actions.KeyUp, Key.X));
            Error.Add(ActionHelper(Actions.KeyUp, Key.C));
            Error.Add(ActionHelper(Actions.KeyUp, Key.V));

            Error.Add(ActionHelper(MaqActions.WaitLyricaState, States.LobbyMove));
            Error.Add(ActionHelper(Actions.ChangeState, States.LobbyMove));
        }
    }
}
