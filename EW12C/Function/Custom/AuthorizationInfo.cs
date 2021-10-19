using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW12C.Function.Custom {

    public class AuthorizationInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string _user;
        public string User {
            get { return _user; }
            set {
                _user = value;
                OnPropertyChanged(nameof(User));
            }
        }
        string _pass;
        public string Password {
            get { return _pass; }
            set {
                _pass = value;
                OnPropertyChanged(nameof(Password));
            }
        }
        
    }
}
