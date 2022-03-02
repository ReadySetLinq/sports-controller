using System.Windows.Forms;

namespace SportsController.Shared
{
    public partial class FormLoading : Form
    {
        public FormLoading() => InitializeComponent();

        public void Setup(int val, int max)
        {
            SetValue(val);
            SetMax(max);
        }

        public void SetMax(int val) => loadingBar.Maximum = val;

        public void SetValue(int val) => loadingBar.Value = val;

        public void Increase() => loadingBar.Value += 1;

        public void Completed() => loadingBar.Value = loadingBar.Maximum;
    }
}
