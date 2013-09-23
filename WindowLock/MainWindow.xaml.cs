using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace WindowLock {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        /// <see cref="http://blogs.msdn.com/b/delay/archive/2009/08/31/get-out-of-the-way-with-the-tray-minimize-to-tray-sample-implementation-for-wpf.aspx"/>
        protected System.Windows.Forms.NotifyIcon NotifyIcon = null;
        protected IntPtr Hwnd = IntPtr.Zero;

        public MainWindow() {
            InitializeComponent();
            CreateSysTrayIcon();
        }
        ~MainWindow() {
            //DeInitWndHook(); // App is already destroyed here (with all of its fields)
            DestroySysTrayIcon();
        }

        #region WindowHook
        protected override void OnSourceInitialized(EventArgs e) {
            base.OnSourceInitialized(e);
            InitWndHook();
        }

        public void InitWndHook() {
            Hwnd = new WindowInteropHelper(this).Handle;  //TODO: why does not work with Process.GetCurrentProcess().MainWindowHandle?
            GetHwndSource().AddHook(((App)Application.Current).WndCreateHook.WndProc);
            ((App)Application.Current).WndCreateHook.RegisterHook(Hwnd);
        }

        public void DeInitWndHook() {
            ((App)Application.Current).WndCreateHook.UnregisterHook();
            GetHwndSource().RemoveHook(((App)Application.Current).WndCreateHook.WndProc);
        }

        protected HwndSource GetHwndSource() {
            return PresentationSource.FromVisual(this) as HwndSource;
        }
        #endregion

        #region System Tray
        protected void CreateSysTrayIcon() {
            NotifyIcon = new System.Windows.Forms.NotifyIcon();
            //NotifyIcon.Icon = new System.Drawing.Icon(Properties.Settings.Default.IconFile);
            Stream imageStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(Properties.Settings.Default.SysTrayIcon);
            NotifyIcon.Icon = new System.Drawing.Icon(imageStream); // new System.Drawing.Size(32, 32)
            NotifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(SysTrayShowPopup);
            NotifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(SysTrayRestore);
        }
        protected void DestroySysTrayIcon() {
            NotifyIcon.Visible = false;
            NotifyIcon = null;
        }

        protected override void OnStateChanged(EventArgs e) {
            if(Properties.Settings.Default.IsMinSysTray) {
                var minimized = WindowState == WindowState.Minimized;
                NotifyIcon.Visible = minimized;
                if(minimized) {
                    Hide();
                }
            }
            base.OnStateChanged(e);
        }
        protected void SysTrayShowPopup(object sender, System.Windows.Forms.MouseEventArgs e) {
            ContextMenu sysTrayPopup = GetSysTrayPopup();
            //sysTrayPopup.PlacementTarget = NotifyIcon;
            if(e.Button == System.Windows.Forms.MouseButtons.Right) {
                sysTrayPopup.IsOpen = true;
            } else if(sysTrayPopup.IsOpen) {
                sysTrayPopup.IsOpen = false;
            }
        }
        protected void SysTrayRestore(object sender, System.Windows.Forms.MouseEventArgs e) {
            if(e == null || e.Button == System.Windows.Forms.MouseButtons.Left) {
                GetSysTrayPopup().IsOpen = false;
                Show();
                WindowState = WindowState.Normal;
            }
        }

        private void SysTrayRestoreClick(object sender, RoutedEventArgs e) {
            SysTrayRestore(sender, null);
        }
        private void SysTrayExitClick(object sender, RoutedEventArgs e) {
            NotifyIcon.Visible = false;
            ((App)Application.Current).Shutdown();
        }

        private System.Windows.Controls.ContextMenu GetSysTrayPopup() {
            return (ContextMenu)Resources["sysTrayPopup"];
        }
        #endregion

        #region Toolbar actions
        private void RefreshClick(object sender, RoutedEventArgs e) {
            ((App)Application.Current).RefreshApps(true);
        }

        private void MoveClick(object sender, RoutedEventArgs e) {
            ((App)Application.Current).RefreshApps(true); // load current windows
            ((App)Application.Current).MoveWindows();
            ((App)Application.Current).RefreshApps(true); // udpate UI after position/size fix
        }
        #endregion

        #region Settings
        private void AutoFix_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //Model.AutoFixType field = (Model.AutoFixType)e.AddedItems[0];
            Model.AutoFixType field = HelperLib.Presentation.EnumListConverter.GetItem<Model.AutoFixType>((string)e.AddedItems[0]);
            ((App)Application.Current).AutoFixUpdate(field, Hwnd);
        }

        private void ShowSingle_Check(object sender, RoutedEventArgs e) {
            //Properties.Settings.Default.IsShowSingle = true;
            ((App)Application.Current).RefreshApps(true);
        }
        private void ShowSingle_Uncheck(object sender, RoutedEventArgs e) {
            //Properties.Settings.Default.IsShowSingle = false;
            ((App)Application.Current).RefreshApps(true);
        }

        private void Filters_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            DataGrid dg = sender as DataGrid;
            if(dg != null) {
                DataGridRow dgr = (DataGridRow)(dg.ItemContainerGenerator.ContainerFromIndex(dg.SelectedIndex));
                if(!dgr.IsEditing) {
                    switch(e.Key) {
                    //case Key.Delete:
                    //    ((App)Application.Current).Settings.RefreshFilters();
                    //    break;
                    case Key.Insert:
                        int index = dgr.GetIndex();
                        //if((e.Modifiers & Keys.Shift) != 0)
                        if(e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift)) {
                            index--; // insert before
                        }
                        ((App)Application.Current).Settings.AddEmptyFilterDataAfter(index < -1 ? -1 : index);
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
