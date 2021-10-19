using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using EW12CG.Function.Global;

namespace EW12CG.Function.IO {
    public class LogDetailFile {

        string logdir = myGlobal.dir_Path;
        string mac = myGlobal.myTesting.MacAddress.Replace("\"", "");

        public LogDetailFile() {
            logdir = string.Format("{0}Logdetail", logdir);
            logdir = string.Format("{0}\\{1}", logdir, myGlobal.mySetting.StationName);
            logdir = string.Format("{0}\\EW12CG", logdir);
            logdir = string.Format("{0}\\{1}", logdir, myGlobal.mySetting.StationNumber);
            logdir = string.Format("{0}\\{1}", logdir, DateTime.Now.ToString("yyyy-MM-dd"));
            logdir = string.Format("{0}\\{1}", logdir, string.Format("{0}_{1}_{2}", mac, DateTime.Now.ToString("HHmmss"), myGlobal.myTesting.totalResult));
            createLogDirectory(logdir);

            myGlobal.detailDirectory = logdir;
        }

        private void createLogDirectory(string logdirectory) {
            if (!Directory.Exists(logdirectory)) {
                Directory.CreateDirectory(logdirectory);
                Thread.Sleep(100);
            }
        }

        public void createLog() {
            //create log system
            string file_log_system = string.Format("EW12CG_{0}_{1}_{2}_system.txt", mac, DateTime.Now.ToString("HHmmss"), myGlobal.myTesting.totalResult);
            File.WriteAllText(System.IO.Path.Combine(logdir, file_log_system), "Product: EW12CG\r\n" + myGlobal.myTesting.logSystem, Encoding.Unicode);

            //create log uart
            string file_log_uart = string.Format("EW12CG_{0}_{1}_{2}_uart.txt", mac, DateTime.Now.ToString("HHmmss"), myGlobal.myTesting.totalResult);
            File.WriteAllText(System.IO.Path.Combine(logdir, file_log_uart), "Product: EW12CG\r\n" + myGlobal.myTesting.logUart, Encoding.Unicode);
        }







    }
}
