using System;
using System.Collections.Generic;
using System.Text;

namespace CompilerDemo.Core {
    /// <summary>
    /// 脚本语言中的一个函数
    /// </summary>
    [Serializable]
    public class Function {
        public string functionname;
        public List<Sentence> sentences = new List<Sentence>();
        public List<string> parametername = new List<string>();
        public int index;
        public Dictionary<string,Data> variables = new Dictionary<string,Data>();
        public List<Data> parametervalue = new List<Data>();
        private Stack<Sentence> recursion = new Stack<Sentence>();
        /// <summary>
        /// 执行一个函数
        /// </summary>
        public virtual Data run() {
            //处理参数
            for (int i = 0;i < parametername.Count;i++) {
                Data para = null;
                try {
                    para = parametervalue[i];
                } catch (IndexOutOfRangeException) {
                    para = new Data(0);
                }
                variables[parametername[i]] = para;
            }
            //执行函数
            Data returnval = null;
            foreach (Sentence sentence in sentences) {
                Data res = getSentenceRes(sentence);
                if (sentence.toks != null && sentence.toks.Count > 0 && sentence.toks[0] != null && sentence.toks[0].value == "return") {
                    returnval = res;
                }
            }
            return returnval;
        }
        private Data getSentenceRes(Sentence sentence) {
            Data res = new Data(0);
            if (sentence is While) {
                While w = sentence as While;
                while (getSentenceRes(w.judgesentence.toks).num == 1) {
                    foreach (Sentence s in w.sentences) {
                        getSentenceRes(s);
                    }
                }
            } else if (sentence is IfElse) {
                IfElse w = sentence as IfElse;
                foreach (Branch b in w.branchs) {
                    if (b.judgesentence == null || getSentenceRes(b.judgesentence.toks).num == 1) {
                        foreach (Sentence s in b.sentences) {
                            getSentenceRes(s.toks);
                        }
                        break;
                    }
                }
            } else if (sentence is Sentence) {
                res = getSentenceRes(sentence.toks);
            } else {
                throw new SyntaxErrorException("程序编译时出错");
            }
            return res;
        }
        private Data getSentenceRes(List<Element> toks) {
            Data result = new Data(0);
            Stack<Data> datastack = new Stack<Data>();
            foreach (Element e in toks) {
                switch (e.flag) {
                    case Element.Function: {
                            FunctionElement ele = e as FunctionElement;
                            Function fun = null;
                            try {
                                fun = CompilerCore.functionlist[e.value];
                            } catch (KeyNotFoundException) {
                                try {
                                    fun = CompilerCore.systemaip[e.value];
                                } catch (KeyNotFoundException) {
                                    throw new SyntaxErrorException("语法错误,不存在的函数");
                                }
                            } finally {
                                Data r = null;
                                if (fun != null) {
                                    List<Data> paras = new List<Data>();
                                    for (int i = 0;i < fun.parametername.Count;i++) {
                                        Data val = getSentenceRes(ele.paras[i]);
                                        paras.Add(val);
                                    }
                                    fun.parametervalue = paras;
                                    r = fun.run();
                                }
                                if (r != null && !r.isEmpty()) {
                                    datastack.Push(r);
                                }
                            }
                            break;
                        }
                    case Element.Symbol: {
                            Data dat1 = null;
                            Data dat2 = datastack.Pop();
                            Data res = null;
                            if (e.value != "!") {
                                dat1 = datastack.Pop();
                            }
                            switch (e.value) {
                                case "^": {
                                        //乘方运算，a^b即为a的b次方
                                        if (dat1.isStr || dat2.isStr) {
                                            throw new SyntaxErrorException("错误:只能进行两数运算");
                                        }
                                        res = new Data(Math.Pow(dat1.num,dat2.num));
                                        break;
                                    }
                                case "*": {
                                        if (dat1.isStr || dat2.isStr) {
                                            throw new SyntaxErrorException("错误:只能进行两数运算");
                                        }
                                        res = new Data(dat1.num * dat2.num);
                                        break;
                                    }
                                case "/": {
                                        if (dat1.isStr || dat2.isStr) {
                                            throw new SyntaxErrorException("错误:只能进行两数运算");
                                        }
                                        res = new Data(dat1.num / dat2.num);
                                        break;
                                    }
                                case "%": {
                                        if (dat1.isStr || dat2.isStr) {
                                            throw new SyntaxErrorException("错误:只能进行两数运算");
                                        }
                                        res = new Data(dat1.num % dat2.num);
                                        break;
                                    }
                                case "+": {
                                        if (dat1.isStr && dat2.isStr) {
                                            res = new Data(dat1.str + dat2.str);
                                        } else if (dat1.isStr && !dat2.isStr) {
                                            res = new Data(dat1.str + dat2.num);
                                        } else if (!dat1.isStr && dat2.isStr) {
                                            res = new Data(dat1.num + dat2.str);
                                        } else {
                                            res = new Data(dat1.num + dat2.num);
                                        }
                                        break;
                                    }
                                /* 减法运算
                                * 1.数字(a)-数字(b)=数字(a-b)
                                * 2.字符串(a)-数字(n)=去掉末尾n位后的字符串a
                                * 3.数字(n)-字符串(a)=去掉开头n位后的字符串a
                                * 4.字符串(a)-字符串(b)=去掉字符串a中的出现的字符串b
                                */
                                case "-": {
                                        if (dat1.isStr && dat2.isStr) {
                                            res = new Data(dat1.str.Replace(dat2.str,""));
                                        } else if (!dat1.isStr && !dat2.isStr) {
                                            res = new Data(dat1.num - dat2.num);
                                        } else if (!dat1.isStr && dat2.isStr) {
                                            res = new Data(dat2.str.Substring(0,(int)dat1.num));
                                        } else {
                                            res = new Data(dat1.str.Substring(dat1.str.Length - (int)dat2.num));
                                        }
                                        break;
                                    }
                                /* 大于和小于运算
                                 * 数字a>b为真，则运算符的值为1，反之为0
                                 * 字符串则是循环比较每一个字符，对字符比较谁的编码值更大
                                 */
                                case ">": {
                                        if (dat1.isStr && dat2.isStr) {
                                            res = Data.comparestr(dat1.str,dat2.str,1);
                                        } else if (!dat1.isStr && !dat2.isStr) {
                                            if (dat1.num > dat2.num) {
                                                res = new Data(1);
                                            } else {
                                                res = new Data(0);
                                            }
                                        } else {
                                            throw new SyntaxErrorException("语法错误");
                                        }
                                        break;
                                    }
                                case "<": {
                                        if (dat1.isStr && dat2.isStr) {
                                            res = Data.comparestr(dat1.str,dat2.str,2);
                                        } else if (!dat1.isStr && !dat2.isStr) {
                                            if (dat1.num < dat2.num) {
                                                res = new Data(1);
                                            } else {
                                                res = new Data(0);
                                            }
                                        } else {
                                            throw new SyntaxErrorException("语法错误,该运算符不支持字符串和数字直接运算");
                                        }
                                        break;
                                    }
                                case ">=": {
                                        if (dat1.isStr && dat2.isStr) {
                                            res = Data.comparestr(dat1.str,dat2.str,3);
                                        } else if (!dat1.isStr && !dat2.isStr) {
                                            if (dat1.num >= dat2.num) {
                                                res = new Data(1);
                                            } else {
                                                res = new Data(0);
                                            }
                                        } else {
                                            throw new SyntaxErrorException("语法错误");
                                        }
                                        break;
                                    }
                                case "<=": {
                                        if (dat1.isStr && dat2.isStr) {
                                            res = Data.comparestr(dat1.str,dat2.str,4);
                                        } else if (!dat1.isStr && !dat2.isStr) {
                                            if (dat1.num <= dat2.num) {
                                                res = new Data(1);
                                            } else {
                                                res = new Data(0);
                                            }
                                        } else {
                                            throw new SyntaxErrorException("语法错误");
                                        }
                                        break;
                                    }
                                case "&": {
                                        if (!dat1.isStr && !dat2.isStr) {
                                            if (dat1.num != 0 && dat2.num != 0) {
                                                res = new Data(1);
                                            } else {
                                                res = new Data(0);
                                            }
                                        } else {
                                            throw new SyntaxErrorException("语法错误");
                                        }
                                        break;
                                    }
                                case "|": {
                                        if (!dat1.isStr && !dat2.isStr) {
                                            if (dat1.num == 0 && dat2.num == 0) {
                                                res = new Data(0);
                                            } else {
                                                res = new Data(1);
                                            }
                                        } else {
                                            throw new SyntaxErrorException("语法错误");
                                        }
                                        break;
                                    }
                                case "!": {
                                        if (!dat2.isStr) {
                                            if (dat2.num == 0)
                                                res = new Data(1);
                                            else
                                                res = new Data(0);
                                        } else {
                                            throw new SyntaxErrorException("语法错误");
                                        }
                                        break;
                                    }
                                case "==": {
                                        if (dat1.isStr && dat2.isStr) {
                                            if (dat1.str == dat2.str)
                                                res = new Data(1);
                                            else
                                                res = new Data(0);
                                        } else if (!dat1.isStr && !dat2.isStr) {
                                            if (dat1.num == dat2.num)
                                                res = new Data(1);
                                            else
                                                res = new Data(0);
                                        } else if (!dat1.isStr && dat2.isStr) {
                                            if (dat1.num == Convert.ToDouble(dat2.str))
                                                res = new Data(1);
                                            else
                                                res = new Data(0);
                                        } else {
                                            if (dat2.num == Convert.ToDouble(dat1.str))
                                                res = new Data(1);
                                            else
                                                res = new Data(0);
                                        }
                                        break;
                                    }
                                case "!=": {
                                        if (dat1.isStr && dat2.isStr) {
                                            if (dat1.str != dat2.str)
                                                res = new Data(1);
                                            else
                                                res = new Data(0);
                                        } else if (!dat1.isStr && !dat2.isStr) {
                                            if (dat1.num != dat2.num)
                                                res = new Data(1);
                                            else
                                                res = new Data(0);
                                        } else if (!dat1.isStr && dat2.isStr) {
                                            if (dat1.num != Convert.ToDouble(dat2.str))
                                                res = new Data(1);
                                            else
                                                res = new Data(0);
                                        } else {
                                            if (dat2.num != Convert.ToDouble(dat1.str))
                                                res = new Data(1);
                                            else
                                                res = new Data(0);
                                        }
                                        break;
                                    }
                                case "=": {
                                        dat2.variablename = dat1.variablename;
                                        this.variables[dat1.variablename] = dat2;
                                        res = dat2;
                                        break;
                                    }
                                default: {
                                        throw new SyntaxErrorException("无法识别的符号");
                                    }
                            }
                            datastack.Push(res);
                            break;
                        }
                    case Element.Variable: {
                            Data dat = null;
                            try {
                                dat = variables[e.value];
                            } catch (KeyNotFoundException) {
                                dat = new Data(0);
                                dat.variablename = e.value;
                                variables.Add(e.value,dat);
                            } finally {
                                datastack.Push(dat);
                            }
                            break;
                        }
                    case Element.Number: {
                            datastack.Push(new Data(Convert.ToDouble(e.value)));
                            break;
                        }
                    case Element.String: {
                            datastack.Push(new Data(EscapeChar(e.value.Trim('\"'))));
                            break;
                        }
                    default: {
                            break;
                        }
                }
            }
            if (datastack.Count > 0) {
                result = datastack.Pop();
            }
            return result;
        }
        public static string EscapeChar(string str) {
            StringBuilder sb = new StringBuilder();
            for (int i = 0;i < str.Length;i++) {
                if (str[i] == '\\') {
                    switch (str[++i]) {
                        case 'n': {
                                sb.Append("\r\n");
                                break;
                            }
                        case 't': {
                                sb.Append('\t');
                                break;
                            }
                        case '\\': {
                                sb.Append('\\');
                                break;
                            }
                        case 'r': {
                                sb.Append('\r');
                                break;
                            }
                    }
                } else {
                    sb.Append(str[i]);
                }
            }
            return sb.ToString();
        }
        public override string ToString() {
            string result = "{\"函数名\":\"" + functionname + "\",\n\"语句列表\":[";
            for (int i = 0;i < sentences.Count;i++) {
                Sentence s = sentences[i];
                if (s is IfElse) {
                    IfElse ifelse = s as IfElse;
                    result += ifelse.ToString() + (i == sentences.Count - 1 ? "" : ",");
                } else if (s is While) {
                    While @while = s as While;
                    result += @while.ToString() + (i == sentences.Count - 1 ? "" : ",");
                } else {
                    result += s.ToString() + (i == sentences.Count - 1 ? "" : ",");
                }
            }
            result += "],\n\"参数列表\":[";
            for (int i = 0;i < parametername.Count;i++) {
                result += "\"" + parametername[i].ToString() + "\"" + (i == parametername.Count - 1 ? "" : ",");
            }
            result += "],\n\"深度\":" + index + "}";
            return result;
        }
    }
}

/*
 * public virtual Data run() {
            //处理参数
            for(int i = 0;i < parametername.Count;i++) {
                Data para = null;
                try {
                    para = parametervalue[i];
                } catch (IndexOutOfRangeException) {
                    para = new Data(0);
                }
                variables.Add(parametername[i],para);
            }
            //执行函数
            Data returnval = null;
            foreach (Sentence sentence in this.sentences) {
                getSentenceRes(sentence);
            }
            return returnval;
        }
        private Data getSentenceRes(Sentence sentence) {
            Data result = new Data(0);
            if (sentence is While) {
                recursion.Push(sentence);
            } else if (sentence is IfElse) {
                recursion.Push(sentence);
            } else if (sentence is Sentence) {
                Sentence s = sentence;
                Data res = getSentenceRes(s.toks);
                if (sentence.toks[0].value == "return") {
                    result = res;
                }
            } else {
                throw new SyntaxErrorException("程序编译时出错");
            }
            return result;
        }
 */