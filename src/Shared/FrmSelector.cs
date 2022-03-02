using System;
using System.Windows.Forms;

namespace SportsController.Shared
{
    public partial class FrmSelector : Form
    {
        public string SelectedDirectory = $"{Environment.CurrentDirectory}\\data\\";
        public bool Canceled = false;

        public FrmSelector(string directory = "")
        {
            InitializeComponent();

            if (!directory.Equals("")) SelectedDirectory = directory;

            txtDirectory.Text = SelectedDirectory;
        }

        public string FolderBrowser(string defaultPath = "")
        {
            string path = "";

            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (string.IsNullOrWhiteSpace(defaultPath))
                {
                    defaultPath = Environment.CurrentDirectory;
                }
                fbd.SelectedPath = defaultPath;

                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path = fbd.SelectedPath;
                }
            }

            return path;
        }

        private void txtDirectory_TextChanged(object sender, EventArgs e)
        {
            SelectedDirectory = txtDirectory.Text.Trim();

            if (txtDirectory.Text.Trim().Equals(string.Empty)) btnSelect.Enabled = false;
            else btnSelect.Enabled = true;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            txtDirectory.Text = FolderBrowser(SelectedDirectory);
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            Canceled = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Canceled = true;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
