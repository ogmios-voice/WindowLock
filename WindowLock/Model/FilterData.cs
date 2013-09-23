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
    /// TODO: Check DataGrid whether it can access List&lt;string&gt; structure directly
    public class FilterData : INotifyPropertyChanged {
        protected string value;
        public string Value {
            get { return value; }
            set {
                if(this.value != value) { 
                    this.value = value;
                    OnPropertyChanged(value);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public FilterData(string value) {
            Value = value;
        }

        protected void OnPropertyChanged(string name) {
            if(PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
