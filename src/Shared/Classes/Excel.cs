using OfficeOpenXml;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SportsController.Shared
{
    public static class Excel
    {

        public static Dictionary<int, string> GetWorksheets(string pFilePath)
        {
            Dictionary<int, string> workSheets = new Dictionary<int, string>();
            try
            {
                if (File.Exists(pFilePath))
                {

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using ExcelPackage pck = new ExcelPackage();
                    using (FileStream stream = File.OpenRead(pFilePath))
                    {
                        pck.Load(stream);
                    }

                    int id = 0;
                    foreach (ExcelWorksheet ws in pck.Workbook.Worksheets)
                    {
                        workSheets.Add(id, ws.Name);
                        id++;
                    }

                }
                else
                {
                    return new Dictionary<int, string>();
                }
            }
            catch
            {
                return new Dictionary<int, string>();
            }
            return workSheets;
        }

        public static DataTable GetDataTable(string pFilePath, int pSheetIndex = 0)
        {
            DataTable tbl = new DataTable();
            try
            {
                if (File.Exists(pFilePath))
                {

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using ExcelPackage pck = new ExcelPackage();
                    using (FileStream stream = File.OpenRead(pFilePath))
                    {
                        pck.Load(stream);
                    }

                    ExcelWorksheet ws = pck.Workbook.Worksheets[pSheetIndex];
                    foreach (ExcelRangeBase firstRowCell in ws.Cells[1, 1, 1, ws.Dimension.End.Column])
                    {
                        tbl.Columns.Add(firstRowCell.Text);
                    }

                    int startRow = 2;
                    for (int rowNum = startRow; rowNum <= ws.Dimension.End.Row; rowNum++)
                    {
                        ExcelRange wsRow = ws.Cells[rowNum, 1, rowNum, ws.Dimension.End.Column];
                        DataRow row = tbl.Rows.Add();
                        foreach (ExcelRangeBase cell in wsRow)
                        {
                            row[cell.Start.Column - 1] = cell.Text;
                        }
                    }
                    tbl.AcceptChanges();
                }
                else
                {
                    return new DataTable();
                }
            }
            catch
            {
                return new DataTable();
            }
            return tbl;
        }

        public static void SaveDataTable(DataTable tbl, string pFilePath, int pSheetIndex = 0)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using ExcelPackage pck = new ExcelPackage(pFilePath);
                ExcelWorksheet ws = pck.Workbook.Worksheets[pSheetIndex];
                tbl.AcceptChanges();
                ws.Cells["A1"].LoadFromDataTable(tbl, true);
                pck.Save();
            }
            catch { }
        }
    }
}