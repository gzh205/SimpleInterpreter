using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CompilerDemo.CodeCheck {
    public class CheckCore {
        private static CheckCore inst;
        public UI ui;
        public List<string> systemapi;
        public static CheckCore instance() {
            if (inst == null) {
                inst = new CheckCore();
            }
            return inst;
        }
        private CheckCore() {
            this.ui = UI.instance;
            Type[] types = Assembly.GetEntryAssembly().GetTypes();
            systemapi = new List<string>();
            foreach (Type t in types) {
                if (t.FullName.Contains("CompilerDemo.Core.Library")) {
                    systemapi.Add(t.Name);
                }
            }
        }
        public void beginCheck(string code) {
            Thread t = new Thread(threadproc);
            t.Start(code);
        }
        private void threadproc(object code) {
            int[] re = ui.getSelection();
            o_o(code as string);
            ui.setSelection(re);
        }
        private void o_o(string code) {
            string[] codearr = code.Split('\n');
            int startindex = 0;
            for (int k = 0;k < codearr.Length;k++) {
                int strCount = -1;
                int braceCount = 0;
                ColorSet.rownum = k;
                for (int i = 0;i < codearr[k].Length;i++) {
                    switch (codearr[k][i]) {
                        case '(': {
                                braceCount++;
                                break;
                            }
                        case '"': {
                                if (i > 0 && codearr[k][i - 1] != '\\') {
                                    if (strCount!=-1) {
                                        ui.color_code(startindex + strCount,i - strCount + 1,Color.Green);
                                        strCount = -1;
                                    } else {
                                        strCount = i;
                                    }
                                }
                                break;
                            }
                        case ')': {
                                braceCount--;
                                if (braceCount < 0) {
                                    ui.color_code(startindex + i,1,Color.Red);
                                }
                                break;
                            }
                        default: {
                                bool iskeyword = false;
                                if(codearr[k][i]>='a'&& codearr[k][i] <= 'z') {
                                    switch (codearr[k][i]) {
                                        case 'w': {
                                                if (i + 5 < codearr[k].Length && codearr[k].Substring(i,5) == "while") {
                                                    ui.color_code(startindex + i,5,Color.Blue);
                                                    i += 5;
                                                    iskeyword = true;
                                                }
                                                break;
                                            }
                                        case 'f': {
                                                if (i + 8 < codearr[k].Length && codearr[k].Substring(i,8) == "function") {
                                                    ui.color_code(startindex + i,8,Color.Blue);
                                                    i += 8;
                                                    iskeyword = true;
                                                }
                                                break;
                                            }
                                        case 'i': {
                                                if (i + 2 < codearr[k].Length && codearr[k].Substring(i,2) == "if") {
                                                    ui.color_code(startindex + i,2,Color.Blue);
                                                    i += 2;
                                                    iskeyword = true;
                                                }
                                                break;
                                            }
                                        case 'e': {
                                                if (i + 6 < codearr[k].Length && codearr[k].Substring(i,6) == "elseif") {
                                                    ui.color_code(startindex + i,6,Color.Blue);
                                                    i += 2;
                                                    iskeyword = true;
                                                } else if (i + 4 < codearr[k].Length && codearr[k].Substring(i,4) == "else") {
                                                    ui.color_code(startindex + i,4,Color.Blue);
                                                    i += 4;
                                                    iskeyword = true;
                                                }
                                                break;
                                            }
                                    }
                                }
                                if (!iskeyword && (codearr[k][i] >= 'a' && codearr[k][i] <= 'z' || codearr[k][i]>='A'&& codearr[k][i]<='Z' || codearr[k][i]=='_')) {
                                    int j = i + 1;
                                    for (;j < codearr[k].Length;j++) {
                                        if ((codearr[k][j] < 'a' || codearr[k][j] > 'z') && (codearr[k][j] < 'A' || codearr[k][j] > 'Z' && (codearr[k][j] < '0' || codearr[k][j] > '9') && codearr[k][j] != '_'))
                                            break;
                                    }
                                    int l = 0;
                                    bool seted = false;
                                    for (;l < systemapi.Count;l++) {
                                        if (systemapi[l] == codearr[k].Substring(i,j - i)) {
                                            ui.color_code(startindex + i,j - i,Color.Pink);
                                            seted = true;
                                            break;
                                        }
                                    }
                                    if (!seted) {
                                        ui.color_code(startindex + i,j - i,Color.Orange);
                                    }
                                    i = j - 1;
                                }
                                break;
                            }
                    }    
                }
                if (braceCount > 0) {
                    ui.color_code(startindex + codearr[k].Length - 1,1,Color.Red);
                }
                startindex += codearr[k].Length + 1;
            }
        }
    }
}