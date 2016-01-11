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

            var qr = c.Database.SqlQuery<string>(q);
            var res = qr.OrderBy(s => s.ToString()).ToList();
            return res;
        }

        public List<string> GetColumns(string table)
        {
            string q = @"SELECT COLUMN_NAME
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = N'" + table + "' ";

            var qr = c.Database.SqlQuery<string>(q);
            var res = qr.OrderBy(s => s.ToString()).Distinct().ToList();
            return res;
        }

        public List<string> GetValues(string table, string column, string where)
        {
            string q = string.Format("select top 100 {0} from {1} {2}", column, table, where);
            var resd = c.Database.DynamicSqlQuery(q);
            var ret = new List<string>();
            foreach (var r in resd)
            {
                var v = r.GetType().GetProperty(column).GetValue(r, null);
                var rv = v == null ? "(null)" : v.ToString();
                ret.Add(rv);
            }

            return ret;
        }

        private static string WhereGenerator(LviItem item)
        {
            string where = string.IsNullOrEmpty(item.where)
          ? string.Format("where {0} like '{1}'", item.column, item.rowValue.Replace("'", "''"))
          : string.Format("{0} and {1} like '{2}'", item.where, item.column, item.rowValue.Replace("'", "''"));

            return where;
        }

        public bool OnlyOnce(LviItem item)
        {
            var where2 = WhereGenerator(item);

            string q = string.Format("select top 100 {0} from {1} {2}", item.column, item.table, where2);

            var resd = c.Database.DynamicSqlQuery(q);
            var ret = new List<string>();
            foreach (var r in resd)
            {
                ret.Add("ok");
                if (ret.Count > 1)
                    return false;
            }

            return true;
        }

        private string UpdateRowQuery(LviItem item)
        {
            var where = WhereGenerator(item);

            string q = string.Format("update {0} \r\nset {1} = '{2}' \r\n{3}", item.table, item.column, item.postRegex.Replace("'", "''").Trim(), where);
            return q;
        }

        public void SaveChanges(List<LviItem> item, bool testMode)
        {
            var fn = "queries.txt";
            FileExtras.SaveToFile(fn, "");

            foreach (var i in item)
            {
                //make sure only one matches
                if (OnlyOnce(i) == false)
                {
                    var msg = string.Format("col={0}, table={1}, where={2}, value={3}", i.column, i.table, i.where,
                        i.rowValue);

                    throw new Exception("error, there exists multiple values for:" + msg);
                }

                var query = UpdateRowQuery(i);
                if (testMode)
                {
                    FileExtras.SaveToFile(fn, query, true);
                }
            }

            if (testMode)
            {
                Process.Start("notepad.exe", fn);
            }
        }

        public void Generate(string table, string @where)
        {
            DataTableExporter cs = new DataTableExporter();
            var ts = new List<string>();
            AddRows(ref cs, table, ref ts, @where);

            //output
            cs.ExportXLS("SQLAUTOJOIN" + FileExtras.GenerateRandomFileName("xlsx"));
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
            string q = $"select top 100 * from {table} {@where}";
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


                csv.AddRow(r, depth == 0, table);
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