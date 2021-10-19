using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO.Ports;
using System.Windows;
using System.Reflection;

namespace EW12C.Function.Protocol {

    public class RS232<T, S> where T : class, new() where S : class, new() {

        public bool isConnected = false;
        public string logVariableName = "";
        public T testing = null;
        public S setting = null;

        SerialPort serialPort = null;
        string portVariableName = "";
        string dataReceived = "";

        public RS232(T t, S s, string log_variable, string port_variable) {
            this.testing = t;
            this.setting = s;
            this.logVariableName = log_variable;
            this.portVariableName = port_variable;
        }

        public bool Open() {
            try {
                PropertyInfo port = this.setting.GetType().GetProperty(this.portVariableName);
                string port_name = (string)port.GetValue(this.setting, null);

                this.serialPort = new SerialPort();
                this.serialPort.PortName = port_name;
                this.serialPort.BaudRate = 115200;
                this.serialPort.Parity = Parity.None;
                this.serialPort.DataBits = 8;
                this.serialPort.StopBits = StopBits.One;
                this.serialPort.Open();
                this.serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_OnReceiveData);
                this.isConnected = this.serialPort.IsOpen;
                return this.isConnected;
            }
            catch {
                return false;
            }
        }

        private void serialPort_OnReceiveData(object sender, SerialDataReceivedEventArgs e) {
            SerialPort s = (SerialPort)sender;
            PropertyInfo log = this.testing.GetType().GetProperty(this.logVariableName);
            string data = (string)log.GetValue(this.testing, null);
            string str_temp = s.ReadExisting();
            dataReceived += str_temp;
            data += str_temp;
            log.SetValue(this.testing, Convert.ChangeType(data, log.PropertyType), null);
        }

        public bool Write(string cmd) {
            if (!isConnected) return false;
            this.dataReceived = "";
            this.serialPort.Write(cmd);
            return true;
        }

        public bool WriteLine(string cmd) {
            if (!isConnected) return false;
            return this.Write(string.Format("{0}\n", cmd));
        }

        public string Query(string cmd, int delay) {
            if (!isConnected) return null;
            this.WriteLine(cmd); Thread.Sleep(delay);
            return this.dataReceived;
        }

        public bool Query(string cmd, string pattern, int timeout_sec, out string feedback) {
            feedback = "";
            if (!isConnected) return false;
            this.WriteLine(cmd);
            timeout_sec = timeout_sec * 1000;
            timeout_sec = timeout_sec < 100 ? 100 : timeout_sec;
            int max = timeout_sec / 100;
            int count = 0;
            bool r = false;
            RE:
            count++;
            r = this.dataReceived.ToLower().Contains(pattern.ToLower());
            if (!r) {
                if (count < max) {
                    Thread.Sleep(100);
                    goto RE;
                }
            }
            feedback = this.dataReceived;
            return r;
        }


        public bool Query(string cmd, out string feedback, int timeout_sec, params string[] patterns) {
            feedback = "";
            if (!isConnected) return false;
            this.WriteLine(cmd);
            timeout_sec = timeout_sec * 1000;
            timeout_sec = timeout_sec < 100 ? 100 : timeout_sec;
            int max = timeout_sec / 100;
            int count = 0;
            bool r = false;
        RE:
            count++;
            foreach ( var pattern in patterns) {
                r = this.dataReceived.ToLower().Contains(pattern.ToLower());
                if (!r) break;
            }
            if (!r) {
                if (count < max) {
                    Thread.Sleep(100);
                    goto RE;
                }
            }
            feedback = this.dataReceived;
            return r;
        }

        public bool Query(string cmd, string pattern, int timeout_sec, int retry_time, out string feedback) {
            feedback = "";
            if (!isConnected) return false;

            bool r = false;
            int count = 0;
            RE:
            count++;
            string data_out = "";
            r = this.Query(cmd, pattern, timeout_sec, out data_out);
            feedback += data_out;
            if (!r) {
                if (count < retry_time) {
                    goto RE;
                }
            }

            return r;
        }

        public bool Query(string cmd, string pattern, int timeout_sec) {
            if (!isConnected) return false;
            this.WriteLine(cmd);
            timeout_sec = timeout_sec * 1000;
            timeout_sec = timeout_sec < 100 ? 100 : timeout_sec;
            int max = timeout_sec / 100;
            int count = 0;
            bool r = false;
            RE:
            count++;
            r = this.dataReceived.ToLower().Contains(pattern.ToLower());
            if (!r) {
                if (count < max) {
                    Thread.Sleep(100);
                    goto RE;
                }
            }
            return r;
        }

        public bool Query(string cmd, string pattern, int timeout_sec, int retry_time) {
            if (!isConnected) return false;

            bool r = false;
            int count = 0;
            RE:
            count++;
            r = this.Query(cmd, pattern, timeout_sec);
            if (!r) {
                if (count < retry_time) {
                    goto RE;
                }
            }
            
            return r;
        }

        public bool QueryAndLog(string cmd, string pattern, int timeout_sec) {
            if (!isConnected) return false;
            this.WriteLine(cmd);
            int count = 0;
            bool r = false;
            PropertyInfo logsys = this.testing.GetType().GetProperty("logSystem");
            string data = (string)logsys.GetValue(this.testing, null);
            data += "... waitting.";
            logsys.SetValue(this.testing, Convert.ChangeType(data, logsys.PropertyType), null);
        RE:
            count++;
            data += string.Format("..{0}", count);
            logsys.SetValue(this.testing, Convert.ChangeType(data, logsys.PropertyType), null);
            r = this.dataReceived.ToLower().Contains(pattern.ToLower());
            if (!r) {
                if (count < timeout_sec) {
                    Thread.Sleep(1000);
                    goto RE;
                }
            }
            return r;
        }

        public string Read() {
            return this.dataReceived;
        }

        public bool Close() {
            try {
                if (this.serialPort == null) return true;
                if (this.isConnected == true) this.serialPort.Close();
                return true;
            }
            catch {
                return false;
            }
        }

    }
}
