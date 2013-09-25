using HelperLib.Caching;
using Point = System.Drawing.Point;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using WindowScrape.Constants;
using WindowScrape.Static;

namespace WindowScrape.Types {
    /// <summary>
    /// Allows the searching, navigation, and manipulation of Hwnd objects.
    /// </summary>
    public class HwndObject {
        /// <summary>
        /// 
        /// </summary>
        public static readonly HwndObject None = new HwndObject(IntPtr.Zero);

        protected static UsageCache<IntPtr, HwndObject> cache = new UsageCache<IntPtr, HwndObject>((IntPtr hwnd) => { return new HwndObject(hwnd); });

        /// <summary>
        /// The windows handle to this object.
        /// </summary>
        public IntPtr Hwnd { get; private set; }
        protected uint pid;
        protected WINDOWINFO info;
        protected RECT rect;
        protected Point location;
        protected Size size;

        #region Property Getters and Setters
        /// <summary>
        /// The registered class name (if any) of this object.
        /// </summary>
        public string ClassName {
            get { return HwndInterface.GetHwndClassName(Hwnd); }
        }

        /// <summary>
        /// The title of this object - Setting this will only effect window title-bar text.
        /// </summary>
        public string Title {
            get { return HwndInterface.GetHwndTitle(Hwnd); }
            set { HwndInterface.SetHwndTitle(Hwnd, value); }
        }

        /// <summary>
        /// The text of this item - setting this will only effect controls and only with appropriate access/privacy
        /// </summary>
        public string Text {
            get { return HwndInterface.GetHwndText(Hwnd); }
            set { HwndInterface.SetHwndText(Hwnd, value); }
        }

        public uint PID {
            get {
                if(pid == 0) {
                    pid = GetProcessID();
                }
                return pid;
            }
            private set { pid = value; }
        }

        public WINDOWINFO Info {
            get {
                if(info.Equals(WINDOWINFO.Empty)) {
                    info = HwndInterface.GetHwndInfo(Hwnd);
                }
                return info;
            }
            private set { info = value; }
        }

        public RECT Rect {
            get {
                if(rect == RECT.Empty) {
                    rect = HwndInterface.GetHwndPlacementRect(Hwnd);
                }
                return rect;
            }
            set {
                rect = new RECT(value);
                HwndInterface.SetHwndPlacementRect(Hwnd, rect);
            }
        }

        /// <summary>
        /// The location of this Hwnd Object.
        /// </summary>
        public Point Location {
            //get { return HwndInterface.GetHwndPos(Hwnd); }
            //set { HwndInterface.SetHwndPos(Hwnd, (int)value.X, (int)value.Y, (int)Size.Width, (int)Size.Height); }
            get {
                if(location == Point.Empty) {
                    location = new Point(Rect.Left, Rect.Top);
                }
                return location;
            }
            set {
                rect.X = value.X;
                rect.Y = value.Y;
                HwndInterface.SetHwndPlacementRect(Hwnd, rect);
            }
        }

        /// <summary>
        /// The size of this Hwnd Object.
        /// </summary>
        public Size Size {
            //get { return HwndInterface.GetHwndSize(Hwnd); }
            //set { HwndInterface.SetHwndSize(Hwnd, (int)value.Width, (int)value.Height); }
            get {
                if(size == Size.Empty) {
                    size = new Size(Rect.Right - Rect.Left, Rect.Bottom - Rect.Top);
                }
                return size;
            }
            set {
                rect.Width = value.Width;
                rect.Height = value.Height;
                HwndInterface.SetHwndPlacementRect(Hwnd, rect);
            }
        }
        #endregion

        /// <summary>
        /// Get HwndObject from cache or create a new instance if not found.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        public static HwndObject GetInstance(IntPtr hwnd) {
            return cache.Get(hwnd);
        }

        #region Caching
        public static void CacheResetUsed() {
            cache.ResetUsed();
        }
        public static void CacheClearUnUsed() {
            cache.ClearUnUsed();
        }
        #endregion

        /// <summary>
        /// Retrieves all top-level Hwnd Objects.
        /// </summary>
        /// <returns></returns>
        public static List<HwndObject> GetWindows() {
            return GetWindows(HwndInterface.EnumHwnds());
        }

        protected static List<HwndObject> GetWindows(List<IntPtr> hwnds) {
            var result = new List<HwndObject>();
            foreach(var hwnd in hwnds) {
                result.Add(GetInstance(hwnd));
            }
            return result;
        }

        /// <summary>
        /// Gets the first top-level HwndObject with the given title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public static HwndObject GetWindowByTitle(string title) {
            return GetInstance(HwndInterface.GetHwndFromTitle(title));
        }

        /// <summary>
        /// Initialized a GetInstance.
        /// </summary>
        /// <param name="hwnd"></param>
        protected HwndObject(IntPtr hwnd) {
            Hwnd = hwnd;
            pid = 0;
            info = WINDOWINFO.Empty;
            ResetPosition();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetInfo() {
            info = WINDOWINFO.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true on window coordinates change</returns>
        public bool ResetPosition() {
            RECT rectOld = rect;
            rect = RECT.Empty;
            location = Point.Empty;
            size = Size.Empty;
            return rectOld != Rect;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public uint GetProcessID() {
            return HwndInterface.GetHwndProcessID(Hwnd);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public HwndObject GetOwner() {
            return GetInstance(HwndInterface.GetHwndOwner(Hwnd));
        }

        /// <summary>
        /// Seeks a parent for this Hwnd Object (if any).
        /// </summary>
        /// <returns></returns>
        public HwndObject GetParent() {
            return GetInstance(HwndInterface.GetHwndParent(Hwnd));
        }

        /// <summary>
        /// Seeks all children of this Hwnd Object.
        /// </summary>
        /// <returns></returns>
        public List<HwndObject> GetChildren() {
            return GetWindows(HwndInterface.EnumChildren(Hwnd));
        }

        /// <summary>
        /// Seeks all children of this Hwnd Object.
        /// </summary>
        /// <returns></returns>
        public List<HwndObject> GetSiblings() {
            return GetWindows(HwndInterface.EnumSiblings(Hwnd));
        }

        /// <summary>
        /// Retrieves a child Hwnd Object by its class and title.
        /// </summary>
        /// <param name="cls"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public HwndObject GetChild(string cls, string title) {
            var hwnd = HwndInterface.GetHwndChild(Hwnd, cls, title);
            return GetInstance(hwnd);
        }

        public override string ToString() {
            var pt = Location;
            var sz = Size;
            var result =
                string.Format(
                    "({0}) {1},{2}:{3}x{4} \"{5}\"",
                    Hwnd,
                    pt.X, pt.Y,
                    sz.Width, sz.Height,
                    Title);
            return result;
        }

        #region Operators
        public static bool operator ==(HwndObject a, HwndObject b) {
            return (a.Hwnd == b.Hwnd);
        }

        public static bool operator !=(HwndObject a, HwndObject b) {
            return !(a == b);
        }

        public bool Equals(HwndObject obj) {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            return obj.Hwnd.Equals(Hwnd);
        }

        public override bool Equals(object obj) {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != typeof(HwndObject)) return false;
            return Equals((HwndObject)obj);
        }

        public override int GetHashCode() {
            return Hwnd.GetHashCode();
        }
        #endregion

        #region Messaging
        /// <summary>
        /// Sends a message to this Hwnd Object
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public void SendMessage(WM msg, uint param1, string param2) {
            HwndInterface.SendMessage(Hwnd, msg, param1, param2);
        }
        /// <summary>
        /// Sends a message to this Hwnd Object
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        public void SendMessage(WM msg, uint param1, uint param2) {
            HwndInterface.SendMessage(Hwnd, msg, param1, param2);
        }
        /// <summary>
        /// Returns a string result from a message.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public string GetMessageString(WM msg, uint param) {
            return HwndInterface.GetMessageString(Hwnd, msg, param);
        }
        /// <summary>
        /// Returns an integer result from a message.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public int GetMessageInt(WM msg) {
            return HwndInterface.GetMessageInt(Hwnd, msg);
        }
        #endregion

        #region UI
        /// <summary>
        /// Simulates a user-click on this object.
        /// </summary>
        public void Click() {
            HwndInterface.ClickHwnd(Hwnd);
        }
        #endregion
    }
}
