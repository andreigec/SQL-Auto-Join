using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ANDREICSLIB;
using ANDREICSLIB.ClassExtras;
using Newtonsoft.Json;
using SQLAutoJoin;

namespace SQLRegex
{
    public class Controller
    {
        public string ConnectionString;
        private DbContext c;
        public List<string> Tables;
        //public List<string> Columns;
        //public string InRegex;
        //public string OutRegex;
        //public bool TestMode = true;


        public Controller(string connectionString)
        {
            ConnectionString = connectionString;
            c = new DbContext(ConnectionString);
        }

        public List<string> GetTables()
        {
            string q = @"SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE' ";

            c.Database.CommandTimeout = 3;
            var qr = c.Database.SqlQuery<string>(q);

            var res = qr.OrderBy(s => s.ToString()).ToList();
            return res;
        }

        public void Generate(string table, string @where, bool openFile)
        {
            DataTableExporter cs = new DataTableExporter();
            var ts = new List<string>();
            AddRows(ref cs, table, ref ts, @where);

            //output
            cs.ExportXLS("SQLAUTOJOIN" + FileExtras.GenerateRandomFileName("xlsx"), openFile);
        }

        public class SQLKey
        {
            public string TableName { get; set; }
            public string KeyName { get; set; }
        }

        public List<string> GetListOfForeignKeyTables(List<string> cols, string origtable)
        {
            //ends with id, but isnt orig table
            var idcols =
                cols.Where(
                    s =>
                        s.Length != 2 && s.EndsWith("id", true, CultureInfo.CurrentCulture) &&
                        s.ToLower() != origtable.ToLower() + "id").ToList();

            return idcols;
        }

        public List<SQLKey> GetParsedForeignKeyTables(List<string> cols, string origtable)
        {
            var ids = GetListOfForeignKeyTables(cols, origtable).Select(s => s.Substring(0, s.LastIndexOf("id", StringComparison.CurrentCultureIgnoreCase))).ToList();
            var ret = new List<SQLKey>();

            foreach (var keyname in ids)
            {
                //var tablename = Tables.FirstOrDefault(s => Tables.Any(s2 => s2 == "L" + s || s == s2) && seentables2.Any(s2 => s2 == s) == false).ToList();
                var tablename = Tables.FirstOrDefault(s => s != null && (s == "L" + keyname || s == keyname));
                if (tablename != null)
                    ret.Add(new SQLKey() { KeyName = keyname + "ID", TableName = tablename });
            }

            return ret;
        }

        private void AddRows(ref DataTableExporter csv, string table, ref List<string> seentables, string where, int depth = 0)
        {
            if (seentables.Contains(table))
                return;

            //get data
            string q = $"select top 10 * from {table} {@where}";
            var tableValues = c.Database.DynamicSQlQueryToDict(q);
            tableValues.ForEach(s => DictionaryExtras.RemoveEmptyKeyValues(ref s));

            //insert
            int i = 0;
            foreach (var r in tableValues)
            {
                if (depth == 0)
                {
                    seentables = new List<string>();
                }
                seentables.Add(table);

                csv.AddRow(r, table);
                var ts = GetParsedForeignKeyTables(r.Keys.ToList(), table);
                //recurse with these tables
                foreach (var t in ts)
                {
                    var key = t.KeyName;
                    var val = r[key];
                    var lwhere = $" where {key} = {val}";
                    AddRows(ref csv, t.TableName, ref seentables, lwhere, depth + 1);
                }

                if (depth == 0)
                {
                    //finalise
                    csv.AddPage();
                }

                i++;
            }
        }
    }
}