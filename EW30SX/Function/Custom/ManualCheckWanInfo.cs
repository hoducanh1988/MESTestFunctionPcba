using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW30SX.Function.Custom {

    public class ManualCheckWanInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ManualCheckWanInfo() {
            logUart = "";
        }

        string _log_uart;
        public string logUart {
            get { return _log_uart; }
            set {
                _log_uart = value;
                OnPropertyChanged(nameof(logUart));
            }
        }
    }
}
