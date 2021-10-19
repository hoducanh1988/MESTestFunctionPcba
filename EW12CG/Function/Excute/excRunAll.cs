using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using EW12CG.Function.DUT;
using EW12CG.Function.Global;
using EW12CG.Function.Custom;
using UtilityPack.Validation;
using System.Diagnostics;
using EW12CG.Function.Protocol;
using System.Windows.Controls;
using EW12CG.UserCtrl;

namespace EW12CG.Function.Excute {

    public partial class excRunAll {
        meshAP<TestingInformation, SettingInformation> mesh = null;
        string macAddress = "";
        TestingInformation myTesting = myGlobal.myTesting;
        SettingInformation mySetting = myGlobal.mySetting;
        Grid testGrid = null;

        public excRunAll(string mac, Grid grid) {
            mesh = new meshAP<TestingInformation, SettingInformation>(myTesting, mySetting, "logUart", "SerialPortName");
            this.macAddress = mac;
            this.testGrid = grid;
        }

        public bool Excuting() {
            Stopwatch st = new Stopwatch();
            st.Start();

            bool r = false;
            myTesting.waitParams();
            myTesting.MacAddress = string.Format("\"{0}\"", this.macAddress);
            myGlobal.myLogTotal = new IO.VnptAsmTestFunctionLogInfo();
            var myLogTotal = myGlobal.myLogTotal;
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            //1. validate mac address
            r = _validateMac();
            if (!r) goto END;

            //2. open serial port
            r = _openSerialPort();
            if (!r) goto END;

            if (myTesting.isUploadFW) {
                //3. access to uboot
                r = _accessToUboot();
                if (!r) goto END;

                //4. change ip uboot
                myGlobal.myTesting.changeIP = "Waiting...";
                //
                r = _changeIpUboot(mySetting.IpServer, mySetting.IpDut);
                myLogTotal.ChangeIP.Result = r ? "Passed" : "Failed";
                myGlobal.myTesting.changeIP = r ? "Passed" : "Failed";
                if (!r) goto END;

                //7. write mac ethernet
                r = _writeMacEthernet();
                myLogTotal.WriteMac.Result = r ? "Passed" : "Failed";
                if (!r) goto END;

                //8. write hardware version
                r = _writeHardwareVersion();
                myLogTotal.WriteHardware.Result = r ? "Passed" : "Failed";
                if (!r) goto END;

                //9. write model number
                r = _writeModelNumber();
                myLogTotal.WriteModel.Result = r ? "Passed" : "Failed";
                if (!r) goto END;

                //10. upload firmware => need check file firmware, tftpd64, ip pc, firewall
                r = _uploadFirmwareBasic();
                myLogTotal.UploadFirmware.Result = r ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //11. wait dut boot complete
            if (myTesting.isCheckWan || myTesting.isCheckLED || myTesting.isCheckButton) {
                r = _waitBootComplete();
                if (!r) goto END;
            }

            //12. check mac ethernet
            if (myTesting.isCheckMacEthernet) {
                r = _checkMacEthernet();
                myLogTotal.CheckMacEthernet.Result = r ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //13. check connection to QCA9886
            if (myTesting.isCheckQCA9886) {
                r = _checkQCA9886();
                myLogTotal.CheckQCA9886.Result = r ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //14. check firmware version
            if (myTesting.isCheckFwVersion) {
                r = _checkFirmwareBasicVersion();
                myLogTotal.CheckFirmwareVersion.Result = r ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //15. check firmware build time
            if (myTesting.isCheckFwBuildTime) {
                r = _checkFirmwareBasicBuildTime();
                myLogTotal.CheckFirmwareBuildTime.Result = r ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //16. check wan
            if (myTesting.isCheckWan) {
                r = _checkWanPort();
                myLogTotal.CheckWan.Result = r ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //17. check leds
            if (myTesting.isCheckLED) {
                r = _checkLEDs(this.testGrid);
                myLogTotal.CheckLed.Result = r ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //18. check buttons
            if (myTesting.isCheckButton) {
                r = _checkButtons(this.testGrid);
                myLogTotal.CheckButton.Result = r ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            END:
            bool ___ = r ? myTesting.passParams() : myTesting.failParams();
            st.Stop();
            myTesting.logSystem += string.Format("\r\n### Finished ###\r\n");
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += string.Format("... Total result : {0}\r\n", r ? "Passed" : "Failed");
            myTesting.logSystem += string.Format("... Total time : {0} ms\r\n", st.ElapsedMilliseconds);
            mesh.Close();
            return r;
        }

        #region all unit functions

        private bool _validateMac() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myTesting.logSystem += string.Format("\r\n### Validating Inputed Mac Address ### \r\n");
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += string.Format("... standard value : {0}\r\n", mySetting.MacHeader);
            myTesting.logSystem += string.Format("... actual value : {0}\r\n", this.macAddress);

            //check format
            bool r = Parse.IsMacAddress(this.macAddress);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.logSystem += string.Format("... error : wrong format\r\n");
                myTesting.errorMessage = "Error validate mac ethernet => wrong format.";
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                return false;
            }

            //check header
            r = mySetting.MacHeader.ToLower().Contains(this.macAddress.Substring(0, 6).ToLower());
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            if (!r) {
                myTesting.logSystem += string.Format("... error : wrong header\r\n");
                myTesting.errorMessage = "Error validate mac ethernet => wrong header.";
            }
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            return r;
        }

        private bool _openSerialPort() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myTesting.logSystem += string.Format("\r\n### Open Serial Port {0} ### \r\n", myGlobal.mySetting.SerialPortName);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += string.Format("... standard value : true\r\n");
            bool r = mesh.Open();
            myTesting.logSystem += string.Format("... actual value : {0}\r\n", r);
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            if (!r) {
                myTesting.logSystem += string.Format("... error : serial port is closed or using by another application.\r\n");
                myTesting.errorMessage = "Error open imap serial port => port is closed or using by another application.";
            }
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            return r;
        }

        private bool _accessToUboot() {
            Stopwatch st = new Stopwatch();
            st.Start();

            bool r = false;
            int timeout = mySetting.timeOutAccessUboot;
            int retry = 3;

            myTesting.logSystem += string.Format("\r\n### Access To Uboot (timeout {0} sec) ### \r\n", timeout);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            r = _waitHitAnyKey(timeout);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.errorMessage = "Error access to uboot => timeout waiting text \"Hit any key to stop autoboot\".";
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                return false;
            }

            int count = 0;
        RE:
            count++;
            r = mesh.Query("\n", "ath>", 3);
            if (!r) {
                if (count < retry) {
                    goto RE;
                }
            }

            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            if (!r) {
                myTesting.logSystem += string.Format("... error : request timeout\r\n");
                myTesting.errorMessage = "Error access to uboot => can't send command to imap.";
            }
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            return r;
        }

        private bool _changeIpUboot(string ipserver, string ipdut) {
            Stopwatch st = new Stopwatch();
            st.Start();

            bool r = false;
            int retry = 3;
            int error_code = 0;

            myTesting.logSystem += string.Format("\r\n### Change IP Uboot (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += string.Format("... new ip address: server={0}, dut={1}\r\n", ipserver, ipdut);

            string data = "";
            r = mesh.Query(string.Format("setenv serverip {0}", ipserver), "ath>", 3, 3, out data);
            if (!r) { error_code = 1; goto END; }
            r = mesh.Query(string.Format("setenv ipaddr {0}", ipdut), "ath>", 3, 3, out data);
            if (!r) { error_code = 3; goto END; }
            r = mesh.Query("saveenv", "ath>", 3, 3, out data);
            if (!r) { error_code = 2; goto END; }

        END:
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            if (!r) {
                string err_str = "";
                switch (error_code) {
                    case 1: { err_str = "can't set server ip to " + ipserver; break; }
                    case 2: { err_str = "can't save env."; break; }
                    case 3: { err_str = "can't set dut ip to " + ipdut; break; }
                    case 4: { err_str = "can't reboot dut."; break; }
                }
                myTesting.logSystem += string.Format("... error : {0}\r\n", err_str);
                myTesting.errorMessage = string.Format("Error change ip uboot => {0}", err_str);
            }
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            return r;
        }

        private bool _validateIpUboot(string ipserver, string ipdut) {
            Stopwatch st = new Stopwatch();
            st.Start();

            bool r = false;
            int retry = 3;
            int error_code = 0;

            myTesting.logSystem += string.Format("\r\n### Validate IP Uboot (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += string.Format("... new ip address: server={0}, dut={1}\r\n", ipserver, ipdut);
            //
            string data = "";
            r = mesh.Query("printenv",ipserver, 3, 3, out data);
            if (!r) { error_code = 1; goto END; }
            r = mesh.Query("printenv", ipdut, 3, 3, out data);
            if (!r) { error_code = 2; goto END; }
            //
            END:
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            if (!r) {
                string err_str = "";
                switch (error_code) {
                    case 1: { err_str = "server ip not same setting " + ipserver; break; }
                    case 2: { err_str = "dut ip not same setting " + ipdut; break; }
                }
                myTesting.logSystem += string.Format("... error : {0}\r\n", err_str);
                myTesting.errorMessage = string.Format("Error validate ip uboot => {0}", err_str);
            }
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            return r;
        }

        private bool _captureLog(string text) {
            return myTesting.logUart.ToLower().Contains(text.ToLower());
        }

        private bool _waitHitAnyKey(int timeout) {
            int count = 0;
            bool r = false;
            int delay = 100;
            int rate = 1000 / delay;
            myTesting.logSystem += string.Format("... waiting text \"Hit any key to stop autoboot\"\r\n...");
            int x = 0;
        REP:
            count++;
            r = _captureLog("Hit any key to stop autoboot:");
            if (x != count / rate) {
                x = count / rate;
                myTesting.logSystem += string.Format("{0}..", x);
            }
            if (!r) {
                if (count < timeout * rate) {
                    Thread.Sleep(delay);
                    goto REP;
                }
            }
            myTesting.logSystem += string.Format("\r\n");
            return r;
        }

        private bool _writeMacEthernet() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.writeMacLan = "Waiting...";
            bool r = false;
            int retry = 3;
            myTesting.logSystem += string.Format("\r\n### Write Mac Ethernet (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            string tmp = string.Format("{0}:{1}:{2}:{3}:{4}:{5}",
                this.macAddress.Substring(0, 2),
                this.macAddress.Substring(2, 2),
                this.macAddress.Substring(4, 2),
                this.macAddress.Substring(6, 2),
                this.macAddress.Substring(8, 2),
                this.macAddress.Substring(10, 2)
                );

            //write mac
            string cmd = string.Format("setenv ethaddr {0}", tmp);
            myTesting.logSystem += string.Format("... write mac : {0}\r\n", cmd);
            r = mesh.Query(cmd, "ath>", 3);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.errorMessage = "Error write mac ethernet => can't send command " + cmd;
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.writeMacLan = "Failed";
                return false;
            }

            //save 
            cmd = "saveenv";
            myTesting.logSystem += string.Format("... save mac : {0}\r\n", cmd);
            r = mesh.Query(cmd, "ath>", 3);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.errorMessage = "Error write mac ethernet => can't send command " + cmd;
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.writeMacLan = "Failed";
                return false;
            }

            //verify
            cmd = "printenv";
            myTesting.logSystem += string.Format("... verify mac : {0}\r\n", cmd);
            r = mesh.Query(cmd, string.Format("ethaddr={0}", tmp), 3);
            if (!r) myTesting.errorMessage = "Error write mac ethernet => verify mac not same inputed";
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.writeMacLan = r ? "Passed" : "Failed";
            return r;
        }

        private bool _writeHardwareVersion() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.writeHardwareVersion = "Waiting...";
            bool r = false;
            int retry = 3;
            myTesting.logSystem += string.Format("\r\n### Write Hardware Version (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            string tmp = mySetting.HardwareVersion;

            //check hardware version format
            myTesting.logSystem += string.Format("... check format : {0}\r\n", tmp);
            r = string.IsNullOrEmpty(tmp) || string.IsNullOrWhiteSpace(tmp);
            if (r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.errorMessage = "Error write hardware version => wrong format";
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.writeHardwareVersion = "Failed";
                return false;
            }

            //write hardware version
            string cmd = string.Format("setenv hardwareversion {0}", tmp);
            myTesting.logSystem += string.Format("... write hw : {0}\r\n", cmd);
            r = mesh.Query(cmd, "ath>", 3);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.errorMessage = "Error write hardware version => can't send command " + cmd;
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.writeHardwareVersion = "Failed";
                return false;
            }

            //save 
            cmd = "saveenv";
            myTesting.logSystem += string.Format("... save hw : {0}\r\n", cmd);
            r = mesh.Query(cmd, "ath>", 3);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.errorMessage = "Error write hardware version => can't send command " + cmd;
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.writeHardwareVersion = "Failed";
                return false;
            }

            //verify
            cmd = "printenv";
            myTesting.logSystem += string.Format("... verify hw : {0}\r\n", cmd);
            r = mesh.Query(cmd, string.Format("hardwareversion={0}", tmp), 3);
            if (!r) myTesting.errorMessage = "Error write hardware version => verify hardware version not same setting";
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.writeHardwareVersion = r ? "Passed" : "Failed";
            return r;
        }

        private bool _writeModelNumber() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.writeModelNumber = "Waiting...";
            bool r = false;
            int retry = 3;
            myTesting.logSystem += string.Format("\r\n### Write Model Number (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            string tmp = mySetting.ModelNumber;

            //check model number format
            myTesting.logSystem += string.Format("... check format : {0}\r\n", tmp);
            r = string.IsNullOrEmpty(tmp) || string.IsNullOrWhiteSpace(tmp);
            if (r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.errorMessage = "Error write model number => wrong format";
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.writeModelNumber = "Failed";
                return false;
            }

            //write model number
            string cmd = string.Format("setenv modelnumber {0}", tmp);
            myTesting.logSystem += string.Format("... write model : {0}\r\n", cmd);
            r = mesh.Query(cmd, "ath>", 3);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.errorMessage = "Error write model number => can't send command " + cmd;
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.writeModelNumber = "Failed";
                return false;
            }

            //save 
            cmd = "saveenv";
            myTesting.logSystem += string.Format("... save model : {0}\r\n", cmd);
            r = mesh.Query(cmd, "ath>", 3);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.errorMessage = "Error write model number => can't send command " + cmd;
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.writeModelNumber = "Failed";
                return false;
            }

            //verify
            cmd = "printenv";
            myTesting.logSystem += string.Format("... verify model : {0}\r\n", cmd);
            r = mesh.Query(cmd, string.Format("modelnumber={0}", tmp), 3);
            if (!r) myTesting.errorMessage = "Error write model number => verify model number not same setting";
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.writeModelNumber = r ? "Passed" : "Failed";
            return r;
        }


        private bool _uploadFirmwareBasic() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.uploadFirmwareBasic = "Waiting...";
            bool r = false;
            int timeout = mySetting.timeOutTransferFileRoot;
            myTesting.logSystem += string.Format("\r\n### Upload Firmware Basic (timeout {0} sec) ### \r\n", timeout);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");

            //check file kernel
            r = _checkFileKernel();
            st.Stop();
            myTesting.logSystem += string.Format("elapsed time : {0} ms\r\n\r\n", st.ElapsedMilliseconds);
            if (!r) goto END;

            //check file root
            st.Reset(); st.Restart();
            r = _checkFileRoot();
            st.Stop();
            myTesting.logSystem += string.Format("elapsed time : {0} ms\r\n\r\n", st.ElapsedMilliseconds);
            if (!r) goto END;

            //check tftpd64, tftpd32
            st.Reset(); st.Restart();
            r = _checkAppTftp();
            st.Stop();
            myTesting.logSystem += string.Format("elapsed time : {0} ms\r\n\r\n", st.ElapsedMilliseconds);
            if (!r) goto END;

            //check firewall
            st.Reset(); st.Restart();
            r = _checkFireWall();
            st.Stop();
            myTesting.logSystem += string.Format("elapsed time : {0} ms\r\n\r\n", st.ElapsedMilliseconds);
            if (!r) goto END;

            //erase last calib wlan
            if (myTesting.isEraseCalibWlan) {
                st.Reset(); st.Restart();
                r = _eraseCalibWlan();
                st.Stop();
                myTesting.logSystem += string.Format("elapsed time : {0} ms\r\n\r\n", st.ElapsedMilliseconds);
                myGlobal.myLogTotal.EraseCalibWlan.Result = r ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //transfer kernel file to imap
            st.Reset(); st.Restart();
            r = _transferFileKernel();
            st.Stop();
            myTesting.logSystem += string.Format("elapsed time : {0} ms\r\n\r\n", st.ElapsedMilliseconds);
            if (!r) goto END;

            //transfer root file to imap
            st.Reset(); st.Restart();
            r = _transferFileRoot();
            st.Stop();
            myTesting.logSystem += string.Format("elapsed time : {0} ms\r\n\r\n", st.ElapsedMilliseconds);
            if (!r) goto END;

            END:
            myTesting.logSystem += string.Format("\r\nresult : {0}\r\n", r ? "Passed" : "Failed");
            myGlobal.myTesting.uploadFirmwareBasic = r ? "Passed" : "Failed";
            return r;
        }

        private bool _waitBootComplete() {
            Stopwatch st = new Stopwatch();
            st.Start();

            bool r = false;
            int timeout = mySetting.timeOutWaitBootingComplete;
            string g_logstart = "fast-classifier: starting up";
            myTesting.logSystem += string.Format("\r\n### Wait Booting Completed (timeout {0} sec) ### \r\n", timeout);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += "... waitting.";

            int count = 0, d = 0, len = 0, rep_len = 0;
            int wait_no_response = mySetting.waitBootComplete;
            int time_boot_complete = timeout;
            REP:
            count++;
            myTesting.logSystem += string.Format("..{0}", count);

            if (myGlobal.myTesting.logUart.Replace("\r", "").Replace("\n", "").Trim().Length == 0) {
                d++;
                if (d < wait_no_response) {
                    Thread.Sleep(1000);
                    goto REP;
                } else {
                    mesh.WriteLine(""); Thread.Sleep(200);
                    if (myGlobal.myTesting.logUart.Contains("root@VNPT:/#")) { r = true; }
                }

                if (count < timeout && r == false) {
                    d = 0;
                    Thread.Sleep(1000);
                    goto REP;
                }
            } else {
                d = 0;
                if (myGlobal.myTesting.logUart.Contains(g_logstart) == false) {
                    if (count < timeout) {
                        if (len == myGlobal.myTesting.logUart.Length) {
                            rep_len++;
                            if (rep_len >= wait_no_response) {
                                string[] buffer = myGlobal.myTesting.logUart.Split('\n');
                                int max = buffer.Length - 1;
                                string data = "";
                                double db = double.MinValue;
                                bool zzz = false;

                                for (int i = max; i >= 0; i--) {
                                    if (string.IsNullOrEmpty(buffer[i]) == false && string.IsNullOrWhiteSpace(buffer[i]) == false) {
                                        if (buffer[i].Contains("[") && buffer[i].Contains("]")) {
                                            data = buffer[i];
                                            data = data.Split(']')[0].Split('[')[1].Trim();
                                            zzz = double.TryParse(data, out db);
                                            if (zzz) break;
                                        }
                                    }
                                }

                                if (zzz) {
                                    if (db >= time_boot_complete) {
                                        mesh.WriteLine(""); Thread.Sleep(200);
                                        if (myGlobal.myTesting.logUart.Contains("root@VNPT:/#")) { r = true; }
                                    } else {
                                        rep_len = 0;
                                        Thread.Sleep(1000);
                                        goto REP;
                                    }
                                } else {
                                    rep_len = 0;
                                    Thread.Sleep(1000);
                                    goto REP;
                                }
                            } else {
                                Thread.Sleep(1000);
                                goto REP;
                            }
                        } else {
                            len = myGlobal.myTesting.logUart.Length;
                            rep_len = 0;
                            Thread.Sleep(1000);
                            goto REP;
                        }
                    }
                } else r = true;
            }
            myTesting.logSystem += "\r\n";
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            if (!r) {
                myTesting.logSystem += string.Format("... error : request timeout\r\n");
                myTesting.errorMessage = "Error wait imap booting complete => request timeout.";
            }
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            return r;
        }

        private bool _checkMacEthernet() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.checkMacEthernet = "Waiting...";
            bool r = false;
            int retry = 20;
            myTesting.logSystem += string.Format("\r\n### Check mac ethernet (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += "... waitting.";
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

            //check mac ethernet
            string data_ref = myTesting.MacAddress.Replace("\"", "").Trim();
            string cmd = @"cat /sys/class/net/eth0/address";
            string s = "";
            mesh.Query("\n", "root@VNPT:/#", 3000, out s);
            string data = mesh.Query(cmd, 250);
            data = data.Replace(cmd, "").Replace("root@VNPT:/#", "").Replace("\r", "").Replace("\n", "").Replace(":","").Trim().ToUpper();
            myTesting.logSystem += string.Format("...... actual : {0}\r\n", data);
            myTesting.logSystem += string.Format("...... setting : {0}\r\n", data_ref);
            r = data.Equals(data_ref);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.logSystem += string.Format("... error : actual value {0} not same standard {1}\r\n", data, data_ref);
                myTesting.errorMessage = string.Format("Error check mac ethernet => actual value {0} not same standard {1}", data, data_ref);
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.checkMacEthernet = "Failed";
                return false;
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.checkMacEthernet = r ? "Passed" : "Failed";
            return r;
        }

        private bool _checkFirmwareBasicVersion() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.checkFwVersion = "Waiting...";
            bool r = false;
            int retry = 20;
            myTesting.logSystem += string.Format("\r\n### Check firmware basic version (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += "... waitting.";
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

            //check firmware basic version
            string data_ref = mySetting.FirmwareVersion.Trim();
            string cmd = @"cat /etc/fw_info";
            string s = "";
            mesh.Query("\n", "root@VNPT:/#", 3000, out s);
            string data = mesh.Query(cmd, 250);
            data = data.Replace(cmd, "").Replace("root@VNPT:/#", "").Replace("Firmware Version:", "").Replace("\r", "").Replace("\n", "").Trim().ToUpper();
            myTesting.logSystem += string.Format("...... actual : {0}\r\n", data);
            myTesting.logSystem += string.Format("...... setting : {0}\r\n", data_ref);
            r = data.Equals(data_ref);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.logSystem += string.Format("... error : actual value {0} not same standard {1}\r\n", data, data_ref);
                myTesting.errorMessage = string.Format("Error check firmware basic version => actual value {0} not same standard {1}", data, data_ref);
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.checkFwVersion = "Failed";
                return false;
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.checkFwVersion = r ? "Passed" : "Failed";
            return r;
        }

        private bool _checkFirmwareBasicBuildTime() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.checkFwBuildTime = "Waiting...";
            bool r = false;
            int retry = 20;
            myTesting.logSystem += string.Format("\r\n### Check firmware basic build time (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += "... waitting.";
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

            //check firmware basic build time
            string data_ref = mySetting.FirmwareBuildTime.Trim().ToLower();
            string cmd = @"cat /proc/version";
            string s = "";
            mesh.Query("\n", "root@VNPT:/#", 3000, out s);
            string data = mesh.Query(cmd, 250);
            data = data.Replace(cmd, "").Replace("root@VNPT:/#", "").Replace("\r", "").Replace("\n", "").Trim().ToUpper();
            data = data.Split(new string[] { ") )" }, StringSplitOptions.None)[1].Trim().ToLower();
            myTesting.logSystem += string.Format("...... actual : {0}\r\n", data);
            myTesting.logSystem += string.Format("...... setting : {0}\r\n", data_ref);
            r = data.Equals(data_ref);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.logSystem += string.Format("... error : actual value {0} not same standard {1}\r\n", data, data_ref);
                myTesting.errorMessage = string.Format("Error check firmware basic build time => actual value {0} not same standard {1}", data, data_ref);
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.checkFwBuildTime = "Failed";
                return false;
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.checkFwBuildTime = r ? "Passed" : "Failed";
            return r;
        }

        private bool _checkQCA9886() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.checkQCA9886 = "Waiting...";
            bool r = false;
            int retry = 20;
            myTesting.logSystem += string.Format("\r\n### Check connection to QCA9886 (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += "... waitting.";
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

            //check connection to QCA9886
            string data_ref = "0000:00:00.0";
            string cmd = @"ls /sys/devices/pci0000\:00/ | grep 0000";
            string s = "";
            mesh.Query("\n", "root@VNPT:/#", 3000, out s);
            string data = mesh.Query(cmd, 250);
            data = data.Replace(cmd, "").Replace("root@VNPT:/#", "").Replace("\r", "").Replace("\n", "").Trim();
            myTesting.logSystem += string.Format("...... actual : {0}\r\n", data);
            myTesting.logSystem += string.Format("...... setting : {0}\r\n", data_ref);
            r = data.Equals(data_ref);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.logSystem += string.Format("... error : actual value {0} not same standard {1}\r\n", data, data_ref);
                myTesting.errorMessage = string.Format("Error check connection to QCA9886 => actual value {0} not same standard {1}", data, data_ref);
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.checkQCA9886 = "Failed";
                return false;
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.checkQCA9886 = r ? "Passed" : "Failed";
            return r;
        }

        private bool _checkWanPort() {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.checkWanPort = "Waiting...";
            bool r = false;
            int retry = 20;
            myTesting.logSystem += string.Format("\r\n### Check WAN Port (retry {0}) ### \r\n", retry);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += "... waitting.";
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            //check speed
            myTesting.logSystem += string.Format("... check speed\r\n");
            string cmd = "swconfig dev switch0 show | grep \"link: port:2\"";
            string speed = "";
            mesh.Query(cmd, "root@VNPT:/#", 3000, out speed);
            
            speed = speed.Replace(cmd, "").Replace("root@VNPT:/#", "").Replace("\r", "").Replace("\n", "").Trim().ToLower();
            if (speed.Contains("speed:") && speed.Contains("baset")) {
                speed = speed.Split(new string[] { "speed:" }, StringSplitOptions.None)[1]
                             .Split(new string[] { "baset" }, StringSplitOptions.None)[0]
                             .Trim();
            }
            
            myTesting.logSystem += string.Format("...... actual : {0} MB\r\n", speed);
            myTesting.logSystem += string.Format("...... setting : {0} MB\r\n", mySetting.SpeedEthernet);
            r = speed.Equals(mySetting.SpeedEthernet);
            if (!r) {
                myTesting.logSystem += string.Format("... result : Failed\r\n");
                myTesting.logSystem += string.Format("... error : actual speed {0} MB not same setting {1} MB\r\n", speed, mySetting.SpeedEthernet);
                myTesting.errorMessage = string.Format("Error check wan port => actual speed {0} MB not same setting {1} MB", speed, mySetting.SpeedEthernet);
                st.Stop();
                myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
                myGlobal.myTesting.checkWanPort = "Failed";
                return false;
            }
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            myTesting.logSystem += string.Format("... result : {0}\r\n", r ? "Passed" : "Failed");
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.checkWanPort = r ? "Passed" : "Failed";
            return r;
        }

        private bool _checkLEDs(Grid grid) {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.checkLED = "Waiting...";
            bool r = false;
            int timeout = mySetting.timeOutCheckLed;
            int count = 0;
            myTesting.logSystem += string.Format("\r\n### Check LEDs (timeout {0}) ### \r\n", timeout);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += "... waitting.";
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            App.Current.Dispatcher.Invoke(new Action(() => {
                grid.Children.Clear();
                grid.Children.Add(new ucLED(this.mesh));
            }));
            //~~~~~~~~~~~~~~~//
            int x = 0;
            RE:
            count++;
            if (x != count / 4) {
                x = count / 4;
                myTesting.logSystem += string.Format("..{0}", x);
            }
            r = myTesting.ledWan == "Passed" || myTesting.ledWan == "Failed";
            if (!r) {
                if (count < timeout * 4) {
                    Thread.Sleep(250);
                    goto RE;
                }
            }
            //~~~~~~~~~~~~~~~//
            App.Current.Dispatcher.Invoke(new Action(() => {
                grid.Children.Clear();
                grid.Children.Add(new ucItemTest());
            }));
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            r = myTesting.ledWan == "Passed";
            myTesting.logSystem += string.Format("\r\n");
            myTesting.logSystem += string.Format("... ledwan = {0}\r\n", myTesting.ledWan);
            myTesting.logSystem += string.Format("... total led result : {0}\r\n", r ? "Passed" : "Failed");
            if (!r) myTesting.errorMessage = string.Format("Error check led => ledwan={0}", myTesting.ledWan);
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.checkLED = r ? "Passed" : "Failed";
            return r;
        }

        private bool _checkButtons(Grid grid) {
            Stopwatch st = new Stopwatch();
            st.Start();

            myGlobal.myTesting.checkButton = "Waiting...";
            bool r = false;
            int timeout = mySetting.timeOutCheckButton;
            int count = 0;
            myTesting.logSystem += string.Format("\r\n### Check Button (timeout {0}) ### \r\n", timeout);
            myTesting.logSystem += string.Format("-------------------------------------------------------- \r\n");
            myTesting.logSystem += "... waitting.";
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            App.Current.Dispatcher.Invoke(new Action(() => {
                grid.Children.Clear();
                grid.Children.Add(new ucButton());
            }));
            //~~~~~~~~~~~~~~~//
            int x = 0;
            RE:
            count++;
            if (x != count / 4) {
                x = count / 4;
                myTesting.logSystem += string.Format("..{0}", x);
            }
            bool r1 = myTesting.buttonWps == "Passed" || myTesting.buttonWps == "Failed";
            bool r2 = myTesting.buttonReset == "Passed" || myTesting.buttonReset == "Failed";
            r = r1 && r2;
            if (!r) {
                if (count < timeout * 4) {
                    Thread.Sleep(250);
                    goto RE;
                }
            }
            //~~~~~~~~~~~~~~~//
            App.Current.Dispatcher.Invoke(new Action(() => {
                grid.Children.Clear();
                grid.Children.Add(new ucItemTest());
            }));
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            r = myTesting.buttonWps == "Passed" && myTesting.buttonReset == "Passed";
            myTesting.logSystem += string.Format("\r\n");
            myTesting.logSystem += string.Format("... button wps = {0}\r\n", myTesting.buttonWps);
            myTesting.logSystem += string.Format("... button reset = {0}\r\n", myTesting.buttonReset);
            myTesting.logSystem += string.Format("... total button result : {0}\r\n", r ? "Passed" : "Failed");
            if (!r) myTesting.errorMessage = string.Format("Error check button => button wps={0}, button reset={1}", myTesting.buttonWps, myTesting.buttonReset);
            st.Stop();
            myTesting.logSystem += string.Format("... elapsed time : {0} ms\r\n", st.ElapsedMilliseconds);
            myGlobal.myTesting.checkButton = r ? "Passed" : "Failed";
            return r;
        }

        #endregion

    }
}
