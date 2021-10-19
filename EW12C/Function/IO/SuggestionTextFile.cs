using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EW12C.Function.Custom;
using UtilityPack.IO;

namespace EW12C.Function.IO {

    public class SuggestionTextFile {

        public List<SuggestionText> Read() {
            try {
                return CsvHelper<SuggestionText>.FromCsvFile(string.Format("{0}ref_\\SuggestionText.csv", AppDomain.CurrentDomain.BaseDirectory), Encoding.Unicode, ",");
            }
            catch {
                return null;
            }
        }
    }
}
