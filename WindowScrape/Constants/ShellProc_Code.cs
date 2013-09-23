using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowScrape.Constants {
    /// <summary>
    ///
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms644991(v=vs.85).aspx">ShellProc callback</see>
    public enum ShellProc_Code : int {
        HSHELL_ACCESSIBILITYSTATE = 11,
        HSHELL_ACTIVATESHELLWINDOW = 3,
        HSHELL_APPCOMMAND = 12,
        HSHELL_GETMINRECT = 5,
        HSHELL_LANGUAGE = 8,
        HSHELL_REDRAW = 6,
        HSHELL_TASKMAN = 7,
        HSHELL_WINDOWACTIVATED = 4,
        HSHELL_WINDOWCREATED = 1,
        HSHELL_WINDOWDESTROYED = 2,
        HSHELL_WINDOWREPLACED = 13,
    }
}