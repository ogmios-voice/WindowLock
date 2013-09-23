using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowLock.Model {
    /// <summary>
    /// 
    /// </summary>
    /// TODO: Check DataGrid whether it can access AppData/AppWinData structure directly
    public class AppInfo : INotifyPropertyChanged {
        public string ProcName { get; protected set; }
        public int    PID { get; protected set; }
        public string ProcTitle { get; protected set; }
        public string WinTitle { get; protected set; }
        public System.Drawing.Point Location { get; set; }
        public System.Drawing.Size Size { get; set; }
        public bool   IsMain { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInfo(AppData appData, AppWinData appWinData) {
            ProcName  = appData.ProcName;
            PID       = appData.PID;
            ProcTitle = appData.ProcTitle;
            WinTitle  = appWinData.WinTitle;
            Location  = appWinData.HwndObj.Location;
            Size      = appWinData.HwndObj.Size;
            IsMain    = appData.IsMainWindow(appWinData);
        }

        protected void OnPropertyChanged(string name) {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
