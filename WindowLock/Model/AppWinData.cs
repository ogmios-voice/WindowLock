using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowScrape.Types;

namespace WindowLock.Model {
    public class AppWinData {
        public HwndObject HwndObj { get; set; }
        public string     WinTitle { get; set; }
        public bool       IsPresent { get; set; }

        public AppWinData(HwndObject hwndObj, string winTitle) {
            HwndObj = hwndObj;
            WinTitle = winTitle;
            IsPresent = true;
        }
    }
}
