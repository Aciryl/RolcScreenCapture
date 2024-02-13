using CaptureSampleCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;
using static System.Windows.Forms.AxHost;

namespace Macro.Maq
{
    public class Kurumi : MaqMacro
    {
        public Kurumi(IntPtr hwnd,
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
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.C));
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Lobby.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Lobby.Add(ActionHelper(Actions.ChangeState, States.Ready));

            // 移動
            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "Moved"));
            Mission.AddRange(Perspective);
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.V, 5));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 50));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 5));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));

            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            Mission.Add(ActionHelper(Actions.SleepF, 10));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up, 15));
            Mission.Add(ActionHelper(Actions.SendMessage, "Blessing_Lyrica"));

            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            Mission.Add(ActionHelper(Actions.SleepF, 10));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 12));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Mission.Add(ActionHelper(Actions.SendMessage, "Blessing_Alice"));

            Mission.Add(ActionHelper(Actions.SleepF, 20));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up, 15));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 2));
            Mission.Add(ActionHelper(Actions.SleepF, 10));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up, 5));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 5));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Z));

            Mission.Add(ActionHelper(Actions.SendMessage, "Buff"));

            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 0.25));

            // 移動
            Mission.Add(ActionHelper(Actions.SleepF, 50));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 40));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up, 20));
            Mission.Add(ActionHelper(Actions.SendMessage, "Moved2"));
            Mission.Add(ActionHelper(MaqActions.WaitAliceMessage, "Moved2"));
            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "Moved2"));

            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 3));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 21));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.SleepF, 60));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));

            //Mission.Add(ActionHelper(Actions.SendKeyF, Key.V, 5));
            //Mission.Add(ActionHelper(Actions.KeyDwon, Key.Right));
            //Mission.Add(ActionHelper(Actions.SendKeyF, Key.Down, 5));
            //Mission.Add(ActionHelper(Actions.KeyUp, Key.Right));

            Mission.Add(ActionHelper(Actions.SendMessage, "Moved3"));

            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "FieldBrust"));
            //Mission.Add(ActionHelper(Actions.SleepF, 55));
            Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.Add(ActionHelper(Actions.SendKeyF, System.Windows.Input.Key.F8));
            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "FieldBrust2"));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Up));
            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "FieldBrust3"));

            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Right, 20));
            Mission.Add(ActionHelper(Actions.KeyDwon, Key.Up));
            Mission.Add(ActionHelper(Actions.SleepF, 5));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 25));
            Mission.Add(ActionHelper(Actions.KeyUp, Key.Up));

            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 4));
            Mission.Add(ActionHelper(Actions.SleepF, 15));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 2));
            Mission.Add(ActionHelper(Actions.SleepF, 15));
            //Mission.Add(ActionHelper(Actions.SendKeyF, Key.Left, 5));
            Mission.Add(ActionHelper(Actions.SendKeyF, Key.Z));
            Mission.Add(ActionHelper(Actions.SleepF, 30 * 7));

            Mission.Add(ActionHelper(Actions.SendMessage, "Buff2"));
            Mission.Add(ActionHelper(MaqActions.WaitLyricaMessage, "Buff2"));
            Mission.Add(ActionHelper(MaqActions.WaitAliceMessage, "Buff2"));

            Mission.Add(ActionHelper(Actions.SleepF, 30 * 10));
            Mission.Add(ActionHelper(Actions.Charge, Skill.Left, 3.5));
            Mission.Add(ActionHelper(Actions.Charge, Skill.Righ, 3.5));
            Mission.Add(ActionHelper(Actions.Charge, Skill.Left, 3.5));
            Mission.Add(ActionHelper(Actions.Charge, Skill.Righ, 3.5));
            Mission.Add(ActionHelper(Actions.Charge, Skill.Left, 3.5));
            Mission.Add(ActionHelper(Actions.Charge, Skill.Righ, 3.5));
            Mission.Add(ActionHelper(Actions.Charge, Skill.Left, 3.5));
            Mission.Add(ActionHelper(Actions.Charge, Skill.Righ, 3.5));

            Mission.Add(ActionHelper(Actions.WaitReward));
            Mission.Add(ActionHelper(Actions.ChangeState, States.Reward));

            LobbyMove.Add(ActionHelper(MaqActions.WaitLyricaState, States.Lobby));
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
            LobbyMove.Add(ActionHelper(Actions.SleepF, 23 + 4 - 10));
            LobbyMove.Add(ActionHelper(Actions.KeyUp, Key.Right));
            LobbyMove.Add(ActionHelper(Actions.SendKeyF, Key.Up, 4));

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

        private int count = 0;
        private int missionCounter = 0;
        private States prevState = States.Lobby;
        protected override void CheckError()
        {
            var state = GetState.Invoke();
            if (prevState != States.Mission && state == States.Mission)
            {
                missionCounter = 0;
            }

            if (state == States.Mission)
            {
                ++missionCounter;
                if (missionCounter > 60 * 60 * 2)
                {
                    Manager.HasError = true;
                }

                var color = GetColor(610, 12);
                if (!Near(color.r, 0xff, 64) || !Near(color.g, 0xff, 64) || !Near(color.b, 0, 64))
                    ++count;
                else
                    count = 0;

                if (count > 60 * 4)
                {
                    Manager.HasError = true;
                    count = 0;
                }
            }
            else
            {
                count = 0;
            }

            if (state == States.LobbyMove)
                Manager.HasError = false;

            prevState = state;
        }
    }
}
