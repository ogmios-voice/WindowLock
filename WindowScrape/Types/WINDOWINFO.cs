using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using WindowScrape.Constants;

namespace WindowScrape.Types {
    /// <summary>
    ///
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633516(v=vs.85).aspx"/>
    /// <see cref="http://www.pinvoke.net/default.aspx/user32/GetWindowInfo.html"/>
    /// <see cref="http://pinvoke.net/default.aspx/Structures.WINDOWINFO"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWINFO {
        public static readonly WINDOWINFO Empty;

        public uint   cbSize;
        public RECT   rcWindow;
        public RECT   rcClient;
        public WindowStyles         dwStyle;
        public ExtendedWindowStyles dwExStyle;
        public uint   dwWindowStatus;
        public uint   cxWindowBorders;
        public uint   cyWindowBorders;
        public ushort atomWindowType;
        public ushort wCreatorVersion;

        /// <summary>
        /// Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
        /// </summary>
        /// <param name="filler"></param>
        public WINDOWINFO(Boolean ? filler) : this() {  
            cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
        }

        public bool IsVisible() {
            //return ((int)dwStyle & (int)WindowStyles.WS_VISIBLE) != 0;
            return dwStyle.HasFlag(WindowStyles.WS_VISIBLE);
        }

        public bool IsOverlappedWindow() {
            //return ((int)dwStyle & (int)WindowStyles.WS_OVERLAPPEDWINDOW) != 0;
            return dwStyle.HasFlag(WindowStyles.WS_OVERLAPPEDWINDOW);
        }
    }
}
