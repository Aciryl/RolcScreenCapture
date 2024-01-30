using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class WindowUtil
    {
        #region ウィンドウを検索

        // ウィンドウを列挙時に呼ばれるコールバック
        private delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

        // すべてのウィンドウを列挙
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumWindowsDelegate lpEnumFunc, IntPtr lparam);

        // 指定したウィンドウハンドルのタイトルの長さ
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        // 指定したウィンドウハンドルのタイトル
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        // 指定したウィンドウハンドルのクラス名
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        // ウィンドウハンドルからプロセスIDを取得
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        // 指定したウィンドウハンドルの子クラスを列挙
        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //private static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hWndChildAfter, string lpClassName, string lpWindowName);


        // 対象ウィンドウのクラス名
        private const string ClassName = "tmsbROLO";
        private static List<WindowInfo> infoList;

        /// <summary>
        /// Rolc のウィンドウ情報を取得する
        /// </summary>
        /// <returns>見つかった Rolc のウィンドウリスト</returns>
        public static List<WindowInfo> GetRolc()
        {
            infoList = new List<WindowInfo>();

            //ウィンドウを列挙して、対象のプロセスを探す
            EnumWindows(new EnumWindowsDelegate(EnumWindowCallBack), IntPtr.Zero);

            return infoList;
        }

        private static bool EnumWindowCallBack(IntPtr hwnd, IntPtr lparam)
        {
            //ウィンドウのクラス名を取得する
            var csb = new StringBuilder(256);
            GetClassName(hwnd, csb, csb.Capacity);
            //クラス名に指定された文字列を含むか
            if (csb.ToString().IndexOf(ClassName) < 0)
            {
                //含んでいない時は、次のウィンドウを検索
                return true;
            }

            //プロセスのIDを取得する
            GetWindowThreadProcessId(hwnd, out int processId);

            //今まで見つかったプロセスでは無いことを確認する
            if (!infoList.Any(x => x.ProcessID == processId))
            {
                /*
                //ウィンドウのタイトルの長さを取得する
                int textLen = GetWindowTextLength(hwnd);
                string title;
                if (textLen > 0)
                {
                    //ウィンドウのタイトルを取得する
                    var tsb = new StringBuilder(textLen + 1);
                    GetWindowText(hwnd, tsb, tsb.Capacity);
                    title = tsb.ToString();
                }
                else
                    title = "";
                //*/

                // リストに追加
                infoList.Add(new WindowInfo(hwnd, processId));
            }

            //次のウィンドウを検索
            return true;
        }

        public class WindowInfo
        {
            public IntPtr HWnd { get; private set; }
            public int ProcessID { get; private set; }
            public Process Process { get; private set; }
            public string Title { get; private set; }

            public WindowInfo(IntPtr hwnd, int processID)
            {
                HWnd = hwnd;
                ProcessID = processID;
                Process = Process.GetProcessById(ProcessID);
                Title = Process.MainWindowTitle;
            }
        }

        #endregion ウィンドウを検索

        #region ウィンドウの(アクティブ/非アクティブ)

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        // ウィンドウをアクティブにする
        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //private const int WM_KEYDOWN = 0x0100;

        private const int SW_SHOWNORMAL = 0x0001;
        private const int SW_SHOWNOACTIVATE = 0x0004;

        private const int WM_ACTIVATE = 0x0006;
        private const int WA_ACTIVE = 0x0001;

        //private const int SWP_NOSIZE = 0x0001;
        //private const int SWP_NOMOVE = 0x0002;
        //private const int SWP_NOZORDER = 0x0004;

        /// <summary>
        /// 指定したウィンドウのアクティブにする
        /// </summary>
        /// <param name="hwnd">アクティブにするウィンドウのハンドル</param>
        /// <param name="change">現在のアクティブウィンドウを非アクティブにするか</param>
        public static void Activate(IntPtr hwnd, bool change = true)
        {
            if (change)
            {
                //SetWindowPos(hwnd, 0, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOZORDER);
                ShowWindow(hwnd, SW_SHOWNORMAL);
                SetForegroundWindow(hwnd);
            }
            else
            {
                ShowWindow(hwnd, SW_SHOWNOACTIVATE);
                PostMessage(hwnd, WM_ACTIVATE, WA_ACTIVE, 0);
            }
        }

        /// <summary>
        /// 指定したウィンドウのアクティブにする
        /// </summary>
        /// <param name="info">アクティブにするウィンドウの情報</param>
        /// /// <param name="change">現在のアクティブウィンドウを非アクティブにするか</param>
        public static void Activate(WindowInfo info, bool change = true)
            => Activate(info.HWnd, change);

        /// <summary>
        /// 指定したウィンドウの非アクティブにする
        /// </summary>
        /// <param name="hwnd">非アクティブにするウィンドウのハンドル</param>
        public static void InActivate(IntPtr hwnd)
        {
            var activeHWnd = GetForegroundWindow();
            Activate(hwnd);
            Activate(activeHWnd);
        }

        /// <summary>
        /// 指定したウィンドウの非アクティブにする
        /// </summary>
        /// <param name="info">非アクティブにするウィンドウの情報</param>
        public static void InActivate(WindowInfo info)
            => InActivate(info.HWnd);

        #endregion ウィンドウの(アクティブ/非アクティブ)

        public static IntPtr GetActiveWindowHandle()
        {
            return GetForegroundWindow();
        }
    }
}
