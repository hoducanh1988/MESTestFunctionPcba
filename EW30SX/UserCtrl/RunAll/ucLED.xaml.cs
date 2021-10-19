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
using EW30SX.Function.Global;
using EW30SX.Function.DUT;
using EW30SX.Function.Custom;
using System.Threading;

namespace EW30SX.UserCtrl {
    /// <summary>
    /// Interaction logic for ucLED.xaml
    /// </summary>
    public partial class ucLED : UserControl {
        meshAP<TestingInformation, SettingInformation> mesh = null;
        volatile bool flag_thread = false;

        public ucLED(meshAP<TestingInformation, SettingInformation> _mesh) {
            InitializeComponent();
            this.DataContext = myGlobal.myTesting;
            mesh = _mesh;

            Thread t = new Thread(new ThreadStart(() => {

                while (!flag_thread) {
                    led_wan_off();
                    Thread.Sleep(500);
                    led_wan_green();
                    Thread.Sleep(500);
                    led_wan_red();
                    Thread.Sleep(500);
                    //led_wan_yellow();
                    //Thread.Sleep(500);
                }
                
            }));
            t.IsBackground = true;
            t.Start();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e) {
            RadioButton radio = sender as RadioButton;
            string radio_tag = (string)radio.Tag;

            switch (radio_tag) {
                case "ledwan_passed": {
                        myGlobal.myTesting.ledWan = "Passed";
                        flag_thread = true;
                        break;
                    }
                case "ledwan_failed": {
                        myGlobal.myTesting.ledWan = "Failed";
                        flag_thread = true;
                        break;
                    }
            }
        }

        private void led_wan_off() {
            List<string> lst_commands = new List<string>();
            lst_commands.Add("killall led-state-run.sh");
            lst_commands.Add("echo 0 > /sys/class/leds/led_green/brightness");
            //lst_commands.Add("echo 0 > /sys/class/leds/led_blue//brightness");
            lst_commands.Add("echo 0 > /sys/class/leds/led_red/brightness");
            _control_led(lst_commands);
        }

        private void led_wan_green() {
            List<string> lst_commands = new List<string>();
            lst_commands.Add("echo 1 > /sys/class/leds/led_green/brightness");
            //lst_commands.Add("echo 0 > /sys/class/leds/led_blue//brightness");
            lst_commands.Add("echo 0 > /sys/class/leds/led_red/brightness");
            _control_led(lst_commands);
        }

        private void led_wan_red() {
            List<string> lst_commands = new List<string>();
            lst_commands.Add("echo 0 > /sys/class/leds/led_green/brightness");
            //lst_commands.Add("echo 0 > /sys/class/leds/led_blue//brightness");
            lst_commands.Add("echo 1 > /sys/class/leds/led_red/brightness");
            _control_led(lst_commands);
        }

        private void led_wan_yellow() {
            List<string> lst_commands = new List<string>();
            lst_commands.Add("echo 0 > /sys/class/leds/led_green/brightness");
            //lst_commands.Add("echo 1 > /sys/class/leds/led_blue//brightness");
            lst_commands.Add("echo 0 > /sys/class/leds/led_red/brightness");
            _control_led(lst_commands);
        }
        
        private void _control_led(List<string> cmds) {
            Thread t = new Thread(new ThreadStart(() => {
                bool r = false;
                r = mesh.Query("\n", "root@VNPT:/#", 3);
                foreach (var cmd in cmds) r = mesh.Query(cmd, "root@VNPT:/#", 3);
            }));
            t.IsBackground = true;
            t.Start();
        }

    }
}
