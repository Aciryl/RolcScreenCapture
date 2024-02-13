using CaptureSampleCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace Macro.Maq
{
    public class Lyrica : MaqMacro
    {


        public Lyrica(IntPtr hwnd,
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


            Lobby.Add(ActionHelper(MaqActions.WaitKurumiState, States.Ready));
            Lobby.Add(ActionHelper(MaqActions.WaitAliceState, States.Ready));
            //Lobby.Add(ActionHelper(Actions.SleepF, 15));
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.C));
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.S));
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Lobby.Add(ActionHelper(Actions.ChangeState, States.Ready));

            // 移動
            Mission.Add(ActionHelper(MaqActions.WaitAliceMessage, "Moved"));
            Mission.AddRange(Perspective);
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.V, 5));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 50));
            Mission.Add(ActionHelper(Actions.SendMessage, "Moved"));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up, 30 * 2));

            Mission.Add(ActionHelper(MaqActions.WaitKurumiMessage, "Blessing_Lyrica"));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 2));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            Mission.Add(ActionHelper(Actions.SleepF, 10));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 10));
            Mission.Add(ActionHelper(Actions.SleepF, 5));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));


            Mission.Add(ActionHelper(MaqActions.WaitKurumiMessage, "Buff"));
            Mission.Add(ActionHelper(Actions.SleepF, 60));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.AddRange(Perspective);
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            Mission.Add(ActionHelper(Actions.SleepF, 10));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 22));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.AddRange(Perspective);
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            Mission.Add(ActionHelper(Actions.SleepF, 10));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 20));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.AddRange(Perspective);
            Mission.Add(ActionHelper(Actions.SendMessage, "Moved2"));
            Mission.Add(ActionHelper(MaqActions.WaitKurumiMessage, "Moved2"));
            Mission.Add(ActionHelper(MaqActions.WaitAliceMessage, "Moved2"));


            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 5));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up, 73));

            Mission.Add(ActionHelper(MaqActions.WaitKurumiMessage, "Moved3"));
            Mission.Add(ActionHelper(MaqActions.WaitAliceMessage, "Moved3"));

            //Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            //Mission.AddRange(FieldBrust);
            //Mission.AddRange(FieldBrust);
            Mission.Add(ActionHelper(Actions.SendMessage, "FieldBrust"));
            //Mission.Add(ActionHelper(Actions.SleepF, 30));

            // MP回復
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Down));
            Mission.Add(ActionHelper(Actions.SleepF, 1));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.C));
            Mission.Add(ActionHelper(Actions.SleepF, 1));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Down));
            Mission.AddRange(Perspective);
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Down));
            Mission.Add(ActionHelper(Actions.SleepF, 1));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.C));
            Mission.Add(ActionHelper(Actions.SleepF, 1));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Down));
            Mission.AddRange(Perspective);
            //Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            //Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            //Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            //Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            //Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            //Mission.Add(ActionHelper(Actions.SleepF, 15));
            //Mission.Add(ActionHelper(Actions.KeyDwon, Key.Down));
            //Mission.Add(ActionHelper(Actions.SleepF, 1));
            //Mission.Add(ActionHelper(Actions.SendKeyF, Key.C));
            //Mission.Add(ActionHelper(Actions.SleepF, 1));
            //Mission.Add(ActionHelper(Actions.KeyUp, Key.Down));
            //Mission.AddRange(Perspective);
            Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up, 13));
            Mission.Add(ActionHelper(Actions.SendMessage, "FieldBrust2"));

            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            //Mission.AddRange(FieldBrust);
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 0.25));
            Mission.Add(ActionHelper(Actions.SleepF, 60));
            Mission.Add(ActionHelper(Actions.SendMessage, "FieldBrust3"));

            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 30));
            Mission.Add(ActionHelper(Actions.SleepF, 10));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 5));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));

            Mission.Add(ActionHelper(Actions.SendMessage, "Buff2"));
            Mission.Add(ActionHelper(MaqActions.WaitKurumiMessage, "Buff2"));
            Mission.Add(ActionHelper(MaqActions.WaitAliceMessage, "Buff2"));

            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));

            Mission.Add(ActionHelper(Actions.SleepF, 30 * 8 + 2));

            Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            Mission.Add(ActionHelper(Actions.SleepF, 20));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));
            //Mission.Add(ActionHelper(Actions.SleepF, 10));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            //Mission.Add(ActionHelper(Actions.SleepF, 10));
            //Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 1));

            Mission.Add(ActionHelper(Actions.WaitReward));
            Mission.Add(ActionHelper(Actions.ChangeState, States.Reward));

            LobbyMove.Add(ActionHelper(Actions.SleepF, 45));
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
            LobbyMove.Add(ActionHelper(Actions.SleepF, 23 + 7 - 10));
            LobbyMove.Add(ActionHelper(Actions.KeyUp, Key.Right));
            LobbyMove.Add(ActionHelper(Actions.SendKeyF, Key.Up, 7));

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

            Error.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.Enter));
            Error.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.Divide));
            Error.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.E));
            Error.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.X));
            Error.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.I));
            Error.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.T));
            Error.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.Enter));
            Error.Add(ActionHelper(Actions.WaitColor, 0, 0, Color.Black));
            Error.Add(ActionHelper(Actions.WaitNotColor, 774, 47, Color.Black, 10));
            Error.Add(ActionHelper(Actions.ChangeState, States.LobbyMove));
        }
    }
}
