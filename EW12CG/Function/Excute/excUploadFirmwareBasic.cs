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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool _checkFileKernel() {
            bool r = false;
            myTesting.logSystem += string.Format("check file kernel: {0} ...................\r\n", mySetting.FileKernel);
            myTesting.logSystem += string.Format("...... standard: existed\r\n");
            r = File.Exists(string.Format("C:\\TFTP-Root\\{0}", mySetting.FileKernel));

            myTesting.logSystem += string.Format("...... actual : {0}\r\n", r ? "existed" : "not existed");
            myTesting.logSystem += string.Format("...... result : {0}\r\n", r ? "Passed" : "Failed");
            if (!r) {
                myTesting.logSystem += string.Format("...... error : file kernel is not exist in directory C:\\TFTP-Root\r\n");
                myTesting.errorMessage = "Error upload firmware basic => file kernel is not exist.";
            }
            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool _checkFileRoot() {
            bool r = false;
            myTesting.logSystem += string.Format("check file root: {0} ...................\r\n", mySetting.FileRoot);
            myTesting.logSystem += string.Format("...... standard: existed\r\n");
            r = File.Exists(string.Format("C:\\TFTP-Root\\{0}", mySetting.FileRoot));

            myTesting.logSystem += string.Format("...... actual : {0}\r\n", r ? "existed" : "not existed");
            myTesting.logSystem += string.Format("...... result : {0}\r\n", r ? "Passed" : "Failed");

            if (!r) {
                myTesting.logSystem += string.Format("...... error : file root is not exist in directory C:\\TFTP-Root\r\n");
                myTesting.errorMessage = "Error upload firmware basic => file root is not exist.";
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool _checkAppTftp() {
            bool r = false;
            myTesting.logSystem += string.Format("check app tftpd32/tftpd64 ...................\r\n");
            myTesting.logSystem += string.Format("...... standard: running\r\n");
            r = UtilityPack.IO.WindowProcess.isProcessRunning("tftpd64") || UtilityPack.IO.WindowProcess.isProcessRunning("tftpd32");

            myTesting.logSystem += string.Format("...... actual : {0}\r\n", r ? "running" : "stoped");
            myTesting.logSystem += string.Format("...... result : {0}\r\n", r ? "Passed" : "Failed");

            if (!r) {
                myTesting.logSystem += string.Format("...... error : app tftpd64/tftpd32 is not running\r\n");
                myTesting.errorMessage = "Error upload firmware basic => app tftpd64/tftpd32 is not running.";
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool _checkFireWall() {
            bool r = false;

            myTesting.logSystem += string.Format("check firewall ...................\r\n");
            myTesting.logSystem += string.Format("...... standard: disable\r\n");
            r = !new FirewallManager.FirewallCom().Enable;

            myTesting.logSystem += string.Format("...... actual : {0}\r\n", r ? "disable" : "enable");
            myTesting.logSystem += string.Format("...... result : {0}\r\n", r ? "Passed" : "Failed");

            if (!r) {
                myTesting.logSystem += string.Format("...... error : firewall is enable\r\n");
                myTesting.errorMessage = "Error upload firmware basic => firewall is enable.";
            }

            return r;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool _eraseCalibWlan() {
            myTesting.EraseCalibWlan = "Waiting...";
            bool r = false;
            string cmd = "erase 0x9fff0000 +0x10000";
            string pattern = "ath>"; //ath>
            string actual = "";
            int timeout = 3; //sec

            myTesting.logSystem += string.Format("erase data calib wlan ...................\r\n");
            myTesting.logSystem += string.Format("...... command: \"{0}\"\r\n", cmd);
            myTesting.logSystem += string.Format("...... standard: \"{0}\"\r\n", pattern);
            r = mesh.Query(cmd, pattern, timeout, out actual);

            myTesting.logSystem += string.Format("...... actual : \"{0}\"\r\n", actual);
            myTesting.logSystem += string.Format("...... result : {0}\r\n", r ? "Passed" : "Failed");

            if (!r) {
                myTesting.logSystem += string.Format("... error : can't erased data calib wlan\r\n");
                myTesting.errorMessage = "Error upload firmware basic => can't erased data calib wlan.";
            }
            myTesting.EraseCalibWlan = r ? "Passed" : "Failed";
            return r;
        }


        private bool _transferFileKernel() {
            bool r = false;
            string cmd = "tftp 0x80060000 " + mySetting.FileKernel + " && erase 0x9fe80000 +${filesize} && cp.b $fileaddr 0x9fe80000 $filesize";
            string pattern1 = "Copy to Flash..."; //ath>
            string pattern2 = "done"; //ath>
            string pattern3 = "ath>"; //ath>
            string actual = "";
            int timeout = mySetting.timeOutTransferFileKernel; //sec

            myTesting.logSystem += string.Format("transfer kernel file to imap ...................\r\n");
            myTesting.logSystem += string.Format("...... command: \"{0}\"\r\n", cmd);
            myTesting.logSystem += string.Format("...... standard: \"{0}\",\"{1}\",\"{2}\"\r\n", pattern1, pattern2, pattern3);
            r = mesh.Query(cmd, out actual, timeout, pattern1, pattern2, pattern3);

            myTesting.logSystem += string.Format("...... actual : \"{0}\"\r\n", actual);
            myTesting.logSystem += string.Format("...... result : {0}\r\n", r ? "Passed" : "Failed");

            if (!r) {
                myTesting.logSystem += string.Format("...... error : can't transfer kernel file to imap\r\n");
                myTesting.errorMessage = "Error upload firmware basic => can't transfer kernel file to imap.";
            }
            return r;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool _transferFileRoot() {
            bool r = false;
            string cmd = "tftp 0x80060000 " + mySetting.FileRoot + " && erase 0x9f050000 +${filesize} && cp.b $fileaddr 0x9f050000 $filesize && boot";
            string pattern = "Starting kernel"; //ath>
            string actual = "";
            int timeout = mySetting.timeOutTransferFileRoot; //sec

            myTesting.logSystem += string.Format("transfer root file to imap ...................\r\n");
            myTesting.logSystem += string.Format("...... command: \"{0}\"\r\n", cmd);
            myTesting.logSystem += string.Format("...... standard: \"{0}\"\r\n", pattern);
            r = mesh.Query(cmd, pattern, timeout, out actual);

            myTesting.logSystem += string.Format("...... actual : \"{0}\"\r\n", actual);
            myTesting.logSystem += string.Format("...... result : {0}\r\n", r ? "Passed" : "Failed");

            if (!r) {
                myTesting.logSystem += string.Format("...... error : can't transfer root file to imap\r\n");
                myTesting.errorMessage = "Error upload firmware basic => can't transfer root file to imap.";
            }
            return r;
        }
        

    }
}
