using System.Windows.Forms;

namespace SportsController.Shared
{
    public static class DataGridViewExtensions
    {
        public static void RemoveEmptyRows(this DataGridView dgv)
        {
            try
            { 
                for (int i = 1; i < dgv.RowCount - 1; i++)
                {
                    try
                    {
                        if (dgv.Rows[i].Cells[0].Value == null || string.IsNullOrEmpty(dgv.Rows[i].Cells[0].Value.ToString()))
                        {
                            dgv.Rows.RemoveAt(i);
                            i--;
                        }
                    } 
                    catch { }
                }
            }
            catch { }
        }
    }
}
