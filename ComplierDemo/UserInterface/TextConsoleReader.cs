using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace CompilerDemo.UserInterface {
    public class TextConsoleReader : TextReader {
        public static int bufferedlength;
        public string readstr = "";
        private System.Windows.Forms.TextBox _textBox { set; get; }
        public Semaphore semaphore { get; private set; }
        private static TextConsoleReader inst;
        public static TextConsoleReader instance() {
            if (inst == null)
                inst = new TextConsoleReader();
            return inst;
        }
        public static TextConsoleReader instance(System.Windows.Forms.TextBox txt) {
            if (inst == null) {
                inst = new TextConsoleReader();
                inst.init(txt);
            }
            return inst;
        }
        public void init(System.Windows.Forms.TextBox txt) {
            this._textBox = txt;
            semaphore = new Semaphore(0,1);
            txt.KeyDown += new System.Windows.Forms.KeyEventHandler(keydown);
        }
        private void keydown(object sender,KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                readstr = _textBox.Text.Substring(bufferedlength,_textBox.Text.Length - bufferedlength);
                semaphore.Release();
            }
        }
        private TextConsoleReader() {
            bufferedlength = 0;
            Console.SetIn(this);
        }
        public override string ReadLine() {                     
            semaphore.WaitOne();
            return readstr;
        }
    }
}