using System;
using System.Collections.Generic;

namespace CompilerDemo.Core {
    [Serializable]
    public class While : Sentence {
        public While(List<Element> toks,int deepth):base(toks,deepth) {
        }
        public Sentence judgesentence;
        public List<Sentence> sentences;
        public override string ToString() {
            string result = "{\"判断语句\":" + judgesentence.ToString() + ",\n\"循环表达式\":";
            for(int i=0;i<sentences.Count;i++) {
                result += sentences[i].ToString() + (i == sentences.Count - 1 ? "" : ",");
            }
            return result.Substring(0,result.Length - 1) + "}";
        }
    }
}
