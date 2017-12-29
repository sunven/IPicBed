using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using QiniuZone = Qiniu.Storage.Zone;

namespace IPicBed
{
    public partial class Form1 : Form
    {
        public int Zone { get; set; }

        public string Bucket { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public string Domain { get; set; }

        public Form1()
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel2_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;
        }

        private void splitContainer1_Panel2_DragDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetData(DataFormats.FileDrop, false) as String[];
            Upload(files);
        }

        private void InitControl()
        {
            lblTip.Text = "";
            txtUrl.Text = "";
        }

        private void Upload(string[] files)
        {
            if (files == null)
            {
                return;
            }
            InitControl();
            var urlList = new List<string>();
            foreach (var item in files)
            {
                if (Directory.Exists(item))
                {
                    continue;
                }
                var url = Upload(files[0]);
                urlList.Add(url);

            }
            if (radMarkdown.Checked)
            {
                urlList = urlList.Select(c => $"![image]({c})").ToList();
            }

            var urlStrs = string.Join(",", urlList);
            Clipboard.SetDataObject(urlStrs);
            txtUrl.Text = urlStrs;
            lblTip.Text = "上传成功！已复制到剪贴板";
        }

        private string Upload(string path)
        {
            var mac = new Mac(AccessKey, SecretKey);
            var fileName = Path.GetFileNameWithoutExtension(path) + DateTime.Now.ToString("yyyyMMddHHmmssffff") + Path.GetExtension(path);

            var putPolicy = new PutPolicy { Scope = Bucket + ":" + fileName };
            putPolicy.SetExpires(3600);
            putPolicy.DeleteAfterDays = 1;
            var token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            var config = new Config
            {
                Zone = GetQiniuZone(Zone),
                UseHttps = true,
                UseCdnDomains = true,
                ChunkSize = ChunkUnit.U512K
            };
            var target = new FormUploader(config);
            var result = target.UploadFile(path, fileName, token, null);
            if ((int)HttpCode.OK == result.Code)
            {
                return Domain + "/" + fileName;
            }
            return null;
        }

        private void btnSelectPicture_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "All Image Files|*.bmp;*.ico;*.gif;*.jpeg;*.jpg;*.png;",
                Multiselect = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Upload(ofd.FileNames);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Zone = int.Parse(XmlHelper.Select(Program.XmlPath, Program.XPath + "Zone"));
            Bucket = XmlHelper.Select(Program.XmlPath, Program.XPath + "Bucket");
            AccessKey = XmlHelper.Select(Program.XmlPath, Program.XPath + "AccessKey");
            SecretKey = XmlHelper.Select(Program.XmlPath, Program.XPath + "SecretKey");
            Domain = XmlHelper.Select(Program.XmlPath, Program.XPath + "Domain");
        }

        private QiniuZone GetQiniuZone(int zone)
        {
            switch (zone)
            {
                case 0:
                    return QiniuZone.ZONE_CN_East;
                case 1:
                    return QiniuZone.ZONE_CN_North;
                case 2:
                    return QiniuZone.ZONE_CN_South;
                case 3:
                    return QiniuZone.ZONE_US_North;
                default:
                    throw new Exception("获取不到Zone");
            }
        }

        private void 七牛配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QiniuSettingForm form=new QiniuSettingForm();
            form.Show();
        }
    }
}
