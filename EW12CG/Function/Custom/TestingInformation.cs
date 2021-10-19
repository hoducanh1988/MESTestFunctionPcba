using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EW12CG.Function.Custom {

    public class TestingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //constructor
        public TestingInformation() {
            initParams();
        }

        //method
        public bool initParams() {

            MacAddress = "";

            changeIP = "-";
            writeMacLan = "-";
            writeHardwareVersion = "-";
            writeModelNumber = "-";
            EraseCalibWlan = "-";
            uploadFirmwareBasic = "-";
            checkMacEthernet = "-";
            checkQCA9886 = "-";
            checkFwVersion = "-";
            checkFwBuildTime = "-";
            checkWanPort = "-";
            checkLED = "-";
            ledWan = "-";
            checkButton = "-";
            buttonWps = "-";
            buttonReset = "-";
            legendWps = "-";
            legendReset = "-";
            totalResult = "-";

            logSystem = "";
            logUart = "";
            errorMessage = "";
            errorCode = "";

            return true;
        }

        public bool waitParams() {
            MacAddress = "";

            changeIP = "-";
            writeMacLan = "-";
            writeHardwareVersion = "-";
            writeModelNumber = "-";
            EraseCalibWlan = "-";
            uploadFirmwareBasic = "-";
            checkMacEthernet = "-";
            checkQCA9886 = "-";
            checkFwVersion = "-";
            checkFwBuildTime = "-";
            checkWanPort = "-";
            checkLED = "-";
            ledWan = "-";
            checkButton = "-";
            buttonWps = "-";
            buttonReset = "-";
            legendWps = "-";
            legendReset = "-";
            totalResult = "Waiting...";

            logSystem = "";
            logUart = "";
            errorMessage = "";
            errorCode = "";

            return true;
        }

        public bool passParams() {
            totalResult = "Passed";
            errorMessage = "Product is ok";
            return true;
        }

        public bool failParams() {
            totalResult = "Failed";
            return true;
        }

        //property
        #region //TEST VARIABLE ---------------------------//

        string _mac;
        public string MacAddress {
            get { return _mac; }
            set {
                _mac = value;
                OnPropertyChanged(nameof(MacAddress));
            }
        }

        #endregion

        #region //TEST RESULT ----------------------------//

        string _change_ip;
        public string changeIP {
            get { return _change_ip; }
            set {
                _change_ip = value;
                OnPropertyChanged(nameof(changeIP));
            }
        }
        string _write_mac;
        public string writeMacLan {
            get { return _write_mac; }
            set {
                _write_mac = value;
                OnPropertyChanged(nameof(writeMacLan));
            }
        }
        string _write_hw_version;
        public string writeHardwareVersion {
            get { return _write_hw_version; }
            set {
                _write_hw_version = value;
                OnPropertyChanged(nameof(writeHardwareVersion));
            }
        }
        string _write_model;
        public string writeModelNumber {
            get { return _write_model; }
            set {
                _write_model = value;
                OnPropertyChanged(nameof(writeModelNumber));
            }
        }
        string _erase_calib_wlan;
        public string EraseCalibWlan {
            get { return _erase_calib_wlan; }
            set {
                _erase_calib_wlan = value;
                OnPropertyChanged(nameof(EraseCalibWlan));
            }
        }
        string _upload_fw;
        public string uploadFirmwareBasic {
            get { return _upload_fw; }
            set {
                _upload_fw = value;
                OnPropertyChanged(nameof(uploadFirmwareBasic));
            }
        }
        string _check_mac_ethernet;
        public string checkMacEthernet {
            get { return _check_mac_ethernet; }
            set {
                _check_mac_ethernet = value;
                OnPropertyChanged(nameof(checkMacEthernet));
            }
        }
        string _check_qca9886;
        public string checkQCA9886 {
            get { return _check_qca9886; }
            set { _check_qca9886 = value; OnPropertyChanged(nameof(checkQCA9886)); }
        }
        string _check_fw_version;
        public string checkFwVersion {
            get { return _check_fw_version; }
            set { _check_fw_version = value; OnPropertyChanged(nameof(checkFwVersion)); }
        }
        string _check_fw_build_time;
        public string checkFwBuildTime {
            get { return _check_fw_build_time; }
            set { _check_fw_build_time = value; OnPropertyChanged(nameof(checkFwBuildTime)); }
        }
        string _check_wan;
        public string checkWanPort {
            get { return _check_wan; }
            set { _check_wan = value; OnPropertyChanged(nameof(checkWanPort)); }
        }
        string _check_led;
        public string checkLED {
            get { return _check_led; }
            set {
                _check_led = value;
                OnPropertyChanged(nameof(checkLED));
            }
        }
        string _led_wan;
        public string ledWan {
            get { return _led_wan; }
            set {
                _led_wan = value;
                OnPropertyChanged(nameof(ledWan));
            }
        }
        
        string _check_button;
        public string checkButton {
            get { return _check_button; }
            set {
                _check_button = value;
                OnPropertyChanged(nameof(checkButton));
            }
        }
        string _button_wps;
        public string buttonWps {
            get { return _button_wps; }
            set {
                _button_wps = value;
                OnPropertyChanged(nameof(buttonWps));
            }
        }
        string _button_reset;
        public string buttonReset {
            get { return _button_reset; }
            set {
                _button_reset = value;
                OnPropertyChanged(nameof(buttonReset));
            }
        }

        string _total_result;
        public string totalResult {
            get { return _total_result; }
            set {
                _total_result = value;
                OnPropertyChanged(nameof(totalResult));
            }
        }

        #endregion

        #region //LOG DATA -------------------------------//

        string _log_system;
        public string logSystem {
            get { return _log_system; }
            set {
                _log_system = value;
                OnPropertyChanged(nameof(logSystem));
            }
        }
        string _log_uart;
        public string logUart {
            get { return _log_uart; }
            set {
                _log_uart = value;
                OnPropertyChanged(nameof(logUart));
            }
        }
        string _error_message;
        public string errorMessage {
            get { return _error_message; }
            set {
                _error_message = value;
                OnPropertyChanged(nameof(errorMessage));
            }
        }
        string _error_code;
        public string errorCode {
            get { return _error_code; }
            set {
                _error_code = value;
                OnPropertyChanged(nameof(errorCode));
            }
        }
        string _legend_wps;
        public string legendWps {
            get { return _legend_wps; }
            set {
                _legend_wps = value;
                OnPropertyChanged(nameof(legendWps));
            }
        }
        string _legend_reset;
        public string legendReset {
            get { return _legend_reset; }
            set {
                _legend_reset = value;
                OnPropertyChanged(nameof(legendReset));
            }
        }

        #endregion

        #region //CẤU HÌNH BÀI TEST ----------------------//

        bool _is_change_ip;
        public bool isChangeIP {
            get { return _is_change_ip; }
            set {
                _is_change_ip = value;
                OnPropertyChanged(nameof(isChangeIP));
            }
        }
        bool _is_write_mac;
        public bool isWriteMacLan {
            get { return _is_write_mac; }
            set {
                _is_write_mac = value;
                OnPropertyChanged(nameof(isWriteMacLan));
            }
        }
        bool _is_write_hw;
        public bool isWriteHW {
            get { return _is_write_hw; }
            set {
                _is_write_hw = value;
                OnPropertyChanged(nameof(isWriteHW));
            }
        }
        bool _is_write_model;
        public bool isWriteModel {
            get { return _is_write_model; }
            set {
                _is_write_model = value;
                OnPropertyChanged(nameof(isWriteModel));
            }
        }
        bool _is_erase_calib_wlan;
        public bool isEraseCalibWlan {
            get { return _is_erase_calib_wlan; }
            set {
                _is_erase_calib_wlan = value;
                OnPropertyChanged(nameof(isEraseCalibWlan));
            }
        }
        bool _is_upload_fw;
        public bool isUploadFW {
            get { return _is_upload_fw; }
            set {
                _is_upload_fw = value;
                OnPropertyChanged(nameof(isUploadFW));
            }
        }
        bool _is_check_mac_ethernet;
        public bool isCheckMacEthernet {
            get { return _is_check_mac_ethernet; }
            set {
                _is_check_mac_ethernet = value;
                OnPropertyChanged(nameof(isCheckMacEthernet));
            }
        }
        bool _is_check_wan;
        public bool isCheckWan {
            get { return _is_check_wan; }
            set {
                _is_check_wan = value;
                OnPropertyChanged(nameof(isCheckWan));
            }
        }
        bool _is_check_led;
        public bool isCheckLED {
            get { return _is_check_led; }
            set {
                _is_check_led = value;
                OnPropertyChanged(nameof(isCheckLED));
            }
        }
        bool _is_check_button;
        public bool isCheckButton {
            get { return _is_check_button; }
            set {
                _is_check_button = value;
                OnPropertyChanged(nameof(isCheckButton));
            }
        }
        bool _is_check_qca9886;
        public bool isCheckQCA9886 {
            get { return _is_check_qca9886; }
            set {
                _is_check_qca9886 = value;
                OnPropertyChanged(nameof(isCheckQCA9886));
            }
        }
        bool _is_check_fw_version;
        public bool isCheckFwVersion {
            get { return _is_check_fw_version; }
            set { _is_check_fw_version = value; OnPropertyChanged(nameof(isCheckFwVersion)); }
        }
        bool _is_check_fw_build_time;
        public bool isCheckFwBuildTime {
            get { return _is_check_fw_build_time; }
            set { _is_check_fw_build_time = value; OnPropertyChanged(nameof(isCheckFwBuildTime)); }
        }

        #endregion
    }
}
