using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Net;
using System.IO;
using System.Configuration;
namespace AutoUpdate
{
    public partial class Form1 : Form
    {
        const string TimeFormat = "持续时间:{0}";
        const string ProgressFormat = "{0}%";
        const string LogFormat = "{0}:{1}\r\n";
        string product;
        long version;
        string update_json = ConfigurationManager.AppSettings["update_url"].ToString();
        Stopwatch sw;
        public Form1(string product,string version)
        {
            InitializeComponent();
            this.product = product;
            this.version = ParseVer(version);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ClearData();
            AutoDownLoad();

        }

        private void AutoDownLoad()
        {
            sw = new Stopwatch();
            sw.Start();
            timer1.Interval = 1000;
            timer1.Start();
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += bg_DoWork;
            bg.RunWorkerCompleted += bg_RunWorkerCompleted;
            bg.RunWorkerAsync();
        }

        void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            sw.Stop();
            timer1.Stop();

            MessageBox.Show("update complete,enjoy!", "success");
            this.Close();
        }

        void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "download");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                WebClient client = new WebClient();
                StreamReader sr = new StreamReader(client.OpenRead(update_json));
                var s_json = sr.ReadToEnd();
                var modelList = JsonConvert.DeserializeObject<List<UpdateModel>>(s_json);
                modelList = modelList.Where(a => a.product == product && a.ver > version).OrderByDescending(a => a.ver).ToList();
                if (modelList != null && modelList.Count > 0)
                {
                    var model = modelList.FirstOrDefault();
                    int i = 1;
                    foreach (var fileInfo in model.files)
                    {
                        SetProgress(i, model.files.Count);
                        AppendLog("downloading " + fileInfo.file);
                        Stopwatch sw_speed = new Stopwatch();
                        sw_speed.Start();
                        FileStream fsWriter = new FileStream(Path.Combine(path, fileInfo.file), FileMode.OpenOrCreate, FileAccess.Write);
                        Stream stream = client.OpenRead(string.Format("{0}/{1}", model.url, fileInfo.file));
                        byte[] bytes = new byte[1024 * 1024];
                        int count = 0;
                        long countLen = 0;
                        while ((count = stream.Read(bytes, 0, bytes.Length)) > 0)
                        {
                            fsWriter.Write(bytes, 0, count);
                            countLen += count;
                            var speed = (decimal)(countLen*1000 / (sw_speed.ElapsedMilliseconds*1024));
                            SetSpeed(speed);
                        }
                        fsWriter.Close();
                        stream.Close();
                        sw_speed.Stop();
                        i++;
                    }

                    AppendLog("download update complete");
                }
            }
            catch (Exception ex)
            {
                AppendLog("download error");
                AppendLog(ex.ToString());
            }
        }
        MemoryStream StreamToMemoryStream(Stream instream)
        {
            MemoryStream outstream = new MemoryStream();
            const int bufferLen = 4096;
            byte[] buffer = new byte[bufferLen];
            int count = 0;
            while ((count = instream.Read(buffer, 0, bufferLen)) > 0)
            {
                outstream.Write(buffer, 0, count);
            }
            return outstream;
        }
        void ClearData()
        {
            SetProgress(0, 100);
            SetLog("");
            SetTime("0s");
        }
        void SetTime(string time)
        {
            this.Invoke(new Action(() =>
            {
                this.lbTime.Text = string.Format(TimeFormat, time);
            }));
        }
        void SetLog(string log)
        {
            this.Invoke(new Action(() =>
            {
                rtbLog.Text = "";
            }));
        }

        void SetSpeed(decimal speed)
        {
            this.Invoke(new Action(() =>
            {
                lbSpeed.Text = string.Format("{0}kb/s",Math.Floor(speed));
            }));
        }
        void AppendLog(string log)
        {
            this.Invoke(new Action(() =>
            {
                rtbLog.Text += string.Format(LogFormat, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), log);
            }));
        }
        void SetProgress(int value, int max)
        {
            this.Invoke(new Action(() => {
                double progress = Math.Floor((double)value*100 / max);
                this.lbProgress.Text = string.Format(ProgressFormat, progress);
                this.progressBar1.Value = value;
                this.progressBar1.Maximum = max;
            }));
        }

        long ParseVer(string version)
        {
            long data = 0;
            var temp = version.Replace(".", "");
            long.TryParse(temp, out data);
            return data;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string s_time = "0s";
            long senconds = sw.ElapsedMilliseconds/1000;
            if (senconds > 60)
            {
                decimal minutes = Math.Floor((decimal)senconds / 60);
                decimal new_senconds = senconds - 60 * minutes;
                s_time = string.Format("{0}m{1}s", minutes, new_senconds);
            }
            else
            {
                s_time = string.Format("{0}s", senconds);
            }
            SetTime(s_time);
        }
    }
}
