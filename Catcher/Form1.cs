using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Catcher
{
    public partial class Form1 : Form
    {
        static public System.Collections.Generic.List<string> list = new List<string>();
        Watcher watcher;
        Timer tick;
        int lastlistrow = 0;
        
        public Form1()
        {
            InitializeComponent();
            button1.Text = "Vybrat složku";
            tick = new Timer();
            tick.Interval = 1;
            tick.Tick += Tick_Tick;
            tick.Start();
            System.IO.Directory.CreateDirectory("copyed");
        }

        private void Tick_Tick(object sender, EventArgs e)
        {
            if (lastlistrow < list.Count)
            {
                for (int i = lastlistrow +1 ; i < list.Count; i++)
                {
                    listBox1.Items.Add(list[i]);
                }
                lastlistrow = list.Count;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            watcher = new Watcher(folderBrowserDialog1.SelectedPath, listBox1);
            
        }
        public static string Now()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmssFFFFFFF");
            
        }
    }
    public class Watcher
    {
        private string folder;
        private ListBox listBox;

        public string Folder { get => folder; set => folder = value; }

        public Watcher(string folder, ListBox listBox)
        {
            this.folder = folder;
            this.listBox = listBox;
            
            System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher(folder);
            watcher.Deleted += (s,e) => Form1.list.Add(Form1.Now() + "DELETED " + e.Name);
            watcher.Changed += Watcher_Changed;
            watcher.Error += (s, e) => Form1.list.Add(Form1.Now() + "MONITORING STOPPED");
            watcher.Created += Watcher_Created;

            watcher.IncludeSubdirectories = true;
            watcher.EnableRaisingEvents = true;
        }

        private void Watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {
            try
            {
                if (e.Name.Contains("\\"))
                {
                    int notfile = e.Name.LastIndexOf('\\');
                    System.IO.Directory.CreateDirectory("copyed\\" + e.Name.Substring(0, notfile));

                }
                System.IO.File.Copy(e.FullPath, "copyed\\" + e.Name + Form1.Now());
                Form1.list.Add(Form1.Now() + "CHANGED " + e.Name);
            }
            catch
            {

            }
        }

        private void Watcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            try
            {
                if (e.Name.Contains("\\"))
                {
                    int notfile = e.Name.LastIndexOf('\\');
                        System.IO.Directory.CreateDirectory("copyed\\"+e.Name.Substring(0, notfile));
                    
                }
                System.IO.File.Copy(e.FullPath, "copyed\\" + e.Name + Form1.Now());
                Form1.list.Add(Form1.Now() + "CREATED " + e.Name);
            }
            catch
            {

            }
        }
    }
}
