using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW12C.Function.Custom {

    public class MainWindowInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string _station_number;
        public string StationNumber {
            get { return _station_number; }
            set {
                _station_number = value;
                OnPropertyChanged(nameof(StationNumber));
            }
        }
        string _serial_port_name;
        public string SerialPortName {
            get { return _serial_port_name; }
            set {
                _serial_port_name = value;
                OnPropertyChanged(nameof(SerialPortName));
            }
        }
        string _station_name;
        public string StationName {
            get { return _station_name; }
            set {
                _station_name = value;
                OnPropertyChanged(nameof(StationName));
            }
        }



    }
}
