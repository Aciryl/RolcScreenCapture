using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

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
        Enter,
    }

    public class Macro
    {
        private Dictionary<Key, VirtualKeyCode> KeyList { get; set; } = new Dictionary<Key, VirtualKeyCode>();
        private InputSimulator Sim { get; set; } = new InputSimulator();

        public Macro(Dictionary<Key, VirtualKeyCode> keyList)
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
        }


    }
}
