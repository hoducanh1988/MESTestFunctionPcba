using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW12SG.Function.Custom {

    public class ManualCheckButtonInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ManualCheckButtonInfo() {
            logUart = "";
            LegendWps = "-";
            LegendReset = "-";
        }

        string _log_uart;
        public string logUart {
            get { return _log_uart; }
            set {
                _log_uart = value;
                OnPropertyChanged(nameof(logUart));
            }
        }
        string _legend_check_wps;
        public string LegendWps {
            get { return _legend_check_wps; }
            set {
                _legend_check_wps = value;
                OnPropertyChanged(nameof(LegendWps));
            }
        }
        string _legend_check_reset;
        public string LegendReset {
            get { return _legend_check_reset; }
            set {
                _legend_check_reset = value;
                OnPropertyChanged(nameof(LegendReset));
            }
        }

    }
}
