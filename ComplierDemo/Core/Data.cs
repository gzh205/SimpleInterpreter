using System;
using System.Collections.Generic;
using System.Text;

namespace CompilerDemo.Core {
    /// <summary>
    /// 用于表示一个数字或字符串
    /// </summary>
    [Serializable]
    public class Data {
        public double num;
        public string str;
        public bool isStr { get; }
        public string variablename;
        public Data(double num) {
            this.num = num;
            isStr = false;
        }
        public Data(string str) {
            this.str = str;
            isStr = true;
        }
        public bool isEmpty() {
            return str == null && num == 0;
        }
        /// <summary>
        /// 比较两个字符串的大小
        /// </summary>
        /// <param name="str1">字符串1</param>
        /// <param name="str2">字符串2</param>
        /// <param name="pattern">模式，一共有4中
        /// 1:str1>str2?
        /// 2:str1<str2?
        /// 3:str1>=str2?
        /// 4:str1<=str2?
        /// </param>
        /// <returns></returns>
        public static Data comparestr(string str1,string str2,int pattern) {
            if (pattern == 1) {
                for (int i = 0;i < str1.Length && i < str2.Length;i++) {
                    if (str1[i] > str2[i]) {
                        return new Data(1);

                    } else if (str1[i] < str2[i]) {
                        return new Data(0);
                    }
                }
                if (str1.Length > str2.Length) {
                    return new Data(1);
                } else {
                    return new Data(0);
                }
            }else if (pattern == 2) {
                for (int i = 0;i < str1.Length && i < str2.Length;i++) {
                    if (str1[i] < str2[i]) {
                        return new Data(1);

                    } else if (str1[i] < str2[i]) {
                        return new Data(0);
                    }
                }
                if (str1.Length < str2.Length) {
                    return new Data(1);
                } else {
                    return new Data(0);
                }
            } else if (pattern == 3) {
                for (int i = 0;i < str1.Length && i < str2.Length;i++) {
                    if (str1[i] >= str2[i]) {
                        return new Data(1);

                    } else if (str1[i] < str2[i]) {
                        return new Data(0);
                    }
                }
                if (str1.Length >= str2.Length) {
                    return new Data(1);
                } else {
                    return new Data(0);
                }
            } else if (pattern == 4) {
                for (int i = 0;i < str1.Length && i < str2.Length;i++) {
                    if (str1[i] <= str2[i]) {
                        return new Data(1);

                    } else if (str1[i] < str2[i]) {
                        return new Data(0);
                    }
                }
                if (str1.Length <= str2.Length) {
                    return new Data(1);
                } else {
                    return new Data(0);
                }
            } else {
                throw new Exception("pattern错误,当前值为:"+pattern);
            }
        }
    }
}