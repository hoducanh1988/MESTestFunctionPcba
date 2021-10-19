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
using System.Diagnostics;
using System.IO;
using registrationLogin.Global;
using UtilityPack.IO;
using registrationLogin.Custom;

namespace registrationLogin {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        string dir = AppDomain.CurrentDomain.BaseDirectory;

        public MainWindow() {
            InitializeComponent();
            
            //load setting from file
            if (File.Exists(myGlobal.settingFileFullName)) myGlobal.mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.settingFileFullName);

            //set itemsource for combobox
            this.cbbModel.ItemsSource = new List<string>() { "EW12S", "EW12CG", "EW12SG", "EW12C", "EW30SX", "EW30CX" };
            this.cbbStation.ItemsSource = new List<string>() { "UploadFirmwareBasic", "TestFunctionPcba" };

            //binding data
            this.DataContext = myGlobal.mySetting;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            string model = this.cbbModel.Text;
            string app_test = string.Format("{0}{1}\\{1}.exe", dir, model);
            string app_setting = string.Format("{0}{1}\\setting.xml", dir, model);
            switch (model) {
                case "EW30CX":
                case "EW30SX":
                case "EW12C":
                case "EW12S":
                case "EW12SG":
                case "EW12CG": {
                        //check đường dẫn log folder
                        if (!UtilityPack.Validation.Parse.isLogPathValid(122)) {
                            MessageBox.Show("Đường dẫn folder phần mềm quá dài hoặc có kí tự không hợp lệ.\r\nVui lòng copy folder phần mềm sang đường dẫn khác.", "Lỗi đường dẫn", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        Process.Start(app_test);
                        XmlHelper<SettingInformation>.ToXmlFile(myGlobal.mySetting, myGlobal.settingFileFullName); //save setting to xml file
                        changeAppSetting(app_setting);
                        this.Close();
                        break;
                    }
                default: {
                        MessageBox.Show("Vui lòng chọn model sản phẩm.", "Lỗi chọn model sản phẩm", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                    }
            }
        }


        private void changeAppSetting(string app_setting_file) {
            
            //read app setting file
            string[] lines = File.ReadAllLines(app_setting_file);
            List<string> buffer = new List<string>();
            foreach (var l in lines) {
                if (l.Contains("StationName")) {
                    string old_text = get_text_from_setting_line(l);
                    string new_text = string.Format(">{0}</",myGlobal.mySetting.StationName);
                    buffer.Add(l.Replace(old_text, new_text));
                }
                else if (l.Contains("StationNumber")) {
                    string old_text = get_text_from_setting_line(l);
                    string new_text = string.Format(">{0}</", myGlobal.mySetting.JigNumber);
                    buffer.Add(l.Replace(old_text, new_text));
                }
                else {
                    buffer.Add(l);
                }
            }

            //delete app setting file
            System.IO.File.Delete(app_setting_file);

            //save new app setting file
            using (var sw = new StreamWriter(app_setting_file, true)) {
                foreach (var l in buffer) {
                    sw.WriteLine(l);
                }
            }
            
        }

        private string get_text_from_setting_line(string line) {
            string data = line.Split(new string[] { "</" }, StringSplitOptions.None)[0];
            data = data.Split(new string[] { ">" }, StringSplitOptions.None)[1];
            return string.Format(">{0}</", data);
        }

    }
}
