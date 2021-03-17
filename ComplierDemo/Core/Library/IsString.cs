using CompilerDemo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerDemo.Core.Library {
    public class IsString :Function{
        public IsString() {
            parametername.Add("input");
        }
        public override Data run() {
            Data res = parametervalue[0];
            return new Data(res.isStr ? 1 : 0);
        }
    }
}
