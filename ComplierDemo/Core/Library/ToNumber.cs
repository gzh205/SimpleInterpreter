using System;

namespace CompilerDemo.Core.Library {
    public class ToNumber : Function{
        public ToNumber() {
            parametername.Add("input");
        }
        public override Data run() {
            Data res = parametervalue[0];
            if (!res.isEmpty()) {
                if (res.isStr) {
                    res = new Data(Convert.ToDouble(res.str));
                }
            } else {
                res = new Data(0);
            }
            return res;
        }
    }
}
