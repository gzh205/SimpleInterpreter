using CompilerDemo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerDemo.Core.Library {
    public class Print : Function {
        public Print() {
            parametername.Add("input");
        }
        public override Data run() {
            //处理参数
            if (parametervalue.Count < 1) {
                throw new SyntaxErrorException("语法错误:Print参数过少");
            }
            Data res = parametervalue[0];
            if (!res.isEmpty()) {
                Console.Write(res.isStr ? res.str : ""+res.num);
            }
            return new Data(0);
        }
    }
}
