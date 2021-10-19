using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using EW30SX.Function.Global;
using EW30SX.UserCtrl;
using EW30SX.Function.Custom;

namespace EW30SX {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        Thread thrd_login = null;

        //calculate startup location
        void calStartupLocation() {

            int z = int.Parse(myGlobal.mySetting.StationNumber) % 4;
            if (z == 0) z = 4;
            double x = 0.25 * (z - 1);

            this.Top = 5;
            this.Left = SystemParameters.WorkArea.Width * x + 5;
            this.Width = SystemParameters.WorkArea.Width * 0.25;
            this.Height = SystemParameters.WorkArea.Height - 10;
        }

        //Constructor MainWindow
        public MainWindow() {
            InitializeComponent();
            calStartupLocation();
            this.DataContext = myGlobal.myMainWindow;
            //add runall
            addUserControl(this.gridContent, "runall");
        }

        //
        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            Label l = sender as Label;
            string label_tag = (string)l.Tag;

            switch (label_tag) {
                case "about":
                case "runall":
                case "manual":
                case "setting": {
                        addUserControl(this.gridContent, label_tag);
                        break;
                    }
                case "close": {
                        this.Close();
                        break;
                    }
                case "version": {
                        break;
                    }
                case "help": {
                        break;
                    }

                default: break;
            }
        }

        private void addUserControl(Grid mainGrid, string name) {
            mainGrid.Children.Clear();
            switch (name) {
                case "runall": {
                        if (thrd_login != null) thrd_login.Abort();
                        mainGrid.Children.Add(new ucRunAll());
                        this.lblMinus.Margin = new Thickness(5, 0, 0, 0);
                        break;
                    }
                case "manual": {
                        if (thrd_login != null) thrd_login.Abort();
                        mainGrid.Children.Add(new ucManual());
                        this.lblMinus.Margin = new Thickness(95, 0, 0, 0);
                        break;
                    }
                case "setting": {
                        myGlobal.myAuthorization = new AuthorizationInfo();
                        mainGrid.Children.Add(new ucLogin());
                        this.lblMinus.Margin = new Thickness(185, 0, 0, 0);

                        thrd_login = new Thread(new ThreadStart(() => {
                            bool r = false;
                            while (true) {
                                if (myGlobal.myAuthorization.User == "admin" && myGlobal.myAuthorization.Password == "vnpt") {
                                    r = true;
                                    break;
                                }
                            }
                            App.Current.Dispatcher.Invoke(new Action(() => {
                                if (r) mainGrid.Children.Add(new ucSetting());
                            }));
                        }));
                        thrd_login.IsBackground = true;
                        thrd_login.Start();

                        break;
                    }
                case "about": {
                        if (thrd_login != null) thrd_login.Abort();
                        mainGrid.Children.Add(new ucAbout());
                        this.lblMinus.Margin = new Thickness(275, 0, 0, 0);
                        break;
                    }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            calStartupLocation();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                //this.DragMove();
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Image img = sender as Image;
                string tag = (string)img.Tag;
                switch (tag) {
                    case "open_log_detail": {
                            try {
                                Process.Start(myGlobal.detailDirectory);
                            }
                            catch {
                                Process.Start(myGlobal.dir_Path);
                            }
                            break;
                        }
                    case "open_log_total": {
                            try {
                                Process.Start(myGlobal.totalDirectory);
                            }
                            catch {
                                Process.Start(myGlobal.dir_Path);
                            }
                            break;
                        }
                }
            }
        }
    }
}
