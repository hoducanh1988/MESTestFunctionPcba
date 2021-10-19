using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EW12SG.Function.Global;

namespace EW12SG.Function.Custom {

    public class SettingInformation : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = null) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        //constructor
        public SettingInformation() {

            Factory = "1";
            LineNumber = "1";
            StationName = "-";
            StationNumber = "1";
            WorkOrder = "#12345";
            Operator = "nguyen van a";

            MacHeader = "A06518:A4F4C2:D49AA0";
            HardwareVersion = "EW12SG000H0001";
            ModelNumber = "EW12SG";
            SpeedLan = "1000";
            SpeedWan = "1000";

            FirmwareVersion = "EW12XG000F0001";
            FirmwareBuildTime = "#1 Fri Feb 21 12:02:19 +07 2020";
            FileKernel = "openwrt-ar71xx-generic-ap151-16M-kernel.bin";
            FileRoot = "openwrt-ar71xx-generic-ap151-16M-rootfs-squashfs.bin";
            IpServer = "192.168.88.100";
            IpDut = "";

            SerialPortName = "COM1";

            isEraseCalibWlan = false;
            isUploadFW = true;
            isCheckWan = false;
            isCheckLan = false;
            isCheckLED = false;
            isCheckButton = false;
            isCheckQCA9886 = false;
            isCheckMacEthernet = false;
            isCheckFwVersion = false;
            isCheckFwBuildTime = false;

            timeOutAccessUboot = 30;
            timeOutTransferFileKernel = 20;
            timeOutTransferFileRoot = 120;
            timeOutWaitBootingComplete = 90;
            timeOutCheckLed = 180;
            timeOutCheckButton = 180;
            timeOutPressButtonWps = 30;
            timeOutPressButtonReset = 30;

            waitBootComplete = 5;

            logDirectory = "D:\\LOGDATA";
        }

        //property

        #region //CÀI ĐẶT TRẠM ---------------------------//
        string _ip;
        public string IpDut {
            get { return _ip; }
            set {
                _ip = value;
                OnPropertyChanged(nameof(IpDut));
            }
        }
        string _fac;
        public string Factory {
            get { return _fac; }
            set {
                _fac = value;
                OnPropertyChanged(nameof(Factory));
            }
        }
        string _line;
        public string LineNumber {
            get { return _line; }
            set {
                _line = value;
                OnPropertyChanged(nameof(LineNumber));
            }
        }
        string _station_name;
        public string StationName {
            get { return _station_name; }
            set {
                _station_name = value;
                OnPropertyChanged(nameof(StationName));
                switch (value) {
                    case "TestFunctionPcba": {
                            isUploadFW = false;
                            isCheckWan = true;
                            isCheckLan = true;
                            isCheckLED = true;
                            isCheckButton = true;
                            isCheckQCA9886 = true;
                            isCheckMacEthernet = true;
                            isCheckFwBuildTime = true;
                            isCheckFwVersion = true;
                            myGlobal.myMainWindow.StationName = "Station: Test Function PCBA";
                            break;
                        }
                    case "UploadFirmwareBasic": {
                            isUploadFW = true;
                            myGlobal.myMainWindow.StationName = "Station: Upload Firmware Basic";
                            break;
                        }
                }
            }
        }
        
        string _station_number;
        public string StationNumber {
            get { return _station_number; }
            set {
                _station_number = value;
                IpDut = string.Format("192.168.88.{0}", int.Parse(value) + 1);
                myGlobal.myMainWindow.StationNumber = string.Format("DUT #{0}", int.Parse(value) < 10 ? string.Format("0{0}", value) : value);
                OnPropertyChanged(nameof(StationNumber));
            }
        }
        string _work_order;
        public string WorkOrder {
            get { return _work_order; }
            set {
                _work_order = value;
                OnPropertyChanged(nameof(WorkOrder));
            }
        }
        string _operator;
        public string Operator {
            get { return _operator; }
            set {
                _operator = value;
                OnPropertyChanged(nameof(Operator));
            }
        }

        #endregion

        #region //CÀI ĐẶT THÔNG TIN IMAP ---------------------------//

        string _mac_header;
        public string MacHeader {
            get { return _mac_header; }
            set {
                _mac_header = value;
                OnPropertyChanged(nameof(MacHeader));
            }
        }
        string _hw_version;
        public string HardwareVersion {
            get { return _hw_version; }
            set {
                _hw_version = value;
                OnPropertyChanged(nameof(HardwareVersion));
            }
        }
        string _model;
        public string ModelNumber {
            get { return _model; }
            set {
                _model = value;
                OnPropertyChanged(nameof(ModelNumber));
            }
        }
        string _speed_lan;
        public string SpeedLan {
            get { return _speed_lan; }
            set {
                _speed_lan = value;
                OnPropertyChanged(nameof(SpeedLan));
            }
        }
        string _speed_wan;
        public string SpeedWan {
            get { return _speed_wan; }
            set {
                _speed_wan = value;
                OnPropertyChanged(nameof(SpeedWan));
            }
        }

        #endregion

        #region //CÀI ĐẶT DOWNLOAD FIRMWARE ---------------------------//

        string _firmware_version;
        public string FirmwareVersion {
            get { return _firmware_version; }
            set {
                _firmware_version = value;
                OnPropertyChanged(nameof(FirmwareVersion));
            }
        }
        string _firmware_build_time;
        public string FirmwareBuildTime {
            get { return _firmware_build_time; }
            set {
                _firmware_build_time = value;
                OnPropertyChanged(nameof(FirmwareBuildTime));
            }
        }
        string _file_kernel;
        public string FileKernel {
            get { return _file_kernel; }
            set {
                _file_kernel = value;
                OnPropertyChanged(nameof(FileKernel));
            }
        }
        string _file_root;
        public string FileRoot {
            get { return _file_root; }
            set {
                _file_root = value;
                OnPropertyChanged(nameof(FileRoot));
            }
        }
        string _ip_server;
        public string IpServer {
            get { return _ip_server; }
            set {
                _ip_server = value;
                OnPropertyChanged(nameof(IpServer));
            }
        }

        #endregion

        #region //CÀI ĐẶT USB TO UART ---------------------------//

        string _serial_port_name;
        public string SerialPortName {
            get { return _serial_port_name; }
            set {
                _serial_port_name = value;
                myGlobal.myMainWindow.SerialPortName = value;
                OnPropertyChanged(nameof(SerialPortName));
            }
        }

        #endregion

        #region //CẤU HÌNH BÀI TEST ----------------------//

        bool _is_change_ip;
        public bool isChangeIP {
            get { return _is_change_ip; }
            set {
                _is_change_ip = value;
                myGlobal.myTesting.isChangeIP = value;
                OnPropertyChanged(nameof(isChangeIP));
            }
        }
        bool _is_write_mac;
        public bool isWriteMacLan {
            get { return _is_write_mac; }
            set {
                _is_write_mac = value;
                myGlobal.myTesting.isWriteMacLan = value;
                OnPropertyChanged(nameof(isWriteMacLan));
            }
        }
        bool _is_write_hw;
        public bool isWriteHW {
            get { return _is_write_hw; }
            set {
                _is_write_hw = value;
                myGlobal.myTesting.isWriteHW = value;
                OnPropertyChanged(nameof(isWriteHW));
            }
        }
        bool _is_write_model;
        public bool isWriteModel {
            get { return _is_write_model; }
            set {
                _is_write_model = value;
                myGlobal.myTesting.isWriteModel = value;
                OnPropertyChanged(nameof(isWriteModel));
            }
        }
        bool _is_erase_calib_wlan;
        public bool isEraseCalibWlan {
            get { return _is_erase_calib_wlan; }
            set {
                _is_erase_calib_wlan = value;
                myGlobal.myTesting.isEraseCalibWlan = value;
                OnPropertyChanged(nameof(isEraseCalibWlan));
            }
        }
        bool _is_upload_fw;
        public bool isUploadFW {
            get { return _is_upload_fw; }
            set {
                _is_upload_fw = value;
                OnPropertyChanged(nameof(isUploadFW));
                //
                myGlobal.myTesting.isUploadFW = value;
                this.isChangeIP = value;
                this.isWriteMacLan = value;
                this.isWriteHW = value;
                this.isWriteModel = value;
                //
                this.isCheckLan = !value;
                this.isCheckWan = !value;
                this.isCheckLED = !value;
                this.isCheckButton = !value;
                this.isCheckQCA9886 = !value;
                this.isCheckMacEthernet = !value;
                this.isCheckFwVersion = !value;
                this.isCheckFwBuildTime = !value;
            }
        }
        bool _is_check_mac_ethernet;
        public bool isCheckMacEthernet {
            get { return _is_check_mac_ethernet; }
            set {
                _is_check_mac_ethernet = value;
                OnPropertyChanged(nameof(isCheckMacEthernet));
                myGlobal.myTesting.isCheckMacEthernet = value;
            }
        }
        bool _is_check_qca9886;
        public bool isCheckQCA9886 {
            get { return _is_check_qca9886; }
            set {
                _is_check_qca9886 = value;
                OnPropertyChanged(nameof(isCheckQCA9886));
                myGlobal.myTesting.isCheckQCA9886 = value;
            }
        }
        bool _is_check_fw_version;
        public bool isCheckFwVersion {
            get { return _is_check_fw_version; }
            set { _is_check_fw_version = value; OnPropertyChanged(nameof(isCheckFwVersion));
                myGlobal.myTesting.isCheckFwVersion = value;
            }
        }
        bool _is_check_fw_build_time;
        public bool isCheckFwBuildTime {
            get { return _is_check_fw_build_time; }
            set {
                _is_check_fw_build_time = value; OnPropertyChanged(nameof(isCheckFwBuildTime));
                myGlobal.myTesting.isCheckFwBuildTime = value;
            }
        }
        bool _is_check_lan;
        public bool isCheckLan {
            get { return _is_check_lan; }
            set {
                _is_check_lan = value;
                OnPropertyChanged(nameof(isCheckLan));
                myGlobal.myTesting.isCheckLan = value;
            }
        }
        bool _is_check_wan;
        public bool isCheckWan {
            get { return _is_check_wan; }
            set {
                _is_check_wan = value;
                OnPropertyChanged(nameof(isCheckWan));
                myGlobal.myTesting.isCheckWan = value;
            }
        }
        bool _is_check_led;
        public bool isCheckLED {
            get { return _is_check_led; }
            set {
                _is_check_led = value;
                OnPropertyChanged(nameof(isCheckLED));
                myGlobal.myTesting.isCheckLED = value;
            }
        }
        bool _is_check_button;
        public bool isCheckButton {
            get { return _is_check_button; }
            set {

                _is_check_button = value;
                OnPropertyChanged(nameof(isCheckButton));
                myGlobal.myTesting.isCheckButton = value;
            }
        }

        #endregion

        #region //CÀI ĐẶT GIÁ TRỊ TIMEOUT ------------------------//

        int _timeout_access_uboot;
        public int timeOutAccessUboot {
            get { return _timeout_access_uboot; }
            set {
                _timeout_access_uboot = value;
                OnPropertyChanged(nameof(timeOutAccessUboot));
            }
        }
        int _timeout_transfer_file_kernel;
        public int timeOutTransferFileKernel {
            get { return _timeout_transfer_file_kernel; }
            set {
                _timeout_transfer_file_kernel = value;
                OnPropertyChanged(nameof(timeOutTransferFileKernel));
            }
        }
        int _timeout_transfer_file_root;
        public int timeOutTransferFileRoot {
            get { return _timeout_transfer_file_root; }
            set {
                _timeout_transfer_file_root = value;
                OnPropertyChanged(nameof(timeOutTransferFileRoot));
            }
        }
        int _timeout_wait_boot;
        public int timeOutWaitBootingComplete {
            get { return _timeout_wait_boot; }
            set {
                _timeout_wait_boot = value;
                OnPropertyChanged(nameof(timeOutWaitBootingComplete));
            }
        }
        int _timeout_check_led;
        public int timeOutCheckLed {
            get { return _timeout_check_led; }
            set {
                _timeout_check_led = value;
                OnPropertyChanged(nameof(timeOutCheckLed));
            }
        }
        int _timeout_check_button;
        public int timeOutCheckButton {
            get { return _timeout_check_button; }
            set {
                _timeout_check_button = value;
                OnPropertyChanged(nameof(timeOutCheckButton));
            }
        }
        int _timeout_press_button_wps;
        public int timeOutPressButtonWps {
            get { return _timeout_press_button_wps; }
            set {
                _timeout_press_button_wps = value;
                OnPropertyChanged(nameof(timeOutPressButtonWps));
            }
        }
        int _timeout_press_button_reset;
        public int timeOutPressButtonReset {
            get { return _timeout_press_button_reset; }
            set {
                _timeout_press_button_reset = value;
                OnPropertyChanged(nameof(timeOutPressButtonReset));
            }
        }

        #endregion

        #region //CÀI ĐẶT DELAY --------------------------//

        int _wait_boot_complete;
        public int waitBootComplete {
            get { return _wait_boot_complete; }
            set {
                _wait_boot_complete = value;
                OnPropertyChanged(nameof(waitBootComplete));
            }
        }


        #endregion

        #region //CÀI ĐẶT LƯU LOG -----------------------------//

        string _log_directory;
        public string logDirectory {
            get { return _log_directory; }
            set {
                _log_directory = value;
                OnPropertyChanged(nameof(logDirectory));
            }
        }

        #endregion

    }
}
