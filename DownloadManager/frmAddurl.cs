using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DownloadManager
{
    public partial class frmAddurl : Form
    {
        public frmAddurl()
        {
            InitializeComponent();
        }
        public string Url { get; set; }
        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Url = txtUrl.Text;
            
        }
    }
}
