using CaptureSampleCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

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
                      Manager manager)
            : base(hwnd, keyList, getState, setState, setMessage, screenShot, lf, rf, manager)
        {
            Lobby.Add(ActionHelper(Actions.SendKey, Key.C));
            Lobby.Add(ActionHelper(Actions.SendKey, Key.Z));
            Lobby.Add(ActionHelper(Actions.SendKey, Key.Z));
            Lobby.Add(ActionHelper(Actions.ChangeState, States.Ready));

            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.Charge, Skill.CS, 2));
            Mission.Add(ActionHelper(Actions.SleepF, 30));
            Mission.Add(ActionHelper(Actions.SendKey, Key.Z));
            Mission.Add(ActionHelper(Actions.ChangeState, States.Pending, false));
        }
    }
}
