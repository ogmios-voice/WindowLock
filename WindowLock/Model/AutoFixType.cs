using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowLock.Model {
    public enum AutoFixType {
        [Description("None")]
        None,
        [Description("Windows Hook")]
        WindowsHook,
        [Description("Window Polling")]
        WindowPolling,
        [Description("All")]
        All
    }
}
