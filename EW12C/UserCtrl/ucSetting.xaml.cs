using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using EW12C.Function.Custom;
using EW12C.Function.Global;
using EW12C.Function.IO;
using UtilityPack.IO;

namespace EW12C.UserCtrl {
    /// <summary>
    /// Interaction logic for ucSetting.xaml
    /// </summary>
    public partial class ucSetting : UserControl {

        public ucSetting() {
            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(myGlobal.settingFileFullName)) myGlobal.mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.settingFileFullName);
            myGlobal.mySetting.isUploadFW = myGlobal.mySetting.StationName == "UploadFirmwareBasic" ? true : false;

            //load itemsource combobx
            this.init_Combobox_Itemsoure();

            //binding data
            this.DataContext = myGlobal.mySetting;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string button_tag = (string)b.Tag;

            switch (button_tag) {
                case "save_setting": {
                        XmlHelper<SettingInformation>.ToXmlFile(myGlobal.mySetting, myGlobal.settingFileFullName); //save setting to xml file
                        MessageBox.Show("Success.", string.Format("Save Setting DUT-{0}", myGlobal.mySetting.StationNumber), MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                case "browse_kernel": {
                        OpenFileDialog fileDialog = new OpenFileDialog();
                        fileDialog.InitialDirectory = "C:\\";
                        fileDialog.Filter = "file kernel *.bin|*.bin";
                        fileDialog.Title = "Select file kernel";
                        fileDialog.FileName = "kernel.bin";
                        if (fileDialog.ShowDialog() == true) {
                            myGlobal.mySetting.FileKernel = fileDialog.SafeFileName;
                        }
                        break;
                    }
                case "browse_root": {
                        OpenFileDialog fileDialog = new OpenFileDialog();
                        fileDialog.InitialDirectory = "C:\\";
                        fileDialog.Filter = "file root *.bin|*.bin";
                        fileDialog.Title = "Select file root";
                        fileDialog.FileName = "root.bin";
                        if (fileDialog.ShowDialog() == true) {
                            myGlobal.mySetting.FileRoot = fileDialog.SafeFileName;
                        }
                        break;
                    }
                case "browse_log_path": {
                        System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
                        if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                            myGlobal.mySetting.logDirectory = folderBrowser.SelectedPath;
                        }
                        break;
                    }
            }
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            e.Handled = !((ComboBox)sender).IsDropDownOpen;
        }

        private void init_Combobox_Itemsoure() {
            //load combobox itemsoure
            var Indexers = new List<string>();
            var Comports = new List<string>();
            var Stations = new List<string>();

            List<SuggestionText> suggestiontexts = new SuggestionTextFile().Read();
            if (suggestiontexts != null) {
                foreach (var x in suggestiontexts) {
                    if (string.IsNullOrEmpty(x.index) == false && string.IsNullOrWhiteSpace(x.index) == false) {
                        Indexers.Add(x.index);
                    }
                    if (string.IsNullOrEmpty(x.port) == false && string.IsNullOrWhiteSpace(x.port) == false) {
                        Comports.Add(x.port);
                    }
                    if (string.IsNullOrEmpty(x.station) == false && string.IsNullOrWhiteSpace(x.station) == false) {
                        Stations.Add(x.station);
                    }
                }
            }

            cbbFac.ItemsSource = cbbLine.ItemsSource = cbbStationNumber.ItemsSource = Indexers;
            cbbSerialPort.ItemsSource = Comports;
            cbbStationName.ItemsSource = Stations;
        }

    }
}
