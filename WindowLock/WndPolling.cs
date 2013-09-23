using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using WindowScrape.Constants;
using WindowScrape.Static;

namespace WindowLock {
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="http://msdn.microsoft.com/en-us/library/system.threading.timer.aspx"/>
    /// <see cref="http://msdn.microsoft.com/en-us/library/system.timers.timer.aspx"/>
    /// <see cref="http://msdn.microsoft.com/en-us/library/0tcs6ww8"/>
    public class WndPolling {
        protected System.Threading.Timer Timer;

        public WndPolling() {
            CreateTimer();
        }
        ~WndPolling() {
        }

        #region Timer
        protected void CreateTimer() {
            Timer = new System.Threading.Timer(new System.Threading.TimerCallback(OnTimedEvent));
        }

        public void Enable() {
            Timer.Change(Properties.Settings.Default.RefreshMillis, Properties.Settings.Default.RefreshMillis);
        }
        public void Disable() {
            Timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }
        #endregion

        //private void OnTimedEvent(object source, ElapsedEventArgs e) {
            //Console.WriteLine("The Elapsed event was raised at {0}", e.SignalTime);
        private void OnTimedEvent(object stateInfo) {
            bool isChanged = ((App)Application.Current).RefreshApps();
            ((App)Application.Current).MoveWindows();
        }
    }
}
