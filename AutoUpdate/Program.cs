using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace AutoUpdate
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] parms)
        {
            if(parms.Length!=2)
            {
                MessageBox.Show("参数不符合要求！");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(parms[0],parms[1]));
        }
    }
}
