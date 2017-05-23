using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Microsoft.Win32;

namespace oneimg_wallpaper
{
    public partial class main : Form
    {
        int nowid = 0;
        int nextid = 0;
        bool one = true;
        bool startgo = false;
        string path = @"c:\";//写入目录
        int imgnum = 4000;//默认图片数量
        public const int WM_SYSCOMMAND = 0x0112;
        public const int SC_MOVE = 0xF010;
        public const int HTCAPTION = 0x0002;
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
         int uAction,
         int uParam,
         string lpvParam,
         int fuWinIni
         );
        //拖动
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        public main()
        {
            InitializeComponent();
        }


        void DeleteDirectory(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                DirectoryInfo[] childs = dir.GetDirectories();
                foreach (DirectoryInfo child in childs)
                {
                    child.Delete(true);
                }
                dir.Delete(true);
            }
        }

        public void ReadWebFile(string url, string fileName)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.Headers.Add("UserAgent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                client.DownloadFile(url, fileName);
            }
            catch
            {
                MessageBox.Show("eero! Dont find oneimg server! Please check your Internet! \n 错误!无法链接到一图服务器或无法写入文件!请检查你的网络");

            }

        }

        public void ReadImgNumber()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://t5.haotown.cn/oneimg/data/");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream recStream = response.GetResponseStream();
            Encoding utf8 = Encoding.GetEncoding("utf-8");
            StreamReader sr = new StreamReader(recStream, utf8);
            String content = sr.ReadToEnd();
            imgnum = int.Parse(content);
            System.Console.Write(imgnum);

        }

        private void yiyan()
        {
            //指定请求
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://t5.haotown.cn/hitokoto/?encode=text");
            //得到返回
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //得到流
            Stream recStream = response.GetResponseStream();
            //编码方式
            Encoding utf8 = Encoding.GetEncoding("utf-8");
            StreamReader sr = new StreamReader(recStream, utf8);
            //以字符串方式得到网页内容
            String content = sr.ReadToEnd();
            //将网页内容显示在TextBox中
            label3.Text = content;
        }



        private void getimgnumber()
        {
            nowid = nextid;
            Random ran = new Random();
            nextid = ran.Next(1, imgnum);
            textBox1.Text = nextid + "";
        }



        private void downoneimg()
        {
            if (!one)
            {
                yiyan();
            }
            ReadWebFile("http://oneimg.haotown.cn/img/bj@" + nextid + ".jpg", path + nextid + ".jpg");
            button1.Enabled = true;
            if (one)
            {
                getimgnumber();
                ReadWebFile("http://oneimg.haotown.cn/img/bj@" + nextid + ".jpg", path + nextid + ".jpg");
                one = false;
            }

        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://oneimg.haotown.cn");
        }
        [DllImport("wininet")]
        private extern static bool InternetGetConnectedState(out int connectionDescription, int reservedValue);


        private void main_Load(object sender, EventArgs e)
        {
            checknet();
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread thread1 = new Thread(new ThreadStart(ReadImgNumber));
            thread1.Start();
            Thread thread2 = new Thread(new ThreadStart(yiyan));
            thread2.Start();

        }


        public static bool IsConnectedInternet()
        {
            int i = 0;
            if (InternetGetConnectedState(out i, 0))
            {
                return true;
            }

            else
            {
                return false;
            }

        }
        public void checknet()
        {
            if (IsConnectedInternet())
            {
                label1.Text = "已联网";

            }
            else
            {
                label1.Text = "未联网";
            }

        }




        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            SystemParametersInfo(20, 0, path + nowid + ".jpg", 0x2);
            getimgnumber();
            Thread thread1 = new Thread(new ThreadStart(downoneimg));
            Control.CheckForIllegalCrossThreadCalls = false;
            thread1.Start();

        }



        private void linkLabel1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://app.haotown.cn");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            checknet();

        }

        private void main_Shown(object sender, EventArgs e)
        {
            path = @Path.GetTempPath() + "oneimg/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            getimgnumber();
            nowid = nextid;
            Thread thread1 = new Thread(new ThreadStart(downoneimg));
            Control.CheckForIllegalCrossThreadCalls = false;
            thread1.Start();
            loadini();
        }

        private void main_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x0112, 0xF012, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
        }




        private void label3_Click(object sender, EventArgs e)
        {
            yiyan();
        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (checkBox2.Checked)
            {
                if (int.Parse(textBox2.Text) > 0)
                {
                    timer2.Interval = int.Parse(textBox2.Text) * 60000;
                    timer2.Enabled = true;
                    IniFile ini = new IniFile(path + "oneimg.ini");
                    ini.IniWriteValue("oneimg", "time", textBox2.Text);
                    ini.IniWriteValue("oneimg", "tstart", "true");
                }

            }else 
            {
                timer2.Enabled = false;
                IniFile ini = new IniFile(path + "oneimg.ini");
                ini.IniWriteValue("oneimg", "tstart", "false");
            }
            if (checkBox1.Checked && !startgo)
            {
                MessageBox.Show("设置开机自启动，需要修改注册表 如设置失败 请用管理员权限运行", "提示");
                try
                {
                    string path = Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.SetValue("JcShutdown", path);
                    rk2.Close();
                    rk.Close();
                    IniFile ini = new IniFile(path + "oneimg.ini");
                    ini.IniWriteValue("oneimg", "start", "true");

                }
                catch (Exception ex)
                {
                    MessageBox.Show("权限不足 请用管理员权限运行", "提示");
                    IniFile ini = new IniFile(path + "oneimg.ini");
                    ini.IniWriteValue("oneimg", "start", "false");
                }
               
            }
            else if (!checkBox1.Checked && startgo)
            {
                try
                {
                    MessageBox.Show("取消开机自启动，需要修改注册表", "提示");
                    string path = Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.DeleteValue("JcShutdown", false);
                    rk2.Close();
                    rk.Close();
                    IniFile ini = new IniFile(path + "oneimg.ini");
                    ini.IniWriteValue("oneimg", "start", "false");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("权限不足 请用管理员权限运行", "提示");
                    IniFile ini = new IniFile(path + "oneimg.ini");
                    ini.IniWriteValue("oneimg", "start", "true");
                }
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != '\b')//这是允许输入退格键
            {
                if ((e.KeyChar < '0') || (e.KeyChar > '9'))//这是允许输入0-9数字
                {
                    e.Handled = true;
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            SystemParametersInfo(20, 0, path + nowid + ".jpg", 0x2);
            getimgnumber();
            Thread thread1 = new Thread(new ThreadStart(downoneimg));
            Control.CheckForIllegalCrossThreadCalls = false;
            thread1.Start();
        }
        private void loadini() 
        {
            IniFile ini = new IniFile(path+"oneimg.ini");
            string start= ini.IniReadValue("oneimg", "start");
            string time = ini.IniReadValue("oneimg", "time");
            string tstart = ini.IniReadValue("oneimg", "tstart");
            if (start=="true")
            {
                checkBox1.Checked = true;
                startgo = true;
            }
            if (time!="")
            {
                textBox2.Text = time;
                startgo = false;
            }
            if (tstart == "true")
            {
                checkBox2.Checked = true;
                if (int.Parse(textBox2.Text) > 0)
                {
                    timer2.Interval = int.Parse(textBox2.Text) * 60000;
                    timer2.Enabled = true;
                }
                else {
                    timer2.Enabled = false ;
                }
            }
        }

        public class ButtonEx : Button
        {
            public ButtonEx()
            {
                SetStyle(ControlStyles.Selectable, false);
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (this.ShowInTaskbar)
            {
                this.Hide();
                this.ShowInTaskbar = false;
            }
            else 
            {
                this.Show();
                this.ShowInTaskbar =true;
            }
        }
    }


    public class IniFile
    {
        public string path;             //INI文件名  

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
                    string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def,
                    StringBuilder retVal, int size, string filePath);

        //声明读写INI文件的API函数  
        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        //类的构造函数，传递INI文件名  
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        //写INI文件  
        public string IniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(255);
            int i = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }

        //读取INI文件指定  
    }
}
