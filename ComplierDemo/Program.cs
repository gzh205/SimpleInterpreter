using ComplierDemo;
using CompilerDemo.export;
using System;
using System.IO;
using System.Windows.Forms;

namespace CompilerDemo {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args) {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UI());
        }
    }
}
