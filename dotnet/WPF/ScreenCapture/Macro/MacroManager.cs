using CaptureSampleCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace Macro
{
    public abstract class MacroManager
    {
        public delegate void ErrorHandler();
        public event ErrorHandler OnError;

        private bool _hasError = false;
        public bool HasError
        {
            get => _hasError;
            set
            {
                _hasError = value;
                if (_hasError)
                    OnError?.Invoke();
            }
        }
        public bool IsCanceled { get; set; }

        private List<Macro> MacroList { get; } = new List<Macro>();
        private IntPtr MainWindowHandle { get; }
        private Action MappingStart { get; }
        private Action MappingStop { get; }

        public MacroManager(IntPtr mainWindowHandle, Action mappingStart, Action mappingStop)
        {
            MainWindowHandle = mainWindowHandle;
            MappingStart = mappingStart;
            MappingStop = mappingStop;
        }

        protected void AddMacro(Macro macro)
        {
            MacroList.Add(macro);
        }

        public virtual async void Start()
        {
            MappingStop.Invoke();

            foreach (var macro in MacroList)
            {
                macro.Activate(macro == MacroList[MacroList.Count - 1]);
            }

            WindowUtil.Activate(MainWindowHandle, false);

            await Task.WhenAll(MacroList.Select(x => Task.Run(() => x.Start())));

            MappingStart.Invoke();
        }
    }
}
