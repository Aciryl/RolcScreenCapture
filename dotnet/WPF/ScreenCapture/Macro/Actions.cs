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

        public static Actions SendKeyF { get; } = new Actions();
        //public static Actions SendKeyFT { get; } = new Actions();
        public static Actions Charge { get; } = new Actions();
        public static Actions ChargeT { get; } = new Actions();
        public static Actions KeyDwon { get; } = new Actions();
        public static Actions KeyUp { get; } = new Actions();
        public static Actions SleepF { get; } = new Actions();
        public static Actions ChangeState { get; } = new Actions();
        public static Actions WaitColor { get; } = new Actions();
        public static Actions WaitNotColor { get; } = new Actions();
        public static Actions SendMessage { get; } = new Actions();
        public static Actions WaitReward { get; } = new Actions();
        public static Actions Reward { get; } = new Actions();
        public static Actions WaitAfterReward { get; } = new Actions();
    }
}
