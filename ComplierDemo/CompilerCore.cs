using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using CompilerDemo.Core;

namespace CompilerDemo {
    public struct Cursor {
        public int begin;
        public int end;
        public int index;
        public int linebegin;
        public int lineend;
        public int lineindex;
        public Cursor(int begin,int end,int index,int linebegin = 0,int lineend = 0,int lineindex = 0) {
            this.begin = begin;
            this.end = end;
            this.index = index;
            this.linebegin = linebegin;
            this.lineend = lineend;
            this.lineindex = lineindex;
        }
    }
    public class CompilerCore {
        public static string ReadFromFile(string filename) {
            StreamReader sr = new StreamReader(new FileStream(filename,FileMode.Open));
            string res = sr.ReadToEnd();
            sr.Close();
            return res;
        }
        public const string regexPattern = "[a-z,A-Z,0-9,_]+\\([^()]*(((?'Open'\\()[^()]*)+((?'-Open'\\))[^()]*)+)*(?(Open)(?!))\\)|(?<!\\\\)\".* (? < !\\\\)\"|[a-z,A-Z]+\\[.*\\]|[a-z,A-Z]+[(][^)]*[)]|[>][=]|[<][=]|[<](?!=)|[>](?!=)|[a-z,A-Z]+|[0-9]+|[!][=]|[=]+|[+]|[-]|[*]|[/]|[%]|[;]|[>][=]|[{]|[}]|[(]|[)]|[:]";
        public static string[][] symbol = {
            new string[]{"(",")","[","]"},
            new string[]{"^"},
            new string[]{"*","/","%"},
            new string[]{"+","-"},
            new string[]{">","<",">=","<="},
            new string[]{"==","!="},
            new string[]{"&","|","!"},
            new string[]{"="}
        };//运算符优先级表
        public static string[] keyword = {
            "if","elseif","else","while","for","function","return"
        };//关键字列表
        public static bool IsKeyword(string str) {
            foreach (string s in keyword)
                if (s == str)
                    return true;
            return false;
        }
        /// <summary>
        /// 比较op1和op2两个运算符谁的优先级更高
        /// </summary>
        /// <param name="op1">第一个运算符</param>
        /// <param name="op2">第二个运算符</param>
        /// <returns>按优先级返回不同的结果
        /// 若op1>op2,则返回1
        /// 若op1<op2,则返回-1
        /// 若op1=op2,则返回0
        /// </returns>
        public static int isOpAbove(string op1,string op2) {
            int index1, index2;
            index1 = index2 = -1;
            for (int i = 0;i < symbol.Length;i++) {
                foreach (string t in symbol[i]) {
                    if (op1 == t)
                        index1 = i;
                    if (op2 == t) {
                        index2 = i;
                    }
                }
                if (index1 != -1 && index2 != -1) break;
            }
            return (index1 == index2) ? 0 : ((index1 < index2) ? 1 : -1);
        }
        public static CompilerCore getInstance() {
            if (inst == null)
                inst = new CompilerCore();
            return inst;
        }
        private static CompilerCore inst;
        private CompilerCore() {
            for (int i = 0;i < symbol.Length;i++) {
                for (int j = 0;j < symbol[i].Length;j++) {
                    operator_priority.Add(symbol[i][j],symbol.Length - i);
                }
            }
        }
        public static Dictionary<string,Function> functionlist = new Dictionary<string,Function>();//函数列表
        public static Dictionary<string,Function> systemaip = new Dictionary<string,Function>();
        public static List<Sentence> tokens = new List<Sentence>();//语句序列
        public static Dictionary<string,int> operator_priority = new Dictionary<string,int>();
        public static Dictionary<int,int> jmp_table = new Dictionary<int,int>();
        public static Dictionary<string,Data> variables = new Dictionary<string,Data>();
        /// <summary>
        /// 首先进行词法分析，生成tokens序列。
        /// </summary>
        /// <param name="code">含有代码的字符串</param>
        public void Lexer(string code) {
            string[] tmpcode = code.Split('\n');//将代码按照换行符分割成若干行
            for (int i = 0;i < tmpcode.Length;i++) {
                if (tmpcode[i] == null || tmpcode[i].Length == 0)
                    continue;
                int j = 0;
                for (;tmpcode[i][j] == '\t';j++) {
                }
                List<string> tmptok = AdcancedSplit(tmpcode[i].Trim('\t','\r'));
                //整理代码,中缀运算符改后缀
                List<Element> restok = new List<Element>();
                Stack<string> elestack = new Stack<string>();
                int k;
                if (IsKeyword(tmptok[0])) {//第一个字符串是关键字
                    k = 1;
                    restok.Add(new Element(tmptok[0],Element.Keyword));
                } else k = 0;
                for (;k < tmptok.Count;k++) {
                    Element tmpEle;
                    if (tmptok[k].Contains("(") && tmptok[k].Contains(")")) {//是函数
                        tmpEle = new FunctionElement(tmptok[k],Element.Function);
                        restok.Add(tmpEle);
                    } else if (tmptok[k].Contains("[") && tmptok[k].Contains("]")) {//是数组
                        tmpEle = new Element(tmptok[k],Element.Array);
                        restok.Add(tmpEle);
                    } else if (tmptok[k][0] >= '0' && tmptok[k][0] <= '9') {//是数字
                        tmpEle = new Element(tmptok[k],Element.Number);
                        restok.Add(tmpEle);
                    } else if (tmptok[k][0] >= 'a' && tmptok[k][0] <= 'z' || tmptok[k][0] >= 'A' && tmptok[k][0] <= 'Z') {//是一个变量
                        tmpEle = new Element(tmptok[k],Element.Variable);
                        restok.Add(tmpEle);
                    } else if (tmptok[k][0] == '"') {//是字符串
                        tmpEle = new Element(tmptok[k],Element.String);
                        restok.Add(tmpEle);
                    } else {//是符号
                        if (elestack.Count == 0 || tmptok[k] == "(") {
                            elestack.Push(tmptok[k]);
                        } else {
                            if (tmptok[k] == ")") {
                                while (elestack.Peek() != "(" && elestack.Count > 0) {
                                    restok.Add(new Element(elestack.Pop(),Element.Symbol));
                                }
                                if (elestack.Count == 0) {
                                    throw new SyntaxErrorException("syntax error,the number of '(' and ')' don't match!");
                                }
                                elestack.Pop();
                            } else {
                                if (operator_priority[tmptok[k]] >= operator_priority[elestack.Peek()]) {
                                    elestack.Push(tmptok[k]);
                                } else {
                                    if (elestack.Peek() == "(") {
                                        elestack.Push(tmptok[k]);
                                    } else {
                                        while (operator_priority[tmptok[k]] < operator_priority[elestack.Peek()] && elestack.Count > 0) {
                                            if (elestack.Peek() == "(") {
                                                elestack.Pop();
                                                break;
                                            }
                                            restok.Add(new Element(elestack.Pop(),Element.Symbol));
                                        }
                                        elestack.Push(tmptok[k]);
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
                //将整理后的代码生成一个Sentence对象并存入tokens序列中
                tokens.Add(new Sentence(restok,j));
            }
        }
        /// <summary>
        /// 将tokens按照函数的形式整理
        /// </summary>
        public void arrange() {
            int depth = -1;
            Function tmp = null;
            int i = 0;
            for (;i < tokens.Count;i++) {
                if (tokens[i].deepth < depth) {//根据制表符\t判断该条语句是否已经离开函数定义
                    try {
                        functionlist.Add(tmp.functionname,tmp);
                        depth = tokens[i].deepth;
                        i--;
                        continue;
                    } catch (ArgumentNullException) {
                        Console.WriteLine("arrange函数出现bug,tokens[" + i + "].deepth=" + tokens[i].deepth + ";deepth=" + depth);
                    }
                } else {
                    if (tokens[i].toks[0].value == "function") {
                        tmp = new Function();
                        depth = tokens[i].deepth + 1;
                        tmp.functionname = tokens[i].toks[1].value;
                        FunctionElement tmpele = tokens[i].toks[1] as FunctionElement;
                        if (tmpele == null)
                            throw new SyntaxErrorException("语法错误，函数名的后面应该有括号");
                        List<string> paraname = new List<string>();
                        foreach (List<Element> ele in tmpele.paras) {
                            if (ele != null && ele.Count > 0 && ele[0] != null)
                                paraname.Add(ele[0].value);
                        }
                        tmp.parametername = paraname;
                        int j = i + 1;
                        for (;(j < tokens.Count) && (tokens[j].deepth >= depth);j++) {
                        }
                        tmp.sentences = CalcSentences(tokens.GetRange(i + 1,j - i - 1));
                        i = j - 1;
                    }
                }
            }
            //循环结束如果存在函数，则执行CalcSentences
            try {
                functionlist.Add(tmp.functionname,tmp);
            } catch (ArgumentNullException) {
                Console.WriteLine("arrange函数出现bug,tokens[" + i + "].deepth=" + tokens[i].deepth + ";deepth=" + depth);
            }
        }
        /// <summary>
        /// 递归将所有语句以树形结构组织
        /// </summary>
        /// <param name="sens">传入一个Sentence数组</param>
        /// <returns></returns>
        public List<Sentence> CalcSentences(List<Sentence> sens) {
            List<Sentence> result = new List<Sentence>();
            IfElse ifElse = null;
            for (int i = 0;i < sens.Count;i++) {
                switch (sens[i].toks[0].value) {
                    case "while": {
                            int whiledep = sens[i].deepth;
                            While @while = new While(null,whiledep);
                            @while.judgesentence = new Sentence(sens[i].toks.GetRange(1,sens[i].toks.Count - 1),whiledep);
                            int j = i + 1;
                            for (;j < sens.Count && sens[j].deepth > whiledep;j++) {
                            }
                            @while.sentences = CalcSentences(sens.GetRange(i + 1,j - i - 1));
                            result.Add(@while);
                            i = j - 1;
                            break;
                        }
                    case "if": {
                            int ifdeepth = sens[i].deepth;
                            int j = i + 1;
                            for (;sens[j].deepth > ifdeepth;j++) {
                            }
                            ifElse = new IfElse(null,sens[i].deepth);
                            ifElse.branchs.Add(new Branch(new Sentence(sens[i].toks.GetRange(1,sens[i].toks.Count - 1),ifdeepth),CalcSentences(sens.GetRange(i + 1,j - i - 1))));
                            i = j - 1;
                            break;
                        }
                    case "elseif": {
                            int elifdep = sens[i].deepth;
                            int j = i + 1;
                            for (;sens[j].deepth > elifdep;j++) { }
                            if (ifElse == null) throw new SyntaxErrorException("syntax error:expected 'if'");
                            else {
                                ifElse.branchs.Add(new Branch(new Sentence(sens[i].toks.GetRange(1,sens[i].toks.Count - 1),elifdep),CalcSentences(sens.GetRange(i + 1,j - i - 1))));
                            }
                            i = j - 1;
                            break;
                        }
                    case "else": {
                            int elifdep = sens[i].deepth;
                            int j = i + 1;
                            for (;sens[j].deepth > elifdep;j++) { }
                            if (ifElse == null) throw new SyntaxErrorException("syntax error:expected 'if'");
                            else {
                                ifElse.branchs.Add(new Branch(null,CalcSentences(sens.GetRange(i + 1,j - i - 1))));
                            }
                            result.Add(ifElse);
                            i = j - 1;
                            break;
                        }
                    default: {
                            result.Add(sens[i]);
                            break;
                        }
                }
            }
            return result;
        }
        /// <summary>
        /// 加载系统函数(就是CompilerDemo.Core.Library里的几个函数)
        /// 注:利用反射实现自动加载CompilerDemo.Core.Library命名空间下的所有函数
        /// 不能在CompilerDemo.Core.Library下添加非系统函数的类
        /// </summary>
        public void loadSystemApi() {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type t in types) {
                if (t.FullName.Contains("CompilerDemo.Core.Library")) {
                    systemaip.Add(t.Name,Activator.CreateInstance(t) as Function);
                }
            }
        }
        /// <summary>
        /// 分割字符串，用于进行词法分析
        /// </summary>
        /// <param name="code">一行代码</param>
        /// <returns>返回被分割的字符串数组</returns>
        public static List<string> AdcancedSplit(string code) {
            List<string> result = new List<string>();
            int splitpoint = 0, i = 0;
            for (;i < code.Length;i++) {
                switch (code[i]) {
                    case ' ': {
                            if (i != splitpoint) {
                                result.Add(code.Substring(splitpoint,i - splitpoint));
                            }
                            splitpoint = i + 1;
                            break;
                        }
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '%':
                    case '^':
                    case '&':
                    case '|':
                    case ')': {//常规符号正常存入result中
                            if(i!=splitpoint)
                                result.Add(code.Substring(splitpoint,i - splitpoint));
                            result.Add("" + code[i]);
                            splitpoint = i + 1;
                            break;
                        }
                    case '>':
                    case '<':
                    case '=': {//先判断大于或小于号后面是否有等号,再存储
                            result.Add(code.Substring(splitpoint,i - splitpoint));
                            if (i < code.Length && code[i + 1] == '=') {
                                result.Add(code.Substring(i,2));
                                i++;
                            } else {
                                result.Add("" + code[i]);
                            }
                            splitpoint = i + 1;
                            break;
                        }
                    case '(': {//判断是函数调用的括号,还是提升运算优先级的括号
                            if (code[splitpoint] >= '0' && code[splitpoint] <= '9' || code[splitpoint] >= 'a' && code[splitpoint] <= 'z'
                                || code[splitpoint] >= 'A' && code[splitpoint] <= 'Z' || code[splitpoint] == '_') {
                                int j = i + 1;
                                int parenthesis = 1;
                                for (;j < code.Length;j++) {
                                    if (code[j] == '(') {
                                        parenthesis++;
                                    } else if (code[j] == ')') {
                                        parenthesis--;
                                    }
                                    if (parenthesis == 0) {
                                        break;
                                    }
                                }
                                result.Add(code.Substring(splitpoint,j - splitpoint + 1));
                                i = j < code.Length - 1 ? j + 1 : code.Length - 1;
                                splitpoint = i + 1;
                            } else {
                                result.Add(code.Substring(splitpoint,i - splitpoint));
                                result.Add("" + code[i]);
                                splitpoint = i + 1;
                            }
                            break;
                        }
                    case '"': {
                            int j = i + 1;
                            for (;j < code.Length && !(code[j] == '"' && code[j - 1] != '\\');j++) {
                            }
                            result.Add(code.Substring(splitpoint,j - splitpoint + 1));
                            i = j < code.Length - 1 ? j : code.Length - 1;
                            splitpoint = i + 1;
                            break;
                        }
                }
            }
            if (splitpoint <= code.Length - 1) {
                result.Add(code.Substring(splitpoint,i - splitpoint));
            }
            return result;
        }
    }
}
