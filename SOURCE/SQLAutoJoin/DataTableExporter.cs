using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANDREICSLIB;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace SQLAutoJoin
{
    public class Page
    {
        public List<List<string>> Cells = new List<List<string>>();
        public List<int> HeaderRows = new List<int>();
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


        public DataTableExporter()
        {
        }

        public void AddPage()
        {
            pages.Add(currentPage);
            currentPage = new Page { Name = "Page" + pages.Count };
        }

        public void AddRow(Dictionary<string, object> row, bool newheaders, string tableName)
        {
            if (newheaders)
            {
                var hd = row.Keys.ToList();
                hd.Insert(0, tableName);
                currentPage.Add(hd);
                currentPage.HeaderRows.Add(currentPage.RowCount);
            }

            var data = row.Values.Select(s => s.ToString()).ToList();
            data.Insert(0, tableName);

            //add data
            currentPage.Add(data);
        }

        private bool CellsMatch(ExcelPackage ep, int x, int y)
        {
            var matches = ep.Workbook.Worksheets.Select(s => s.Cells[y, x].Value).Distinct().Count();

            return matches == 1;
        }

        private void AutoFit(ExcelPackage ep)
        {
            int count = ep.Workbook.Worksheets.Count;
            for (int a = 1; a <= count; a++)
            {
                ep.Workbook.Worksheets[a].Cells.AutoFitColumns();
            }
        }

        private void SetCellsColour(ExcelPackage ep, int x, int y, Color col)
        {
            int count = ep.Workbook.Worksheets.Count;
            for (int c = 1; c <= count; c++)
            {
                var w = ep.Workbook.Worksheets[c];
                SetCellsColour(ep, w, x, y, col);
            }
        }

        private void SetCellsColour(ExcelPackage ep, ExcelWorksheet s, int x, int y, Color col)
        {
            s.Cells[y, x].Style.Border.Bottom.Style =
                   s.Cells[y, x].Style.Border.Left.Style =
                   s.Cells[y, x].Style.Border.Top.Style =
                   s.Cells[y, x].Style.Border.Right.Style = ExcelBorderStyle.Thin;

            s.Cells[y, x].Style.Border.Bottom.Color.SetColor(Color.Black);
            s.Cells[y, x].Style.Border.Left.Color.SetColor(Color.Black);
            s.Cells[y, x].Style.Border.Top.Color.SetColor(Color.Black);
            s.Cells[y, x].Style.Border.Right.Color.SetColor(Color.Black);

            s.Cells[y, x].Style.Fill.PatternType = ExcelFillStyle.Solid;
            s.Cells[y, x].Style.Fill.BackgroundColor.SetColor(col);
        }

        private void ColourFormatDocument(ExcelPackage ep, int maxx, int maxy)
        {
            //for each cell, see if same cell in each workpage matches
            for (int y = 1; y < maxy; y++)
            {
                for (int x = 1; x < maxx; x++)
                {
                    bool match = CellsMatch(ep, x, y);
                    if (match == false)
                        SetCellsColour(ep, x, y, Color.Red);
                }
            }
        }

        private void SetHeaderColours(ExcelPackage ep, int maxx, int maxy)
        {
            int i = 1;
            foreach (var p in pages)
            {
                var w = ep.Workbook.Worksheets[i];
                foreach (var y in p.HeaderRows)
                {
                    for (int x = 1; x < maxx; x++)
                    {
                        SetCellsColour(ep, w, x, y, Color.SlateGray);
                    }
                }

                foreach (var x in p.HeaderRows)
                {
                    for (int y = 1; y < maxy; y++)
                    {
                        SetCellsColour(ep, w, x, y, Color.DarkGray);
                    }
                }
                i++;
            }
        }

        public void ExportXLS(string filename)
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
        }

        private void SetWorkbookCells(ExcelPackage pck, Page p, ref int maxx, ref int maxy)
        {
            ExcelWorksheet worksheet = pck.Workbook.Worksheets.Add(p.Name);
            int x = 1, y = 1;
            foreach (var row in p.Cells)
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
