using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CompilerDemo.UserInterface {
    public class TextConsoleWriter : TextWriter {
        private System.Windows.Forms.TextBox _textBox { set; get; }
        private int maxRowLenght = 0;//textBox中显示的最大行数，若不限制，则置为0
        public TextConsoleWriter(System.Windows.Forms.TextBox txt) {
            _textBox = txt;
            Console.SetOut(this);
        }
        public override void Write(string value) {
            if (_textBox.IsHandleCreated)
                _textBox.BeginInvoke(new ThreadStart(() => {
                    _textBox.AppendText(value);
                }));
            TextConsoleReader.bufferedlength += value.Length;
        }

        public override void WriteLine(string value) {
            if (_textBox.IsHandleCreated)
                _textBox.BeginInvoke(new ThreadStart(() => {
                    _textBox.AppendText(value + "\r\n");
                }));
            TextConsoleReader.bufferedlength += value.Length + 2;
        }

        public override Encoding Encoding {
            get {
                return Encoding.UTF8;
            }
        }
    }
}
