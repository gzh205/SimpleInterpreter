using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerDemo.CodeCheck {
    public class ColorSet {
        public static int rownum=0;
        public int row;
        public int beginColumn;
        public int length;
        public string errorStatement;
        public Flag flag;
        public ColorSet(int begin,int length,Flag flag) {
            this.beginColumn = begin;
            this.length = length;
            this.flag = flag;
            this.row = rownum;
        }
        public enum Flag {
            Keyword,Function,Symbol,Variable,Number,String,Error
        };
    }
}
