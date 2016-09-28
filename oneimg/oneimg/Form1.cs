using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;


namespace oneimg
{

    public partial class Form1 : Form
    {
        int i = 0;
        string path = @"c:\";//写入目录
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern int SystemParametersInfo(
         int uAction,
         int uParam,
         string lpvParam,
         int fuWinIni
         );
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
                MessageBox.Show("eero! Dont find oneimg server! Please check your Internet! \n 错误!无法链接到一图服务器!请检查你的网络");
               
            }

        }




    

        private void button2_Click(object sender, EventArgs e)
        {
           
 
            
        }
        private void getimgnumber()
        {
            Random ran = new Random();
            int RandKey = ran.Next(1, 4200);
            textBox1.Text = RandKey+ "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            path = @Path.GetTempPath() + "oneimg/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            getimgnumber();
            downoneimg();
        }

        private void downoneimg()
        {

            ReadWebFile("http://t5.haotown.cn/img/bj@" + textBox1.Text + ".jpg",path + i + ".jpg");
            button1.Enabled = true;
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.haotown.cn"); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            Image image = Image.FromFile(path + i + ".jpg");
            image.Save(path + "bj.bmp", System.Drawing.Imaging.ImageFormat.Bmp);
            SystemParametersInfo(20, 0, path + "bj.bmp", 0x2);
            getimgnumber();
            i++;
            Thread thread1 = new Thread(new ThreadStart(downoneimg));
            Control.CheckForIllegalCrossThreadCalls = false;
            thread1.Start();


        } 

    }




}


