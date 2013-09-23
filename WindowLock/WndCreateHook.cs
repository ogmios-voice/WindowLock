using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using WindowScrape.Constants;

namespace WindowLock {
    /// <summary>
    /// Separate hooks are required for the 32 bit and for the 64 bit applications
    /// </summary>
    /// <see cref="http://www.pinvoke.net/default.aspx/user32/SetWindowsHookEx.html"/>
    /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644990(v=vs.85).aspx"/>
    /// <see cref="http://stackoverflow.com/questions/1024756/how-can-i-be-notified-when-a-new-window-is-created-on-win32"/>
    public class WndCreateHook {
        //private IntPtr hWrapperInst = IntPtr.Zero;

        protected const int MSG_WIN_POS_LOCK = (int)WM.USER;

        ~WndCreateHook() {
            UnregisterHook();
        }

        public void RegisterHook(IntPtr hwnd) {
            //hWrapperInst = LoadLibrary("WinHookLib32.dll");
            if(!SetHook(hwnd)) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        public void UnregisterHook() {
            if(!Unhook()) {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            //if(hWrapperInst != IntPtr.Zero && !FreeLibrary(hWrapperInst)) {
            //    throw new Win32Exception();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        /// <see cref="http://stackoverflow.com/questions/624367/how-to-handle-wndproc-messages-in-wpf"/>
        /// <see cref="http://social.msdn.microsoft.com/Forums/vstudio/en-US/6b97a6de-0480-4339-8ed0-cb7cdb27bd83/wpf-and-wndproc-question"/>
        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch(msg) {
            case MSG_WIN_POS_LOCK:
                //System.Diagnostics.Debug.WriteLine("WndProc: Message received: " + wParam + " - " + lParam);
                ((App)Application.Current).RefreshApps();
                ((App)Application.Current).MoveWindows();
                break;
            }
            return IntPtr.Zero;
        }

        #region Windows API Hook functions
        //[DllImport("kernel32.dll")]
        //private static extern IntPtr LoadLibrary(string lpFileName);
        //[DllImport("kernel32.dll", SetLastError = true)]
        //private static extern bool   FreeLibrary(IntPtr hModule);

        [DllImport("WinHookLib32.dll")]
        private static extern bool SetHook(IntPtr hApp);
        [DllImport("WinHookLib32.dll")]
        private static extern bool Unhook();
        #endregion
    }
}
