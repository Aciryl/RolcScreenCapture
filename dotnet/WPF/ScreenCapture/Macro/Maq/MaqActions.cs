using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro.Maq
{
    public class MaqActions : Actions
    {
        public static Actions WaitLyricaState { get; } = new MaqActions();
        public static Actions WaitKurumiState { get; } = new MaqActions();
        public static Actions WaitAliceState { get; } = new MaqActions();
        public static Actions WaitLyricaMessage { get; } = new MaqActions();
        public static Actions WaitKurumiMessage { get; } = new MaqActions();
        public static Actions WaitAliceMessage { get; } = new MaqActions();
    }
}
