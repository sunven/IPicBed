using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace IPicBed
{
    public partial class QiniuSettingForm : Form
    {

        public QiniuSettingForm()
        {
            InitializeComponent();
            object[] arrZone = { "东北", "华北", "华南", "北美" };
            comboxZone.Items.AddRange(arrZone);
        }

        private void QiniuSettingForm_Load(object sender, EventArgs e)
        {
            BindData();
        }

        private void BindData()
        {
            if (!File.Exists(Program.XmlPath))
            {
                return;
            }
            comboxZone.SelectedIndex = int.Parse(XmlHelper.Select(Program.XmlPath, Program.XPath + "Zone"));
            txtBucket.Text = XmlHelper.Select(Program.XmlPath, Program.XPath + "Bucket");
            txtAccessKey.Text = XmlHelper.Select(Program.XmlPath, Program.XPath + "AccessKey");
            txtSecretKey.Text = XmlHelper.Select(Program.XmlPath, Program.XPath + "SecretKey");
            txtDomain.Text = XmlHelper.Select(Program.XmlPath, Program.XPath + "Domain");
        }

        public void CreatXmlTree(string xmlPath)
        {
            if (File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }
            var xElement = new XElement(
                new XElement("Qiniu",
                    new XElement("Setting",
                        new XElement("Zone", comboxZone.SelectedIndex),
                        new XElement("Bucket", txtBucket.Text),
                        new XElement("AccessKey", txtAccessKey.Text),
                        new XElement("SecretKey", txtSecretKey.Text),
                        new XElement("Domain", txtDomain.Text)
                    )
                )
            );

            //需要指定编码格式，否则在读取时会抛：根级别上的数据无效。 第 1 行 位置 1异常
            var settings = new XmlWriterSettings
            {
                Encoding = new UTF8Encoding(false),
                Indent = true
            };
            var xw = XmlWriter.Create(xmlPath, settings);
            xElement.Save(xw);
            //写入文件
            xw.Flush();
            xw.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreatXmlTree(Program.XmlPath);
        }
    }
}
