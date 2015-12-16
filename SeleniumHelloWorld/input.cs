using System;
using System.Windows.Forms;

namespace SeleniumHelloWorld
{
    public partial class input : Form
    {
        public input()
        {
            InitializeComponent();
            var resources = new Resources();
            txtUserName.Text = resources.userName;
            txtPass.Text = resources.pass;
            btnSend.Enabled = false;
            KeyPreview = true;
        }

        private void input_Load(object sender, EventArgs e)
        {
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            //var resources = new Resources(txtUserName.Text, txtPass.Text, txtLinkToPost.Text);
            ////Resources resources = new Resources();
            //var function = new Function();
            //function.Init(resources);
            //MessageBox.Show("Start Cralwing");
            //txtLinkToPost.Enabled = false;
            //txtUserName.Enabled = false;
            //txtPass.Enabled = false;
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {
        }

        private void txtLinkToPost_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtLinkToPost.Text) || string.IsNullOrWhiteSpace(txtLinkToPost.Text))
            {
                MessageBox.Show("Please input link to post!");
                btnSend.Enabled = false;
            }
            else
            {
                btnSend.Enabled = true;
            }
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