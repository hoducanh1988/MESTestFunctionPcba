using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EW12C.Function.Global;
using System.Threading;

namespace EW12C.UserCtrl {
    /// <summary>
    /// Interaction logic for ucButton.xaml
    /// </summary>
    public partial class ucButton : UserControl {
        public ucButton() {
            InitializeComponent();
            this.DataContext = myGlobal.myTesting;

            Thread t = new Thread(new ThreadStart(() => {
                _wait_button("wps", "WPS PUSH BUTTON EVENT DETECTED", myGlobal.mySetting.timeOutPressButtonWps);
                _wait_button("reset", "REBOOT", myGlobal.mySetting.timeOutPressButtonReset);
            }));
            t.IsBackground = true;
            t.Start();
        }

        private void _wait_button(string button_type, string recognition_text, int timeout) {
            int count = 0;
            bool r = false;
            RE:
            count++;
            r = myGlobal.myTesting.logUart.ToLower().Contains(recognition_text.ToLower());
            switch (button_type) {
                case "wps": {
                        myGlobal.myTesting.legendReset = "-";
                        myGlobal.myTesting.legendWps = string.Format("Vui lòng nhấn nút WPS (còn lại: {0} sec)", ((timeout * 2) - count) / 2);
                        break;
                    }
                case "reset": {
                        myGlobal.myTesting.legendReset = string.Format("Vui lòng nhấn nút RESET (còn lại: {0} sec)", ((timeout * 2) - count) / 2);
                        myGlobal.myTesting.legendWps = "-";
                        break;
                    }
            }

            if (!r) {
                if (count < timeout * 2) {
                    Thread.Sleep(500);
                    goto RE;
                }
            }

            switch (button_type) {
                case "wps": {
                        myGlobal.myTesting.buttonWps = r ? "Passed" : "Failed";
                        break;
                    }
                case "reset": {
                        myGlobal.myTesting.buttonReset = r ? "Passed" : "Failed";
                        break;
                    }
            }
        }
    }
}
