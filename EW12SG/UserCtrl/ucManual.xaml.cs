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
using EW12SG.Function.Custom;
using EW12SG.Function.DUT;
using EW12SG.Function.Global;
using EW12SG.Function.Protocol;
using UtilityPack.IO;

namespace EW12SG.UserCtrl {
    /// <summary>
    /// Interaction logic for ucManual.xaml
    /// </summary>
    public partial class ucManual : UserControl {

        ManualCheckLedInfo ledObject = new ManualCheckLedInfo();
        ManualCheckButtonInfo buttonObject = new ManualCheckButtonInfo();
        ManualCheckWanInfo wanObject = new ManualCheckWanInfo();
        ManualCheckLanInfo lanObject = new ManualCheckLanInfo();
        ManualGetInfo infoObject = new ManualGetInfo();

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
            this.grid_check_lan.DataContext = this.lanObject;
            this.grid_get_info.DataContext = this.infoObject;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string button_tag = (string)b.Tag;

            switch (button_tag) {

                #region check LED
                //led wan
                case "ledwan_green": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 1 > /sys/class/leds/led_green/brightness");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_red/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                case "ledwan_red": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_green/brightness");
                        lst_commands.Add("echo 1 > /sys/class/leds/led_red/brightness");
                        _control_led(lst_commands, b);
                        break;
                    }
                case "ledwan_off": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("killall led-state-run.sh");
                        lst_commands.Add("echo 0 > /sys/class/leds/led_green/brightness");
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

                #region check Lan
                case "lanping_start": {
                        bool r = Network.pingNetwork("192.168.88.1");
                        MessageBox.Show(r ? "Passed" : "Failed", "Ping Result", MessageBoxButton.OK, r ? MessageBoxImage.Information : MessageBoxImage.Error);
                        break;
                    }
                case "lanspeed_start": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("swconfig dev switch0 show | grep \"link: port:2\"");
                        _control_lan(lst_commands, b);
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
                        lst_commands.Add("swconfig dev switch0 show | grep \"link: port:3\"");
                        _control_wan(lst_commands, b);
                        break;
                    }
                #endregion

                #region Get Info

                case "get_mac": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("cat /sys/class/net/eth0/address");
                        _control_get_info(lst_commands, b);
                        break;
                    }
                case "get_hw": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("fw_printenv");
                        _control_get_info(lst_commands, b);
                        break;
                    }
                case "get_model": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("fw_printenv");
                        _control_get_info(lst_commands, b);
                        break;
                    }
                case "get_fw_version": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("cat /etc/fw_info");
                        _control_get_info(lst_commands, b);
                        break;
                    }
                case "get_fw_build_time": {
                        List<string> lst_commands = new List<string>();
                        lst_commands.Add("cat /proc/version");
                        _control_get_info(lst_commands, b);
                        break;
                    }

                    #endregion
            }
        }


        private void _control_get_info(List<string> cmds, Button b) {
            Thread t = new Thread(new ThreadStart(() => {
                infoObject.logUart = "";
                Dispatcher.Invoke(new Action(() => { b.Background = Brushes.Lime; }));
                var mySetting = myGlobal.mySetting;
                var mesh = new meshAP<ManualGetInfo, SettingInformation>(infoObject, mySetting, "logUart", "SerialPortName");
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

        private void _control_lan(List<string> cmds, Button b) {
            Thread t = new Thread(new ThreadStart(() => {
                lanObject.logUart = "";
                Dispatcher.Invoke(new Action(() => { b.Background = Brushes.Lime; }));
                var mySetting = myGlobal.mySetting;
                var mesh = new meshAP<ManualCheckLanInfo, SettingInformation>(lanObject, mySetting, "logUart", "SerialPortName");
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
