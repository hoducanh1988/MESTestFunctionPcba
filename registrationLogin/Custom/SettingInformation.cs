using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace registrationLogin.Custom {

    public class SettingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public SettingInformation() {
            ModelName = "";
            StationName = "";
            JigNumber = "";
        }


        string _model_name;
        public string ModelName {
            get { return _model_name; }
            set {
                _model_name = value;
                OnPropertyChanged(nameof(ModelName));
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
        string _jig_number;
        public string JigNumber {
            get { return _jig_number; }
            set {
                _jig_number = value;
                OnPropertyChanged(nameof(JigNumber));
            }
        }


    }
}
