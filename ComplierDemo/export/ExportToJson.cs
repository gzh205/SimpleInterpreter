using CompilerDemo.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerDemo.export {
    public class ExportToJson {
        Dictionary<string,Function> dir;
        public ExportToJson(Dictionary<string,Function> input) {
            this.dir = input;
        }
        public void write(string filename) {
            StreamWriter sw = new StreamWriter(new FileStream(filename,FileMode.Create));
            string res = "[";
            foreach (string key in dir.Keys) {
                res+=dir[key]+",";
            }
            res = res.Substring(0,res.Length - 1);
            sw.Write(res+"]");
            sw.Close();
        }
    }
}
