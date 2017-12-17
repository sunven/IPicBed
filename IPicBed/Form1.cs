using Qiniu.Http;
using Qiniu.Storage;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace IPicBed
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void splitContainer1_Panel2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
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

            var urlStrs= string.Join(",", urlList);
            Clipboard.SetDataObject(urlStrs);
            txtUrl.Text = urlStrs;
            lblTip.Text = "上传成功！已复制到剪贴板";
        }

        private string Upload(string path)
        {
            var AccessKey = "VuozjapP1huZqCkDIIiRPhy9At2GsYvJzvscpg13";
            var SecretKey = "x1-dw8nJL6OY8BIzF6-a4evbVRGAa3kmvPIg2Scq";
            var Bucket = "v-video";
            var Domain = "http://7xk2dp.com1.z0.glb.clouddn.com";
            Mac mac = new Mac(AccessKey, SecretKey);
            Random rand = new Random();
            var fileName = Path.GetFileNameWithoutExtension(path) + DateTime.Now.ToString("yyyyMMddHHmmssffff")+ Path.GetExtension(path);
            //string key = string.Format("UploadFileTest_{0}.dat", rand.Next());

            //string filePath = LocalFile;

            PutPolicy putPolicy = new PutPolicy();
            putPolicy.Scope = Bucket + ":" + fileName;
            putPolicy.SetExpires(3600);
            putPolicy.DeleteAfterDays = 1;
            string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
            Config config = new Config();
            config.Zone = Zone.ZONE_CN_East;
            config.UseHttps = true;
            config.UseCdnDomains = true;
            config.ChunkSize = ChunkUnit.U512K;
            FormUploader target = new FormUploader(config);
            HttpResult result = target.UploadFile(path, fileName, token, null);
            Console.WriteLine("form upload result: " + result.ToString());
            if ((int)HttpCode.OK==result.Code)
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
              Multiselect=true
            };
            if (ofd.ShowDialog()==DialogResult.OK)
            {
                Upload(ofd.FileNames);
            }
        }
    }
}
