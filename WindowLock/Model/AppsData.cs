using HelperLib.Caching;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WindowScrape.Types;

namespace WindowLock.Model {
    public class AppsData {
        protected Settings Settings;
        protected IDictionary<int, AppData> apps = new Dictionary<int, AppData>();
        /// <summary>
        /// Process.GetProcessById() is very CPU intensive => caching
        /// </summary>
        protected UsageCache<uint, Process> procCache = new UsageCache<uint, Process>((uint pid) => { return Process.GetProcessById((int)pid); });
        protected Process procFirstVisible = null;

        public AppsData(Settings settings) {
            Settings = settings;
        }

        public bool Load(bool force=false) {
            ResetPresent();
            bool isChanged = LoadWindows();
            isChanged |= RemoveOld();
            if(force || isChanged) {
                ResetMainHwnds();
                UpdateMainHwnds();
            }
            return isChanged;
        }

        public void Move() {
            TraverseAppData(move);
        }

        protected Process GetProcessFirstVisible() {
            if(procFirstVisible == null || procFirstVisible.HasExited) {
                //procFirstVisible = Process.GetCurrentProcess(); // does not work
                foreach(var proc in Process.GetProcesses()) {
                    //hwndObject = GetInstance(proc.MainWindowHandle);
                    if(!string.IsNullOrEmpty(proc.MainWindowTitle)) {// && hwndObject.GetOwner().Hwnd.ToInt32() == 0) {
                        procFirstVisible = proc;
                        break;
                    }
                }
            }
            return procFirstVisible;
        }

        protected bool LoadWindows() {
            HwndObject.CacheResetUsed();
            Process proc = GetProcessFirstVisible();
            List<HwndObject> hwndObjs = null;
            if(proc != null) {
                HwndObject hwndObject = HwndObject.GetInstance(proc.MainWindowHandle);
                hwndObjs = hwndObject.GetSiblings();
            } else {
                hwndObjs = HwndObject.GetWindows();
            }

            procCache.ResetUsed();
            bool isChanged = false;
            AppData appData;
            foreach(var hwndObj in hwndObjs) {
                if(IsHwndSelectable(hwndObj)) {
                    proc = procCache.Get(hwndObj.PID);
                    if (IsProcSelectable(proc)) {
                        appData = CreateOrGetAppData(proc);
                        isChanged |= AddOrUpdateAppWinData(appData, hwndObj);
                    }
                }
            }
            procCache.ClearUnUsed();
            HwndObject.CacheClearUnUsed();
            return isChanged;
        }

        protected bool IsHwndSelectable(HwndObject hwndObj) {
            return hwndObj.GetOwner().Hwnd.ToInt32() == 0 && hwndObj.Info.IsVisible();
        }

        protected bool IsProcSelectable(Process proc) {
            // the explorer windows process has empty MainWindowTitle
            //if(!string.IsNullOrEmpty(proc.MainWindowTitle)) {
            foreach(Regex filter in Settings.Filters) {
                if(filter.IsMatch(proc.ProcessName)) {
                    return true;
                }
            }
            //}
            return false;
        }

        /*protected bool LoadChildWindows(AppData appData, HwndObject hwndParent) {
            List<HwndObject> children = hwndParent.GetChildren();
            bool isChanged = false;
            foreach(var hwndChild in children) {
                if(!hwndParent.Equals(hwndChild) && !string.IsNullOrEmpty(hwndChild.Title)) {
                    isChanged |= AddOrUpdateAppWinData(appData, hwndChild);
                    //isChanged |= LoadChildWindows(appData, hwndChild); // recursive
                }
            }
            return isChanged;
        }*/

        #region AppData-tree building
        protected AppData CreateOrGetAppData(Process proc) {
            AppData appData;
            if(apps.ContainsKey(proc.Id)) {
                appData = apps[proc.Id];
            } else {
                appData = new AppData(proc.ProcessName, proc.Id, proc.MainWindowTitle);
                apps.Add(proc.Id, appData);
            }
            return appData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appData"></param>
        /// <param name="hwndObj"></param>
        /// <returns>true if a new AppWinData object was created</returns>
        protected bool AddOrUpdateAppWinData(AppData appData, HwndObject hwndObj) {
            if(appData.Windows.ContainsKey(hwndObj)) {
                return UpdateAppWinData(appData.Windows[hwndObj], hwndObj);
            }
            appData.Windows.Add(hwndObj, CreateAppWinData(hwndObj));
            return true;
        }

        protected AppWinData CreateAppWinData(HwndObject hwndObj) {
            string title = GetTitle(hwndObj);
            return new AppWinData(hwndObj, title);
        }
        protected bool UpdateAppWinData(AppWinData appWinData, HwndObject hwndObj) {
            string title = GetTitle(hwndObj);
            appWinData.IsPresent = true;
            appWinData.WinTitle = title;
            return hwndObj.ResetPosition();
        }

        protected string GetTitle(HwndObject hwndObj) {
            //String title = ""; //= hwndObj.GetOwner().Hwnd.ToString() + "_";
            //title += "0x" + ((int)hwndObj.Info.dwStyle).ToString("X8") + "_";
            //title += "0x" + ((int)hwndObj.Info.dwExStyle).ToString("X8") + "_";
            return hwndObj.Title; //title + hwndObj.Title;
        }
        #endregion

        #region AppData refresh
        protected delegate void ProcessAppWinData(AppData appData, AppWinData appWinData);
        
        protected static ProcessAppWinData resetPresent = (appData, appWinData) => {
            appWinData.IsPresent = false;
        };

        protected static ProcessAppWinData resetMainHwnds  = (appData, appWinData) => { appData.ClearMainWindows(); };
        protected static ProcessAppWinData updateMainHwnds = (appData, appWinData) => { appData.UpdateMainWindows(appWinData); };
        protected static ProcessAppWinData move            = (appData, appWinData) => { appData.Move(appWinData); };

        protected void TraverseAppData(ProcessAppWinData processAppWinData) {
            foreach(AppData appData in apps.Values) {
                foreach(AppWinData appWinData in appData.Windows.Values) {
                    processAppWinData(appData, appWinData);
                }
            }
        }

        protected void ResetPresent() {
            TraverseAppData(resetPresent);
        }
        protected void ResetMainHwnds() {
            TraverseAppData(resetMainHwnds);
        }
        protected void UpdateMainHwnds() {
            TraverseAppData(updateMainHwnds);
        }
        protected bool RemoveOld() {
            bool isChanged = false;
            KeyValuePair<HwndObject, AppWinData> e;
            foreach(AppData appData in apps.Values) {
                for(int i=0; i<appData.Windows.Count; i++) {
                    e = appData.Windows.ElementAt(i);
                    if(!e.Value.IsPresent) {
                        isChanged = true;
                        appData.Windows.Remove(e.Key);
                        i--;
                    }
                }
            }
            return isChanged;
        }
        #endregion

        public List<AppInfo> ToAppInfos() {
            List<AppInfo> appInfos = new List<AppInfo>();
            ProcessAppWinData addAppInfo = (appData, appWinData) => {
                if(Properties.Settings.Default.IsShowSingle || appData.Windows.Count > 1) {
                    appInfos.Add(new AppInfo(appData, appWinData));
                }
            };
            TraverseAppData(addAppInfo);
            return appInfos;
        }
    }
}
