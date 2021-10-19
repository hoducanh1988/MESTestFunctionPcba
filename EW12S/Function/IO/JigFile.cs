using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EW12S.Function
{
    public class JigFile
    {
        private static string _filepath = System.AppDomain.CurrentDomain.BaseDirectory;

        public static string get() {
            string _file = string.Format("{0}jig.ini", _filepath);
            if (File.Exists(_file) == false) return null;
            return File.ReadAllText(_file);
        }

        public static bool set(string _data) {
            try {
                string _file = string.Format("{0}jig.ini", _filepath);
                File.WriteAllText(_file, _data);
                return true;
            } catch {
                return false;
            }
        }

    }
}
