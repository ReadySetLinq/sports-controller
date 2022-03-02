using System.Windows.Forms;
using System.Data;
using System;
using System.Collections.Generic;

namespace DataManager
{
    public partial class FormDataManager : Form
    {
        public int workSheetId = 0;
        private string _filePath = string.Empty;
        private Dictionary<int, string> workSheets = new Dictionary<int, string>();
        readonly FormWorkSheetSelector frmSelector;

        public FormDataManager()
        {
            frmSelector = new FormWorkSheetSelector(this);
            InitializeComponent();
        }

        public void SaveDataTable(DataTable dataTable, string path, int index = 0)
        {
            // Make sure a path was given
            if (string.IsNullOrWhiteSpace(path))
            {
                // Exit out of loading as we don't have all paths needed
                return;
            }

            // Save the data to a .xlsx
            Excel.SaveDataTable(dataTable, path, index);
        }

        public void LoadWorkSheet()
        {
            dataGridManage.Columns.Clear();
            dataGridManage.DataSource = Excel.GetDataTable(_filePath, workSheetId);
            foreach (DataGridViewColumn column in dataGridManage.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            Text = string.Format("ReadySetLinq - DataManager [{0}] - Worksheet: {1}", _filePath, workSheets[workSheetId]);

            frmSelector.Visible = false;
            frmSelector.Enabled = false;
            this.Visible = true;
            this.Enabled = true;
            this.Focus();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel files (*.xlsx, *.xls)|*.xlsx;",
                FilterIndex = 0,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _filePath = openFileDialog.FileName;
                if (!string.IsNullOrEmpty(_filePath))
                {
                    workSheetId = 0;
                    workSheets = Excel.GetWorksheets(_filePath);
                    if (workSheets.Count > 1)
                    {
                        frmSelector.LoadWorkSheets(workSheets);
                        this.Visible = false;
                        this.Enabled = false;
                        frmSelector.Visible = true;
                        frmSelector.Enabled = true;
                    }
                    else
                    {
                        LoadWorkSheet();
                    }
                }

            }
        }

        private void dataGridManage_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            dataGridManage.RemoveEmptyRows();
        }

        private void btnSave_Click(object sender, System.EventArgs e)
        {
            dataGridManage.RemoveEmptyRows();
            SaveDataTable((DataTable)dataGridManage.DataSource, _filePath);
        }
    }
}
