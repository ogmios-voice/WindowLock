using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WindowLock.Model;

namespace WindowLock {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        public Settings      Settings      { get; protected set; }
        public WndCreateHook WndCreateHook { get; protected set; }
        public WndPolling    WndPolling    { get; protected set; }
        protected AppsData AppsData;
        public ObservableCollection<AppInfo> AppInfos { get; protected set; }

        public App() : base() {
            EmbeddedAssemblyResolver.RegisterResolver();
        }

        void AppStartup(object sender, StartupEventArgs args) {
            Settings = new Settings();
            AppsData = new AppsData(Settings);
            AppInfos = new ObservableCollection<AppInfo>();
            Settings.LoadFilterData();
            Settings.RefreshFilters();
            //MainWindow mainWindow = new MainWindow();
            //mainWindow.Show();
            // AutoFix
            WndPolling    = new WndPolling();
            WndCreateHook = new WndCreateHook();
        }

        /// Saving User settings
        void AppExit(object sender, ExitEventArgs e) {
            Settings.SaveFilterData();
            WindowLock.Properties.Settings.Default.Save();
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public bool RefreshApps(bool force=false) {
            bool isChanged = AppsData.Load();
            if(force || isChanged) {
                Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(UpdateView));
            }
            return isChanged;
        }

        protected void UpdateView() {
            AppInfos.Clear();
            foreach(AppInfo appInfo in AppsData.ToAppInfos()) {
                AppInfos.Add(appInfo);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
        public void MoveWindows() {
            AppsData.Move();
        }

        public void AutoFixUpdate(Model.AutoFixType selection, IntPtr hwnd) {
            switch(selection) { // Properties.Settings.Default.AutoFix does not contain yet the new value
            case Model.AutoFixType.None:
                WndCreateHook.UnregisterHook();
                WndPolling.Disable();
                break;
            case Model.AutoFixType.WindowsHook:
                RefreshApps(true);
                WndCreateHook.RegisterHook(hwnd);
                WndPolling.Disable();
                break;
            case Model.AutoFixType.WindowPolling:
                RefreshApps(true);
                WndCreateHook.UnregisterHook();
                WndPolling.Enable();
                break;
            case Model.AutoFixType.All:
                RefreshApps(true);
                WndCreateHook.RegisterHook(hwnd);
                WndPolling.Enable();
                break;
            }
        }
    }
}
