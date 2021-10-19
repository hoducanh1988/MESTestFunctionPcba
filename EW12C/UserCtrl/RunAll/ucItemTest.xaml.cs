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
using EW12C.Function.Global;

namespace EW12C.UserCtrl {
    /// <summary>
    /// Interaction logic for ucItemTest.xaml
    /// </summary>
    public partial class ucItemTest : UserControl {
        public ucItemTest() {
            InitializeComponent();
            this.DataContext = myGlobal.myTesting;
        }
    }
}
