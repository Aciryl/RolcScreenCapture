using CaptureSampleCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput.Native;

namespace Macro.Maq
{
    public class Manager : MacroManager
    {
        public delegate void StateChangeHandler();
        public event StateChangeHandler OnLyricaStateChanged;
        public event StateChangeHandler OnKurumiStateChanged;
        public event StateChangeHandler OnAliceStateChanged;

        public States LyricaState { get; private set; } = States.Lobby;
        public States KurumiState { get; private set; } = States.Lobby;
        public States AliceState { get; private set; } = States.Lobby;

        public delegate void MessageHandler();
        public event MessageHandler OnLyricaMessage;
        public event MessageHandler OnKurumiMessage;
        public event MessageHandler OnAliceMessage;

        public string LyricaMessage { get; private set; } = string.Empty;
        public string KurumiMessage { get; private set; } = string.Empty;
        public string AliceMessage { get; private set; } = string.Empty;

        private const double LyricaRF = 10;
        private const double LyricaLF = 10;

        private const double KurumiRF = 6;
        private const double KurumiLF = 9;

        private const double AliceRF = 5;
        private const double AliceLF = 7;

        public Manager(IntPtr mainWindowHandle, IntPtr[] hwnds, Dictionary<Key, VirtualKeyCode>[] keyLists, Action mappingStart, Action mappingStop, ScreenShot[] screenShot)
            : base(mainWindowHandle, mappingStart, mappingStop)
        {
            if ((hwnds?.Length ?? 0) != 3 || (keyLists?.Length ?? 0) != 3 || (screenShot?.Length ?? 0) != 3)
                throw new ArgumentException("キャラクターの人数が違います。このマクロにはキャラクターが 3 人必要です");

            AddMacro(new Lyrica(hwnds[0], keyLists[0], GetLyricaState, SetLyricaState, SetLyricaMessage, screenShot[0], LyricaLF, LyricaRF, this));
            AddMacro(new Kurumi(hwnds[1], keyLists[1], GetKurumiState, SetKurumiState, SetKurumiMessage, screenShot[1], KurumiLF, KurumiRF, this));
            AddMacro(new Alice(hwnds[2], keyLists[2], GetAliceState, SetAliceState, SetAliceMessage, screenShot[2], AliceLF, AliceRF, this));

            States GetLyricaState()
            {
                return LyricaState;
            }

            States GetKurumiState()
            {
                return KurumiState;
            }

            States GetAliceState()
            {
                return AliceState;
            }

            void SetLyricaState(States state)
            {
                LyricaState = state;
                OnLyricaStateChanged?.Invoke();
            }

            void SetKurumiState(States state)
            {
                KurumiState = state;
                OnKurumiStateChanged?.Invoke();
            }

            void SetAliceState(States state)
            {
                AliceState = state;
                OnAliceStateChanged?.Invoke();
            }

            void SetLyricaMessage(string message)
            {
                LyricaMessage = message ?? string.Empty;
                OnLyricaMessage?.Invoke();
            }

            void SetKurumiMessage(string message)
            {
                KurumiMessage = message ?? string.Empty;
                OnKurumiMessage?.Invoke();
            }

            void SetAliceMessage(string message)
            {
                AliceMessage = message ?? string.Empty;
                OnAliceMessage?.Invoke();
            }
        }
    }
}
