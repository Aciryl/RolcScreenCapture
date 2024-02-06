using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macro
{
    public class Actions
    {
        protected Actions()
        {

        }

        public static Actions SendKey { get; } = new Actions();
        public static Actions SendKeyT { get; } = new Actions();
        public static Actions Charge { get; } = new Actions();
        public static Actions ChargeT { get; } = new Actions();
        public static Actions KeyDwon { get; } = new Actions();
        public static Actions KeyUp { get; } = new Actions();
        public static Actions SleepF { get; } = new Actions();
        public static Actions ChangeState { get; } = new Actions();
        public static Actions WaitColor { get; } = new Actions();
        public static Actions WaitNotColor { get; } = new Actions();
        public static Actions SetMessage { get; } = new Actions();
    }
}
