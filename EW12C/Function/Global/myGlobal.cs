using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EW12C.Function.Custom;
using EW12C.Function.IO;

namespace EW12C.Function.Global {
    public class myGlobal {

        public static string settingFileFullName = string.Format("{0}setting.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static volatile string loginUser = "";
        public static volatile string loginPassword = "";

        public static MainWindowInfo myMainWindow = new MainWindowInfo();
        public static TestingInformation myTesting = new TestingInformation();
        public static SettingInformation mySetting = new SettingInformation();
        public static AuthorizationInfo myAuthorization = null;

        public static VnptAsmTestFunctionLogInfo myLogTotal = null;
        public static string detailDirectory = "";
        public static string totalDirectory = "";
        public static string dir_Path = AppDomain.CurrentDomain.BaseDirectory.Replace("EW12C\\", "");


    }
}
