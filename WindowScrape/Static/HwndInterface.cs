using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;
using WindowScrape.Constants;
using WindowScrape.Types;

namespace WindowScrape.Static {
    internal static class HwndInterface {
        #region Windows
        public static List<IntPtr> EnumHwnds() {
            return EnumChildren(IntPtr.Zero);
        }

        public static IntPtr GetHwnd(string windowText, string className) {
            return (IntPtr)FindWindow(className, windowText);
        }
        public static IntPtr GetHwndFromTitle(string windowText) {
            return (IntPtr)FindWindow(null, windowText);
        }
        public static IntPtr GetHwndFromClass(string className) {
            return (IntPtr)FindWindow(className, null);
        }

        public static bool ActivateWindow(IntPtr hwnd) {
            return SetForegroundWindow(hwnd);
        }

        public static bool MinimizeWindow(IntPtr hwnd) {
            return CloseWindow(hwnd);
        }
        #endregion

        #region Hwnd Attributes
        public static string GetHwndClassName(IntPtr hwnd) {
            var result = new StringBuilder(256);
            GetClassName(hwnd, result, result.MaxCapacity);
            return result.ToString();
        }

        public static string GetHwndTitle(IntPtr hwnd) {
            var length = GetHwndTitleLength(hwnd);
            var result = new StringBuilder(length + 1);
            GetWindowText(hwnd, result, result.Capacity);
            return result.ToString();
        }
        public static int    GetHwndTitleLength(IntPtr hwnd) {
            return GetWindowTextLength(hwnd);
        }
        public static bool   SetHwndTitle(IntPtr hwnd, string text) {
            return SetWindowText(hwnd, text);
        }

        public static string GetHwndText(IntPtr hwnd) {
            var len = (int)SendMessage(hwnd, (UInt32)WM.GETTEXTLENGTH, 0, 0) + 1;
            var sb = new StringBuilder(len);
            SendMessage(hwnd, (UInt32)WM.GETTEXT, (uint)len, sb);
            return sb.ToString();
        }
        public static void   SetHwndText(IntPtr hwnd, string text) {
            SendMessage(hwnd, (UInt32)WM.SETTEXT, 0, text);
        }

        //public static Point  GetHwndPos(IntPtr hwnd) {
        //    var rect = new RECT();
        //    GetWindowRect(hwnd, out rect);
        //    return new Point(rect.Left, rect.Top);
        //}
        //public static bool   SetHwndPos(IntPtr hwnd, int x, int y, int w, int h) {
        //    return SetWindowPos(hwnd, IntPtr.Zero, x, y, 0, 0, (uint)(PositioningFlags.SWP_NOSIZE | PositioningFlags.SWP_NOZORDER));
        //}

        //public static Size   GetHwndSize(IntPtr hwnd) {
        //    var rect = new RECT();
        //    GetWindowRect(hwnd, out rect);
        //    return new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
        //}
        //public static bool   SetHwndSize(IntPtr hwnd, int w, int h) {
        //    return SetWindowPos(hwnd, IntPtr.Zero, 0, 0, w, h, (uint)(PositioningFlags.SWP_NOMOVE | PositioningFlags.SWP_NOZORDER));
        //}

        public static RECT   GetHwndPlacementRect(IntPtr hwnd) {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT(null);
            GetWindowPlacement(hwnd, out placement);
            return placement.rcNormalPosition;
        }
        public static bool   SetHwndPlacementRect(IntPtr hwnd, RECT rect) {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT(null);
            placement.flags   = WINDOWPLACEMENT_Flag.WPF_ASYNCWINDOWPLACEMENT;
            placement.showCmd = WINDOWPLACEMENT_ShowCmd.SW_SHOWNA;
            placement.rcNormalPosition = rect;
            return SetWindowPlacement(hwnd, ref placement);
        }
        #endregion

        #region Hwnd Functions
        public delegate IntPtr GetWindowNext(IntPtr hwnd, IntPtr child);
        public static GetWindowNext GetNextChild   = (hwnd, child) => FindWindowEx(hwnd, child, null, null);
        public static GetWindowNext GetNextSibling = (hwnd, child) => GetWindow(child, GetWindow_Cmd.GW_HWNDNEXT);

        public static List<IntPtr> EnumChildren(IntPtr hwnd) {
            return EnumWindows(hwnd, IntPtr.Zero, GetNextChild);
        }
        public static List<IntPtr> EnumSiblings(IntPtr hwnd) {
            IntPtr sibling0 = GetWindow(hwnd, GetWindow_Cmd.GW_HWNDFIRST);
            return EnumWindows(hwnd, sibling0, GetNextSibling);
        }
        public static List<IntPtr> EnumWindows(IntPtr hwnd, IntPtr child0, GetWindowNext getWindowNext) {
            var child = child0;
            var results = new List<IntPtr>();
            do {
                if(child != IntPtr.Zero) {
                    results.Add(child);
                }
                child = getWindowNext(hwnd, child);
            } while(child != IntPtr.Zero);
            return results;
        }

        public static IntPtr GetHwndChild(IntPtr hwnd, string clsName, string ctrlText) {
            return FindWindowEx(hwnd, IntPtr.Zero, clsName, ctrlText);
        }
        public static IntPtr GetHwndOwner(IntPtr hwnd) {
            return GetWindow(hwnd, GetWindow_Cmd.GW_OWNER);
        }
        public static WINDOWINFO GetHwndInfo(IntPtr hwnd) {
            WINDOWINFO wi = new WINDOWINFO(null);
            GetWindowInfo(hwnd, ref wi);
            return wi;
        }
        public static IntPtr GetHwndParent(IntPtr hwnd) {
            return GetParent(hwnd);
        }
        public static uint   GetHwndProcessID(IntPtr hwnd) {
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            return pid;
        }        
        public static void   ClickHwnd(IntPtr hwnd) {
            SendMessage(hwnd, (uint)WM.BN_CLICKED, IntPtr.Zero, IntPtr.Zero);
        }
        #endregion

        #region Messaging functions
        public static int GetMessageInt(IntPtr hwnd, WM msg) {
            return (int)SendMessage(hwnd, (uint)msg, 0, 0);
        }
        public static string GetMessageString(IntPtr hwnd, WM msg, uint param) {
            var sb = new StringBuilder(65536);
            SendMessage(hwnd, (uint)msg, param, sb);
            return sb.ToString();
        }
        public static int    SendMessage(IntPtr hwnd, WM msg, uint param1, uint param2) {
            return (int)SendMessage(hwnd, (uint)msg, param1, param2);
        }
        public static int    SendMessage(IntPtr hwnd, WM msg, uint param1, string param2) {
            return (int)SendMessage(hwnd, (uint)msg, param1, param2);
        }
        #endregion

        #region Windows API functions (alphabetically ordered)
        [DllImport("user32.dll")]
        private static extern bool   CloseWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int    FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string windowTitle);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int    GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindow(IntPtr hWnd, GetWindow_Cmd uCmd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool   GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool   GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool   GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int    GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int    GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint   GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool   SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool   SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        private static extern bool   SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("USER32.DLL")]
        private static extern bool   SetWindowText(IntPtr hWnd, string lpString);
        #endregion

        #region Windows API Messaging functions
        // Standard interface
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        // Sending messages by string
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, uint wParam, string lParam);

        // Retrieving string data
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, uint wParam, StringBuilder lParam);

        // Retrieving numeric data
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, uint wParam, uint lParam);
        #endregion
    }
}
