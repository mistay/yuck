using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace yuck
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            loadSettings();

        }

        private void loadSettings()
        {
            txtMatrixserverHostname.Text = Properties.Settings.Default.matrixserver_hostname;
            txtMatrixserverUsername.Text = Properties.Settings.Default.matrixserver_username;
            txtMatrixserverPassword.Text = Properties.Settings.Default.matrixserver_password;

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.matrixserver_hostname = txtMatrixserverHostname.Text;
            Properties.Settings.Default.matrixserver_username = txtMatrixserverUsername.Text;
            Properties.Settings.Default.matrixserver_password = txtMatrixserverPassword.Text;
            Properties.Settings.Default.Save();

            this.Close();
        }
    }
}
