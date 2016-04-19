using System;
using System.Windows.Forms;
using Extention;

namespace SeleniumHelloWorld
{
    public partial class Input : Form
    {
        private Resources _resources = new Resources();
        public Input()
        {
            InitializeComponent();
            btnSend.Enabled = false;
            KeyPreview = true;
        }

        private bool IsAllTextBoxFillIn()
        {
            if (!txtSearchString.Text.IsNull() && !txtUserName.Text.IsNull() && !txtPass.Text.IsNull())
            {
                return true;
            }
            return false;
        }
        private void input_Load(object sender, EventArgs e)
        {
             txtUserName.Leave += TextboxLeave;
            txtPass.Leave += TextboxLeave;
            txtSearchString.Leave += TextboxLeave;
        }

        private void TextboxLeave(object sender, EventArgs eventArgs)
        {
            if (IsAllTextBoxFillIn())
            {

                btnSend.Enabled = true;
            }
            else
            {
                btnSend.Enabled = false;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            _resources.UserName = txtUserName.Text;
            _resources.Pass = txtPass.Text;
            _resources.SearchString = txtSearchString.Text;
            DisableAllTextBox();
            btnSend.Enabled = false;
            MessageBox.Show("Start Cralwing");
            var crawlPost = new CrawlPost(_resources);
            crawlPost.Init();
        }

        private void DisableAllTextBox()
        {
            txtSearchString.Enabled = false;
            txtUserName.Enabled = false;
            txtPass.Enabled = false;
        }

        private void input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend.PerformClick();
            }
        }
    }
}