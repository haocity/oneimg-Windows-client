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

namespace oneimg_wallpaper
{
    public partial class main : Form
    {
        int i = 0;
        string path = @"c:\";//写入目录
        int num = 4000;//默认图片数量
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
        public void Read(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.Default);
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                num =int.Parse(line.ToString());

            }
        }
        public void ReadImgNumber()
        {
            ReadWebFile("http://t4.haotown.cn/data/", path + "num.inf");
            Read(path + "num.inf");
        }



    

        private void getimgnumber()
        {
            Random ran = new Random();
            int RandKey = ran.Next(1, num);
            textBox1.Text = RandKey+ "";
        }



        private void downoneimg()
        {

            ReadWebFile("http://oneimg.haotown.cn/img/bj@" + textBox1.Text + ".jpg",path + i + ".jpg");
            button1.Enabled = true;
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
            SystemParametersInfo(20, 0, path + i + ".jpg", 0x2);
            getimgnumber();
            i++;
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
            downoneimg();
            ReadImgNumber();

        }

        private void main_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x0112, 0xF012, 0); 
        }

    }
    public class ButtonEx : Button
    {
        public ButtonEx()
        {
            SetStyle(ControlStyles.Selectable, false);
        }
    }
}
