using CompilerDemo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompilerDemo.Core {
    [Serializable]
    public class FunctionElement : Element {
        public List<List<Element>> paras;
        public FunctionElement(string value,int flag) : base(value,flag) {
            FunctionStruct fun = splitFunction(value);
            List<List<Element>> elements = new List<List<Element>>();
            foreach (string s in fun.paras) {
                elements.Add(construct(s));
            }
            this.paras = elements;
            this.value = fun.funname;
        }
        private FunctionStruct splitFunction(string code) {
            int splitpoint = code.IndexOf('(');
            FunctionStruct res = new FunctionStruct();
            res.funname = code.Substring(0,splitpoint);
            res.paras = new List<string>();
            string data = code.Substring(splitpoint + 1,code.Length - splitpoint - 2);
            int parenthesis = 0, previndex = 0, i = 0;
            for (;i < data.Length;i++) {
                if (data[i] == '(') {
                    parenthesis++;
                } else if (data[i] == ')') {
                    parenthesis--;
                }
                if (data[i] == ',' && parenthesis == 0) {
                    res.paras.Add(data.Substring(previndex,i - previndex));
                    previndex = i + 1;
                } else if (data[i] == '"') {
                    int j = i + 1;
                    for (;j < data.Length && !(data[j] == '"' && data[j - 1] != '\\');j++) {
                    }
                    i = (j >= data.Length - 1) ? data.Length -1: j + 1;
                    
                }
                if (parenthesis < 0) {
                    throw new SyntaxErrorException("括号不匹配");
                }
            }
            res.paras.Add(data.Substring(previndex,i - previndex));
            return res;
        }
        private List<Element> construct(string code) {
            List<string> toks = CompilerCore.AdcancedSplit(code.Trim(' ','\t','\r'));
            //整理代码,中缀运算符改后缀
            List<Element> restok = new List<Element>();
            Stack<string> elestack = new Stack<string>();
            foreach (string data in toks) {
                if (data.Length == 0)
                    continue;
                Element tmpEle;
                if (data.Contains("(") && data.Contains(")")) {//是函数
                    tmpEle = new FunctionElement(data,Element.Function);
                    restok.Add(tmpEle);
                } else if (data.Contains("[") && data.Contains("]")) {//是数组
                    tmpEle = new Element(data,Element.Array);
                    restok.Add(tmpEle);
                } else if (data[0] >= '0' && data[0] <= '9') {//是数字
                    tmpEle = new Element(data,Element.Number);
                    restok.Add(tmpEle);
                } else if (data[0] >= 'a' && data[0] <= 'z' || data[0] >= 'A' && data[0] <= 'Z') {//是一个变量
                    tmpEle = new Element(data,Element.Variable);
                    restok.Add(tmpEle);
                } else if (data[0] == '"') {//是字符串
                    tmpEle = new Element(data,Element.String);
                    restok.Add(tmpEle);
                } else {//是符号
                    if (elestack.Count == 0 || data == "(") {
                        elestack.Push(data);
                    } else {
                        if (data == ")") {
                            while (elestack.Peek() != "(" && elestack.Count > 0) {
                                restok.Add(new Element(elestack.Pop(),Element.Symbol));
                            }
                            if (elestack.Count == 0) {
                                throw new SyntaxErrorException("syntax error,the number of '(' and ')' don't match!");
                            }
                            elestack.Pop();
                        } else {
                            if (CompilerCore.operator_priority[data] >= CompilerCore.operator_priority[elestack.Peek()]) {
                                elestack.Push(data);
                            } else {
                                if (elestack.Peek() == "(") {
                                    elestack.Push(data);
                                } else {
                                    while (CompilerCore.operator_priority[data] < CompilerCore.operator_priority[elestack.Peek()] && elestack.Count > 0) {
                                        if (elestack.Peek() == "(") {
                                            elestack.Pop();
                                            break;
                                        }
                                        restok.Add(new Element(elestack.Pop(),Element.Symbol));
                                    }
                                    elestack.Push(data);
                                }
                            }
                        }
                    }
                }
            }
            //将栈中的剩余运算符按序插入到表达式后面
            while (elestack.Count > 0) {
                if (elestack.Peek() == "(") {
                    elestack.Pop();
                    continue;
                }
                restok.Add(new Element(elestack.Pop(),Element.Symbol));
            }
            return restok;
        }
    }
    struct FunctionStruct {
        public string funname;
        public List<string> paras;
    }
}
