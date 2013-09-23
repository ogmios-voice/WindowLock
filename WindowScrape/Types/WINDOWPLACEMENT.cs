using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WindowScrape.Constants;

namespace WindowScrape.Types {
    /// <summary>
    /// Contains information about the placement of a window on the screen.
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633518(v=vs.85).aspx"/>
    /// <see cref="http://www.pinvoke.net/default.aspx/user32/GetWindowPlacement.html"/>
    /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms632611(v=vs.85).aspx"/>
    /// <see cref="http://www.pinvoke.net/default.aspx/Structures/WINDOWPLACEMENT.html"/>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT {
        public uint  length;
        public WINDOWPLACEMENT_Flag    flags;
        public WINDOWPLACEMENT_ShowCmd showCmd;
        public POINT ptMinPosition;
        public POINT ptMaxPosition;
        public RECT  rcNormalPosition;

        /// <summary>
        /// Allows automatic initialization of "length" with "new WINDOWPLACEMENT(null/true/false)".
        /// </summary>
        /// <param name="filler"></param>
        public WINDOWPLACEMENT(Boolean? filler) : this() {
            length = (uint)Marshal.SizeOf(typeof(WINDOWPLACEMENT));
        }
    }

    [Flags]
    public enum WINDOWPLACEMENT_Flag : uint {
        WPF_ASYNCWINDOWPLACEMENT = 0x0004,
        WPF_RESTORETOMAXIMIZED = 0x0002,
        WPF_SETMINPOSITION = 0x0001,
    }

    [Flags]
    public enum WINDOWPLACEMENT_ShowCmd : uint {
        SW_HIDE = 0,
        SW_MAXIMIZE = 3,
        SW_MINIMIZE = 6,
        SW_RESTORE = 9,
        SW_SHOW = 5,
        SW_SHOWMAXIMIZED = 3,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOWNORMAL = 1,
    }
}
