using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowScrape.Constants {
    /// <summary>
    ///
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644990(v=vs.85).aspx">SetWindowsHookEx</see>
    /// <see cref="http://www.pinvoke.net/default.aspx/Enums/HookType.html"/>
    public enum SetWindowsHookEx_HookType : int {
        WH_CALLWNDPROC = 4,
        WH_CALLWNDPROCRET = 12,
        WH_CBT = 5,
        WH_DEBUG = 9,
        WH_FOREGROUNDIDLE = 11,
        WH_GETMESSAGE = 3,
        WH_JOURNALPLAYBACK = 1,
        WH_JOURNALRECORD = 0,
        WH_KEYBOARD = 2,
        WH_KEYBOARD_LL = 13,
        WH_MOUSE = 7,
        WH_MOUSE_LL = 14,
        WH_MSGFILTER = -1,
        WH_SHELL = 10,
        WH_SYSMSGFILTER = 6,
    }
}
