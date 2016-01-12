using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ANDREICSLIB;
using System.Data.Entity.Core.Metadata.Edm;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using ANDREICSLIB.ClassExtras;

namespace SQLRegex
{
    public partial class Form1 : Form
    {
        #region licensing

        private const string AppTitle = "SQLAutoJoin";
        private const double AppVersion = 0.1;
        private const String HelpString = "";

        private readonly String OtherText =
            @"©" + DateTime.Now.Year +
            @" Andrei Gec (http://www.andreigec.net)
Licensed under GNU LGPL (http://www.gnu.org/)
OCR © Tessnet2/Tesseract (http://www.pixel-technology.com/freeware/tessnet2/)(https://code.google.com/p/tesseract-ocr/)
Zip Assets © SharpZipLib (http://www.sharpdevelop.net/OpenSource/SharpZipLib/)
";

        public void InitLicensing()
        {
            Licensing.CreateLicense(this, menuStrip1, new Licensing.SolutionDetails(null, HelpString, AppTitle, AppVersion, OtherText));
        }

        #endregion
        private static string configPath = "SQLRegex.cfg";

        private Controller c;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitLicensing();

            var literals = LoadConfig();
            if (literals != null && literals.Any())
            {
                foreach (var l in literals)
                {
                    if (l.Item1 == "table")
                    {
                        tableLB.Text = l.Item2;
                    }
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void UpdateConnectionString(string cs)
        {
            if (string.IsNullOrEmpty(cs))
            {
                MessageBox.Show("error with empty connection string");
                return;
            }

            if (c != null && c.ConnectionString == cs) return;

            c = new Controller(cs);
            UpdateTables();
        }

        private void UpdateTables()
        {
            tableLB.Items.Clear();

            var t = c.GetTables();
            AddItems(t, ref tableLB);
            c.Tables = t;

        }

        private void AddItems(List<string> items, ref ListBox lb)
        {
            foreach (var i in items)
            {
                lb.Items.Add(i);
            }
        }

        private void QueryB_Click(object sender, EventArgs e)
        {
            if (c == null)
                return;

            UpdateConnectionString(connectionStringTB.Text);
            try
            {
                c.Generate(tableLB.Text, "where " + whereTB.Text, openXLSOnFinishToolStripMenuItem.Checked);
            }
            catch (Exception ex)
            {
                var m = ex.ToString();
                MessageBox.Show("Error:" + m);
            }
        }

        public void SaveConfig()
        {
            var savethese1 = new List<Control>();
            var savethese2 = new List<ToolStripItem>();
            var tp = new List<Tuple<string, string>>();

            if (dontSaveOptionsToolStripMenuItem.Checked)
            {
                savethese2.Add(dontSaveOptionsToolStripMenuItem);
            }
            else
            {
                savethese1.Add(connectionStringTB);
                savethese1.Add(whereTB);
                if (tableLB != null && tableLB.SelectedIndex != -1)
                    tp.Add(new Tuple<string, string>("table", tableLB.SelectedItem.ToString()));
            }

            FormConfigRestore.SaveConfig(this, configPath, savethese1, savethese2, tp);
        }

        public List<Tuple<string, string>> LoadConfig()
        {
            return FormConfigRestore.LoadConfig(this, configPath);

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                SaveConfig();
            }
            catch (Exception)
            {
            }
          
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateConnectionString(connectionStringTB.Text);
        }
    }
}
