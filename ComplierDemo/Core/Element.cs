using System;

namespace CompilerDemo.Core {
    /// <summary>
    /// 用于记录一个字面值
    /// </summary>
    [Serializable]
    public class Element {
        public string value;//字符串部分
        public int flag;//一个标记,用于区分value字符串的意义
        public Element(string value,int flag) {
            this.value = value;
            this.flag = flag;
        }
        public const int Number = 1;
        public const int String = 2;
        public const int Symbol = 3;
        public const int Keyword = 4;
        public const int Variable = 5;
        public const int Function = 6;
        public const int Array = 7;
        public override string ToString() {
            return "{\"值\":\"" + value + "\",\"符号\":\"" + flag + "\"}";
        }
    }
}
