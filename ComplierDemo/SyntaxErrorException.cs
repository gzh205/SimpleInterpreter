using System;
using System.Collections.Generic;
using System.Text;

namespace CompilerDemo {
    public class SyntaxErrorException :Exception{
        public SyntaxErrorException(string msg) : base(msg) { }
    }
}
