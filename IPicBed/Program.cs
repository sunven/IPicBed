using System;
using System.Windows.Forms;

namespace IPicBed
{
    static class Program
    {
        public static string XmlPath { get; set; }

        public static string XPath { get; set; }

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            XmlPath = AppDomain.CurrentDomain.BaseDirectory + "Qiniu.xml";
            XPath= "Qiniu/Setting/";
            Application.Run(new Form1());
        }
    }
}
