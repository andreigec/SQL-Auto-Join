using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using ANDREICSLIB;
using ANDREICSLIB.ClassExtras;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SQLAutoJoin
{
    public class Page
    {
        public List<List<string>> Cells = new List<List<string>>();
        public string Name = "Page0";

        public void Add(List<string> row)
        {
            Cells.Add(row);
        }

        public int RowCount
        {
            get
            {
                return Cells.Count;
            }
        }
    }

    public class DataTableExporter
    {
        public List<Page> pages = new List<Page>();
        public Page currentPage = new Page();
        public List<string> HeaderRows = new List<string>();


        public DataTableExporter()
        {
        }
        
        public void AddPage()
        {
            pages.Add(currentPage);
            currentPage = new Page { Name = "Page" + pages.Count };
        }

        public void AddRow(Dictionary<string, object> rows, string tableName)
        {
            var unique = rows.Keys.Where(s => HeaderRows.Contains(s) == false).ToList();
            if (HeaderRows.Any() == false)
                HeaderRows.Add("");

            HeaderRows.AddRange(unique);

            var data = ListExtras.Initialise(HeaderRows.Count, "");
            if (tableName == "Event")
                ;
            foreach (var row in rows)
            {
                var i = HeaderRows.IndexOf(row.Key);
                if (i != -1)
                {
                    data[i] = row.Value.ToString();
                }
            }
            data[0] = tableName;

            //add data
            currentPage.Add(data);
        }

        private bool CellsMatch(ExcelPackage ep, int x, int y)
        {
            var matches = ep.Workbook.Worksheets.Select(s => s.Cells[y, x].Value ?? "").ToList();
            var matches2 = matches.Distinct().Count();
            return matches2 == 1;
        }

        private void AutoFit(ExcelPackage ep)
        {
            int count = ep.Workbook.Worksheets.Count;
            for (int a = 1; a <= count; a++)
            {
                ep.Workbook.Worksheets[a].Cells.AutoFitColumns();
                ep.Workbook.Worksheets[a].View.FreezePanes(2, 2);
            }
        }

        private void SetCellsStyle(ExcelPackage ep, int x, int y, Color col)
        {
            int count = ep.Workbook.Worksheets.Count;
            for (int c = 1; c <= count; c++)
            {
                SetCellsStyle(ep.Workbook.Worksheets[c], x, y, col);
            }
        }

        private void SetCellsStyle(ExcelWorksheet s, int x, int y, Color col)
        {
            s.Cells[y, x].Style.Fill.PatternType = ExcelFillStyle.Solid;
            s.Cells[y, x].Style.Fill.BackgroundColor.SetColor(col);

            s.Cells[y, x].Style.Border.Bottom.Style =
                 s.Cells[y, x].Style.Border.Left.Style =
                 s.Cells[y, x].Style.Border.Top.Style =
                 s.Cells[y, x].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            s.Cells[y, x].Style.Border.Bottom.Color.SetColor(Color.Black);
            s.Cells[y, x].Style.Border.Left.Color.SetColor(Color.Black);
            s.Cells[y, x].Style.Border.Top.Color.SetColor(Color.Black);
            s.Cells[y, x].Style.Border.Right.Color.SetColor(Color.Black);
        }

        private void ColourFormatDocument(ExcelPackage ep, int maxx, int maxy)
        {
            //for each cell, see if same cell in each workpage matches
            for (int y = 1; y < maxy; y++)
            {
                for (int x = 1; x < maxx; x++)
                {
                    bool match = CellsMatch(ep, x, y);
                    Color c = match ? Color.Transparent : Color.Red;
                    SetCellsStyle(ep, x, y, c);
                }
            }
        }

        private void SetHeaderColours(ExcelPackage ep, int maxx, int maxy)
        {
            for (int i = 1; i <= pages.Count; i++)
            {
                var w = ep.Workbook.Worksheets[i];

                for (int y = 1; y < maxy; y++)
                {
                    SetCellsStyle(w, 1, y, Color.Gainsboro);
                }

                for (int x = 1; x < maxx; x++)
                {
                    SetCellsStyle(w, x, 1, Color.Azure);
                }
            }
        }

        public void ExportXLS(string filename, bool openfile)
        {
            FileInfo newFile = new FileInfo(filename);

            ExcelPackage pck = new ExcelPackage(newFile);

            int maxx = 1, maxy = 1;
            foreach (var p in pages)
            {
                SetWorkbookCells(pck, p, ref maxx, ref maxy);
            }
            
            ColourFormatDocument(pck, maxx, maxy);
            SetHeaderColours(pck, maxx, maxy);
            AutoFit(pck);
            pck.Save();
            pck.Dispose();
            if (openfile)
                Process.Start(filename);
        }

        private void SetWorkbookCells(ExcelPackage pck, Page p, ref int maxx, ref int maxy)
        {
            ExcelWorksheet worksheet = pck.Workbook.Worksheets.Add(p.Name);
            int x = 1, y = 1;
            var rows = new List<List<string>>();
            rows.Add(HeaderRows);
            rows.AddRange(p.Cells);

            foreach (var row in rows)
            {
                foreach (var cell in row)
                {
                    worksheet.Cells[y, x].Value = cell;
                    x++;
                    if (x > maxx)
                        maxx = x;
                }
                x = 1;
                y++;
                if (y > maxy)
                    maxy = y;
            }
        }
    }
}
