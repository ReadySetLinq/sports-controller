using System;
using System.Windows.Forms;

namespace SportsController
{
    public partial class FormLauncher : Form
    {
        public FormLauncher()
        {
            InitializeComponent();
        }

        private void btnBasketball_Click(object sender, EventArgs e)
        {
            Hide();
            Basketball.FormBasketball frmBasketball = new Basketball.FormBasketball();
            frmBasketball.ShowDialog();
            Show();
            frmBasketball.Dispose();
        }

        private void btnVolleyball_Click(object sender, EventArgs e)
        {
            Hide();
            Volleyball.FormVolleyball frmVolleyball = new Volleyball.FormVolleyball();
            frmVolleyball.ShowDialog();
            Show();
            frmVolleyball.Dispose();
        }
    }
}
