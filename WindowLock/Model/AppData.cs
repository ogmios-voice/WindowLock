using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using WindowScrape.Types;

namespace WindowLock.Model {
    public class AppData {
        public string     ProcName { get; protected set; }
        public int        PID { get; protected set; }
        public string     ProcTitle { get; set; }
        public IDictionary<HwndObject, AppWinData> Windows { get; protected set; }
        protected List<HwndObject> MainHwndObjs { get; set; }

        public AppData(string procName, int pID, string procTitle) {
            ProcName = procName;
            PID = pID;
            ProcTitle = procTitle;
            MainHwndObjs = new List<HwndObject>();
            Windows = new Dictionary<HwndObject, AppWinData>();
        }

        public void ClearMainWindows() {
            MainHwndObjs.Clear();
        }

        public bool IsMainWindow(AppWinData appWinData) {
            return MainHwndObjs.Contains(appWinData.HwndObj);
        }

        public void UpdateMainWindows(AppWinData appWinData) {
            if(MainHwndObjs.Count == 0) {
                MainHwndObjs.Add(appWinData.HwndObj);
            } else {
                int mainHwndIdx = IdxOfMainWindowInRange(appWinData.HwndObj);
                if(mainHwndIdx < 0) {
                    MainHwndObjs.Add(appWinData.HwndObj);
                } else if(IsWindowOnLeft(MainHwndObjs[mainHwndIdx], appWinData.HwndObj)) {
                    MainHwndObjs[mainHwndIdx] = appWinData.HwndObj;
                }
            }
        }

        /// <summary>
        /// Searches for a main window in the predifined vicinity of the given window.
        /// Looks for windows of the same size only.
        /// </summary>
        /// <param name="hwndObj"></param>
        /// <returns></returns>
        protected int IdxOfMainWindowInRange(HwndObject hwndObj) {
            for(int i=0; i<MainHwndObjs.Count; i++) {
                if(IsWindowSizeInRange(MainHwndObjs[i], hwndObj) && IsWindowInRange(MainHwndObjs[i], hwndObj)) {
                    return i;
                }
            }
            return -1;
        }

        protected bool IsWindowSizeInRange(HwndObject mainHwndObj, HwndObject hwndObj) {
            return Math.Abs(hwndObj.Rect.Width  - mainHwndObj.Rect.Width)  <= Properties.Settings.Default.SizeDX
                && Math.Abs(hwndObj.Rect.Height - mainHwndObj.Rect.Height) <= Properties.Settings.Default.SizeDY;
        }
        protected bool IsWindowMisplaced(HwndObject mainHwndObj, HwndObject hwndObj) {
            return mainHwndObj.Rect.X != hwndObj.Rect.X
                || mainHwndObj.Rect.Y != hwndObj.Rect.Y;
        }
        protected bool IsWindowInRange(HwndObject mainHwndObj, HwndObject hwndObj) {
            return Math.Abs(hwndObj.Rect.X - mainHwndObj.Rect.X) <= Properties.Settings.Default.PosDX
                && Math.Abs(hwndObj.Rect.Y - mainHwndObj.Rect.Y) <= Properties.Settings.Default.PosDY;
        }
        protected bool IsWindowOnLeft(HwndObject mainHwndObj, HwndObject hwndObj) {
            return hwndObj.Rect.X < mainHwndObj.Rect.X;
        }
        protected bool IsWindowOnLeft(AppWinData appWinDataMain, AppWinData appWinData) {
            return appWinData.HwndObj.Rect.X < appWinDataMain.HwndObj.Rect.X;
        }

        public void Move(AppWinData appWinData) {
            if(Windows.Count > 1 && !MainHwndObjs.Contains(appWinData.HwndObj)) {
                int mainHwndIdx = IdxOfMainWindowInRange(appWinData.HwndObj); // index will always be valid
                HwndObject mainHwndObj = MainHwndObjs[mainHwndIdx];
                if(IsWindowMisplaced(mainHwndObj, appWinData.HwndObj)) {
                    appWinData.HwndObj.Location = mainHwndObj.Location;
                }
            }
        }
    }
}
