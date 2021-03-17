using CompilerDemo.export;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CompilerDemo.UserInterface;
using System.Drawing;
using CompilerDemo.CodeCheck;
using System.Runtime.InteropServices;
using System;

namespace CompilerDemo {
    public partial class UI : Form {
        public delegate void WriteDelegate(string output);
        public delegate string ReadDelegate();
        private string filename;
        private string code;
        private CompilerCore compiler;
        public void OpenFile() {
            if (filename == null)
                throw new System.IO.FileNotFoundException("文件名变量为空值，无法打开文件");
            StreamReader sr = new StreamReader(new FileStream(filename,FileMode.OpenOrCreate));
            string code = sr.ReadToEnd();
            this.txtCode.Text = code;
            sr.Close();
        }
        public void mainthread() {
            compiler = CompilerCore.getInstance();
            compiler.Lexer(code);
            compiler.arrange();
            compiler.loadSystemApi();//加载系统函数
            CompilerCore.functionlist["main"].run();
            Console.WriteLine("程序运行结束!");
        }
        public UI() {
            InitializeComponent();
            UI.instance = this;
            this.txtConsole.AllowDrop = true;
            string[] args = System.Environment.GetCommandLineArgs();
            if (args != null && args.Length > 1) {
                filename = args[1];
                OpenFile();
                this.txtCode.Text = code;
            }
            new TextConsoleWriter(this.txtConsole);
            TextConsoleReader.instance(this.txtConsole);
        }
        public TextBox getConsole() {
            return this.txtConsole;
        }
        public void start() {
            Thread t = new Thread(mainthread,2097151);
            t.Start();
        }
        public static UI instance { get; private set; }
        private Semaphore semaphore = new Semaphore(0,1);

        private void TxtDragDrop(object sender,DragEventArgs e) {
            this.filename = (e.Data.GetData(DataFormats.FileDrop) as System.Array).GetValue(0).ToString();
            OpenFile();
            CheckCore.instance().beginCheck(txtCode.Text);
        }

        private void TxtDragEnter(object sender,DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void UI_DragDrop(object sender,DragEventArgs e) {
            this.filename = (e.Data.GetData(DataFormats.FileDrop) as System.Array).GetValue(0).ToString();
            OpenFile();
            CheckCore.instance().beginCheck(txtCode.Text);
        }

        private void UI_DragEnter(object sender,DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) {
                e.Effect = DragDropEffects.Link;
            }
        }

        private void toolStripButton1_Click(object sender,System.EventArgs e) {
            //打开文件
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "选择源程序文件";
            ofd.ShowDialog();
            filename = ofd.FileName;
            OpenFile();
        }

        private void toolStripButton2_Click(object sender,System.EventArgs e) {
            //运行程序
            code = this.txtCode.Text;
            start();
            SaveFile();
        }
        public void SaveFile() {
            StreamWriter sw = new StreamWriter(new FileStream(filename,FileMode.Create));
            sw.Write(code);
            sw.Close();
        }

        public void color_code(int begin,int length,Color color) {
            this.Invoke(new MethodInvoker(()=> {
                txtCode.Select(begin,length);
                txtCode.SelectionColor = color;
            }));
        }
        public int[] getSelection() {
            int[] res = null;
            this.Invoke(new MethodInvoker(() => {
                int start = txtCode.SelectionStart;
                int length = txtCode.SelectionLength;
                res = new int[] { start,length };
            }));
            return res;
        }
        public void setSelection(int[] arr) {
            this.Invoke(new MethodInvoker(()=> {
                txtCode.Select(arr[0],arr[1]);
            }));
        }

        private void txtCode_KeyDown(object sender,KeyEventArgs e) {
            if (e.KeyCode == Keys.Space || e.KeyCode==Keys.Enter) {
                CheckCore.instance().beginCheck(txtCode.Text);
            }
        }

        private void toolStripButton4_Click(object sender,System.EventArgs e) {
            if (compiler == null) {
                MessageBox.Show("只有在运行程序后才能使用此功能","提示");
            } else {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Title = "保存文件";
                sfd.Filter = "JSON|*.json";
                sfd.ShowDialog();
                if (sfd.FileName != null)
                    new ExportToJson(CompilerCore.functionlist).write(sfd.FileName);
            }
        }

        private void toolStripButton5_Click(object sender,EventArgs e) {
            FontDialog fd = new FontDialog();
            fd.ShowColor = false;
            fd.ShowEffects = true;
            DialogResult dr = fd.ShowDialog();
            if (dr == DialogResult.OK) {
                txtCode.Font = fd.Font;
            }
            CheckCore.instance().beginCheck(this.txtCode.Text);
        }
    }
}