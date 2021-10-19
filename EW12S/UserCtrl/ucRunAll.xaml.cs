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
using System.Windows.Threading;
using EW12S.Function.Custom;
using EW12S.Function.Global;
using EW12S.Function.Excute;
using EW12S.Function.IO;
using UtilityPack.IO;
using EW12S.Function.DUT;

namespace EW12S.UserCtrl {
    /// <summary>
    /// Interaction logic for ucRunAll.xaml
    /// </summary>
    public partial class ucRunAll : UserControl {

        public ucRunAll() {
            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(myGlobal.settingFileFullName)) myGlobal.mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.settingFileFullName);
            myGlobal.mySetting.isUploadFW = myGlobal.mySetting.StationName == "UploadFirmwareBasic" ? true : false;

            //binding data
            this.DataContext = myGlobal.myTesting;

            this.grid_TestItem.Children.Add(new ucItemTest());

            //control scrollviewer scroll to end
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += (sender, e) => {
                if (myGlobal.myTesting.totalResult == "Waiting...") {
                    this.Scr_LogSystem.ScrollToEnd();
                    this.Scr_LogUart.ScrollToEnd();
                }
            };
            timer.Start();

            Thread t = new Thread(new ThreadStart(() => {
                Thread.Sleep(100);
                Dispatcher.Invoke(new Action(() => { this.txtInputMac.Focus(); }));
            }));
            t.IsBackground = true;
            t.Start();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter) return;
            TextBox tbox = sender as TextBox;
            string tag = (string)tbox.Tag;
            string text = (string)tbox.Text.ToUpper();

            switch (tag) {
                case "input_mac": {
                        Thread t = new Thread(new ThreadStart(() => {
                            //callback runall
                            var runall = new excRunAll(text, this.grid_TestItem);
                            bool r = runall.Excuting();
                            
                            //set textbox focus
                            Dispatcher.Invoke(new Action(() => {
                                tbox.Clear();
                                tbox.Focus();
                            }));

                            //save log detail
                            new LogDetailFile().createLog();

                            //save log total
                            new LogTotalFile().createLog(myGlobal.myLogTotal, new VnptLogMoreInfo());

                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }
            }
        }
    }
}
