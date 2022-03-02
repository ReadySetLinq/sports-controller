using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataManager
{
    public partial class FormWorkSheetSelector : Form
    {
        FormDataManager dataManager;

        public FormWorkSheetSelector(FormDataManager form)
        {
            InitializeComponent();
            dataManager = form;

            cmbWorksheet.Items.Clear();
        }

        public void LoadWorkSheets(Dictionary<int, string> workSheets)
        {
            cmbWorksheet.Items.Clear();
            foreach (KeyValuePair<int, string> workSheet in workSheets)
            {
                cmbWorksheet.Items.Add(workSheet.Value);
            }
            cmbWorksheet.SelectedIndex = 0;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            dataManager.workSheetId = cmbWorksheet.SelectedIndex;
            dataManager.LoadWorkSheet();
        }
    }
}
