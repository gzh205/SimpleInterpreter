using CompilerDemo.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplierDemo.Core.Library {
    public class ReadFile : Function {
        public ReadFile() {
            parametername.Add("filename");
        }
        public override Data run() {
            if (parametervalue.Count < 1 || !parametervalue[0].isStr) {
                throw new SyntaxErrorException("语法错误:参数不合法");
            }
            Data res = parametervalue[0];
            StreamReader sr = null;
            try {
                sr = new StreamReader(new FileStream(res.str,FileMode.Open));
                res.str = sr.ReadToEnd();
            } catch (FileNotFoundException) {
                throw new SyntaxErrorException("文件不存在");
            } finally {
                sr.Close();
            }
            return res;
        }
    }
}
