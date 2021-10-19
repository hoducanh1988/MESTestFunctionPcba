using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using EW30SX.Function.Global;
using System.Reflection;

namespace EW30SX.Function.IO {
    public class LogDetailFile {

        string logdir = myGlobal.dir_Path;
        string mac = myGlobal.myTesting.MacAddress.Replace("\"", "");

        public LogDetailFile() {
            logdir = string.Format("{0}Logdetail", logdir);
            logdir = string.Format("{0}\\{1}", logdir, myGlobal.mySetting.StationName);
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

        private void getSettingInfo(ref string str) {
            str += "SETTING INFO\n";
            PropertyInfo[] propers = myGlobal.mySetting.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in propers) {
                string data = $"{p.Name}={p.GetValue(myGlobal.mySetting, null)}";
                str += $"{data}\n";
            }
            str += "+++++++++++++++++++++++++++++++++++++++++\n";
        }

        private void getAppInfo(ref string str) {
            str += "APP INFO\n";
            str += $"{myGlobal.myMainWindow.appInfo}\n";
            str += "+++++++++++++++++++++++++++++++++++++++++\n";
        }

        public void createLog() {
            string log_data = "";
            getAppInfo(ref log_data);
            getSettingInfo(ref log_data);

            //create log system
            string file_log_system = string.Format("EW30SX_{0}_{1}_{2}_system.txt", mac, DateTime.Now.ToString("HHmmss"), myGlobal.myTesting.totalResult);
            using (var sw = new StreamWriter(System.IO.Path.Combine(logdir, file_log_system), true, Encoding.Unicode)) {
                sw.WriteLine("Product: EW30SX");
                sw.WriteLine(log_data);
                sw.WriteLine(myGlobal.myTesting.logSystem);
            }
            
            //create log uart
            string file_log_uart = string.Format("EW30SX_{0}_{1}_{2}_uart.txt", mac, DateTime.Now.ToString("HHmmss"), myGlobal.myTesting.totalResult);
            using (var sw = new StreamWriter(System.IO.Path.Combine(logdir, file_log_uart), true, Encoding.Unicode)) {
                sw.WriteLine("Product: EW30SX");
                sw.WriteLine(log_data);
                sw.WriteLine(myGlobal.myTesting.logUart);
            }
        }







    }
}
