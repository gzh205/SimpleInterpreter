using System;
using System.Collections.Generic;

namespace CompilerDemo.Core {
    /// <summary>
    /// 脚本语言中的一个句子
    /// </summary>
    [Serializable]
    public class Sentence {
        public List<Element> toks;//一条语句中的单词和符号列表
        public int deepth;//该语句前有多少个Tab
        public Sentence(List<Element> toks,int deepth) {
            this.toks = toks;
            this.deepth = deepth;
        }
        public override string ToString() {
            string result = "{\"单词序列\":[";
            for(int i=0;i<toks.Count;i++) {
                result += toks[i].ToString() + (i == toks.Count - 1 ? "" : ",");
            }
            result += "],\"深度\":"+deepth+"}";
            return result;
        }
    }
}
