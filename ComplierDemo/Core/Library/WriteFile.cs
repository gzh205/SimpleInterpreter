using CompilerDemo.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplierDemo.Core.Library {
    public class WriteFile : Function {
        public WriteFile() {
            parametername.Add("filename");
            parametername.Add("input");
        }
        public override Data run() {
            if (parametervalue.Count < 2 || !(parametervalue[0].isStr && parametervalue[1].isStr)) {
                throw new SyntaxErrorException("WriteFile函数参数数量不合要求");
            }
            StreamWriter sw = new StreamWriter(new FileStream(parametervalue[0].str,FileMode.OpenOrCreate));
            sw.Write(parametervalue[1].str);
            sw.Close();
            return new Data(0);
        }
    }
}
