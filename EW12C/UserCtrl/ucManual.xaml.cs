using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EW12C.Function.Custom;
using EW12C.Function.DUT;
using EW12C.Function.Global;
using EW12C.Function.Protocol;
using UtilityPack.IO;

namespace EW12C.UserCtrl {
    /// <summary>
    /// Interaction logic for ucManual.xaml
    /// </summary>
    public partial class ucManual : UserControl {

        ManualCheckLedInfo ledObject = new ManualCheckLedInfo();
        ManualCheckButtonInfo buttonObject = new ManualCheckButtonInfo();
        ManualCheckWanInfo wanObject = new ManualCheckWanInfo();
        
        ~ucManual() {
        }

        public ucManual() {
            //
            InitializeComponent();

            //load setting from file
            if (File.Exists(myGlobal.settingFileFullName)) myGlobal.mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.settingFileFullName);

            //binding data
            this.grid_check_led.DataContext = this.ledObject;
            this.grid_check_button.DataContext = this.buttonObject;
            this.grid_check_wan.DataContext = this.wanObject;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string button_tag = (string)b.Tag;

            switch (button_tag) {
                
                #region check LED
                //led power
                case "ledpower_green": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_red_system/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                case "ledpower_orange": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 1 > /sys/class/leds/led_red_system/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                //led inet
                case "ledinet_green": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo none > /sys/class/leds/led_green_wan_inet/trigger");
                        lst_commands.Add("echo 1 > /sys/class/leds/led_green_wan_inet/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                case "ledinet_off": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_green_wan_inet/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                //led wan
                case "ledwan_green": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 1 > /sys/class/leds/led_green/brightness");
                        //lst_commands.Add("echo 0 > /sys/class/leds/led_blue//brightness");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_red/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                case "ledwan_yellow": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_green/brightness");
                        //lst_commands.Add("echo 1 > /sys/class/leds/led_blue//brightness");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_red/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                case "ledwan_red": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_green/brightness");
                        //lst_commands.Add("echo 0 > /sys/class/leds/led_blue//brightness");
                        lst_commands.Add("echo 1 > /sys/class/leds/led_red/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                case "ledwan_off": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_green/brightness");
                        //lst_commands.Add("echo 0 > /sys/class/leds/led_blue//brightness");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_red/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                #endregion


                #region check Button
                case "buttonwps_start": {
                        _wait_button("WPS PUSH BUTTON EVENT DETECTED", 30, b);
                        break;
                    }
                case "buttonreset_start": {
                        _wait_button("REBOOT", 30, b);
                        break;
                    }
                #endregion


                #region check Wan
                case "wanping_start": {
                        bool r = Network.pingNetwork("192.168.88.1");
                        MessageBox.Show(r ? "Passed" : "Failed", "Ping Result", MessageBoxButton.OK, r ? MessageBoxImage.Information : MessageBoxImage.Error);
                        break;
                    }
                case "wanspeed_start": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("cat /sys/class/net/eth0/speed");
                        _control_wan(lst_commands, b);
                        break;
                    }
                    #endregion

            }
        }


        private void _control_wan(List<string> cmds, Button b) {
            Thread t = new Thread(new ThreadStart(() => {
                wanObject.logUart = "";
                Dispatcher.Invoke(new Action(() => { b.Background = Brushes.Lime; }));
                var mySetting = myGlobal.mySetting;
                var mesh = new meshAP<ManualCheckWanInfo, SettingInformation>(wanObject, mySetting, "logUart", "SerialPortName");
                bool r = mesh.Open();
                if (!r) {
                    MessageBox.Show("Can't open imap serial port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Dispatcher.Invoke(new Action(() => { b.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#cbd6d5"); }));
                    return;
                }
                r = mesh.Query("\n", "root@VNPT:/#", 3);
                foreach (var cmd in cmds) r = mesh.Query(cmd, "root@VNPT:/#", 3);
                mesh.Close();
                Dispatcher.Invoke(new Action(() => { b.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#cbd6d5"); }));
            }));
            t.IsBackground = true;
            t.Start();
        }

        private void _control_led(List<string> cmds, Button b) {
            Thread t = new Thread(new ThreadStart(() => {
                ledObject.logUart = "";
                Dispatcher.Invoke(new Action(() => { b.Background = Brushes.Lime; }));
                var mySetting = myGlobal.mySetting;
                var mesh = new meshAP<ManualCheckLedInfo, SettingInformation>(ledObject, mySetting, "logUart", "SerialPortName");
                bool r = mesh.Open();
                if (!r) {
                    MessageBox.Show("Can't open imap serial port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Dispatcher.Invoke(new Action(() => { b.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#cbd6d5"); }));
                    return;
                }
                r = mesh.Query("\n", "root@VNPT:/#", 3);
                foreach (var cmd in cmds) r = mesh.Query(cmd, "root@VNPT:/#", 3);
                mesh.Close();
                Dispatcher.Invoke(new Action(() => { b.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#cbd6d5"); }));
            }));
            t.IsBackground = true;
            t.Start();
        }

        private void _wait_button(string recognition_text, int timeout, Button b) {
            string tag = (string)b.Tag;
            Thread t = new Thread(new ThreadStart(() => {
                buttonObject.logUart = "";
                Dispatcher.Invoke(new Action(() => { b.Content = "Stop"; b.Background = Brushes.Lime; }));
                var mySetting = myGlobal.mySetting;
                var mesh = new meshAP<ManualCheckButtonInfo, SettingInformation>(buttonObject, mySetting, "logUart", "SerialPortName");
                bool r = mesh.Open();
                if (!r) {
                    MessageBox.Show("Can't open imap serial port.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Dispatcher.Invoke(new Action(() => { b.Content = "Start"; b.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#cbd6d5"); }));
                    return;
                }
                int count = 0;

                RE:
                count++;
                r = buttonObject.logUart.ToLower().Contains(recognition_text.ToLower());
                switch (tag) {
                    case "buttonwps_start": {
                            buttonObject.LegendReset = "-";
                            buttonObject.LegendWps = string.Format("Please press button wps (remaining: {0} sec)", (timeout * 2) - count);
                            break;
                        }
                    case "buttonreset_start": {
                            buttonObject.LegendReset = string.Format("Please press button reset (remaining: {0} sec)", (timeout * 2) - count);
                            buttonObject.LegendWps = "-";
                            break;
                        }
                }

                if (!r) {
                    if (count < timeout * 2) {
                        Thread.Sleep(500);
                        goto RE;
                    }
                }

                switch (tag) {
                    case "buttonwps_start": {
                            buttonObject.LegendReset = "-";
                            buttonObject.LegendWps = string.Format("{0}", r ? "Passed" : "Failed");
                            break;
                        }
                    case "buttonreset_start": {
                            buttonObject.LegendReset = string.Format("{0}", r ? "Passed" : "Failed");
                            buttonObject.LegendWps = "-";
                            break;
                        }
                }

                mesh.Close();
                Dispatcher.Invoke(new Action(() => { b.Content = "Start"; b.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#cbd6d5"); }));
            }));
            t.IsBackground = true;
            t.Start();
        }

    }
}
