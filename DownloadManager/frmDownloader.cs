using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadManager
{
    public partial class frmDownloader : Form
    {
        private frmMain _frmMain;
        public frmDownloader(frmMain frm)
        {
            InitializeComponent();
            _frmMain = frm;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Uri uri = new Uri(this.Url);
            FileName = System.IO.Path.GetFileName(uri.AbsolutePath);
            client.DownloadFileAsync(uri, Properties.Settings.Default.Path + "/" + FileName);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            client.CancelAsync();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog() { Description = "Select your path" })
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = fbd.SelectedPath;
                    Properties.Settings.Default.Path = txtPath.Text;
                    Properties.Settings.Default.Save();
                }
            }
        }
        WebClient client;
        public string Url { get; set; }

        public string FileName { get; set; }

        public double Percentage { get; set; }

        public double FileSize { get; set; }

        private void frmDownloader_Load(object sender, EventArgs e)
        {
            client = new WebClient();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
            txtAddress.Text = Url;
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            Database.FilesRow row = App.DB.Files.NewFilesRow();
            row.Url = Url;
            row.FileName = FileName;
            row.FileSize = string.Format("{0:0.##} KB", FileSize / 1024);
            row.DateTime = DateTime.Now;
            App.DB.Files.AddFilesRow(row);
            App.DB.AcceptChanges();
            App.DB.WriteXml(string.Format("{0}/data.bat", Application.StartupPath));
            ListViewItem item = new ListViewItem(row.Id.ToString());
            item.SubItems.Add(row.Url);
            item.SubItems.Add(row.FileName);
            item.SubItems.Add(row.FileSize);
            item.SubItems.Add(row.DateTime.ToLongDateString());
            _frmMain.listView1.Items.Add(item);
            this.Close();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Minimum = 0;
            double recieve = double.Parse(e.BytesReceived.ToString());
            FileSize = double.Parse(e.TotalBytesToReceive.ToString());
            Percentage = recieve / FileSize * 100;
            lblStatus.Text = $"Downloaded {string.Format("{0:0.##}", Percentage)}";
            progressBar.Value = int.Parse(Math.Truncate(Percentage).ToString());
            progressBar.Update();
        }
    }
}
