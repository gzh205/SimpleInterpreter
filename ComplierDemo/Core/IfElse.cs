using System;
using System.Collections.Generic;

namespace CompilerDemo.Core {
    [Serializable]
    public class IfElse :Sentence {
        public IfElse(List<Element> toks,int deepth) : base(toks,deepth) {
        }
        public List<Branch> branchs = new List<Branch>();
        public override string ToString() {
            string result = "{\"分支结构\":";
            for(int i=0;i<branchs.Count;i++) {
                result += branchs[i].ToString() + (i == branchs.Count - 1 ? "":",");
            }
            return result.Substring(0,result.Length - 1) + "}";
        }
    }
    public class Branch {
        public Sentence judgesentence;
        public List<Sentence> sentences;
        public Branch(Sentence judgesentence,List<Sentence> sentences) {
            this.judgesentence = judgesentence;
            this.sentences = sentences;
        }
        public override string ToString() {
            string result = "{\"判断语句\":" + (judgesentence==null?"null":judgesentence.ToString()) + ",\n\"表达式\":";
            foreach (Sentence sen in sentences) {
                result += sen.ToString() + ",";
            }
            return result.Substring(0,result.Length - 1) + "}";
        }
    }
}
