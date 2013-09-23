using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowScrape.Constants {
    /// <summary>
    ///
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/windows/desktop/ms633515(v=vs.85).aspx">GetWindow</see>
    /// <see cref="http://www.pinvoke.net/default.aspx/user32/GetWindow.html"/>
    public enum GetWindow_Cmd : uint {
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6,
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
    }
}
