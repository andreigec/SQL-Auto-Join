using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using ANDREICSLIB.ClassExtras;

namespace SQLAutoJoin
{
    public class Controller
    {
        private readonly DbContext c;

        private readonly Dictionary<string, List<Dictionary<string, object>>> cache =
            new Dictionary<string, List<Dictionary<string, object>>>();

        public string ConnectionString;
        public List<string> Tables;

        public Controller(string connectionString)
        {
            ConnectionString = connectionString;
            c = new DbContext(ConnectionString);
        }

        public List<string> GetTables()
        {
            var q = @"SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE' ";

            c.Database.CommandTimeout = 3;
            var qr = c.Database.SqlQuery<string>(q);

            var res = qr.OrderBy(s => s.ToString()).ToList();
            return res;
        }

        public void Generate(string table, string @where, bool openFile, bool alphaHeaderCols)
        {
            var cs = new DataTableExporter();
            var ts = new List<string>();
            var headers = new List<string>();

            if (alphaHeaderCols)
            {
                GetHeaderRows(ref cs, ref headers, table, ref ts, @where);
                //order
                headers = headers.OrderBy(s => s).ToList();
                //init
                cs.HeaderRows = headers;
                ts = new List<string>();
            }

            AddRows(ref cs, table, ref ts, @where, alphaHeaderCols);

            //output
            cs.ExportXLS("SQLAUTOJOIN" + FileExtras.GenerateRandomFileName("xlsx"), openFile);
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
            var ids =
                GetListOfForeignKeyTables(cols, origtable)
                    .Select(s => s.Substring(0, s.LastIndexOf("id", StringComparison.CurrentCultureIgnoreCase)))
                    .ToList();
            var ret = new List<SQLKey>();

            foreach (var keyname in ids)
            {
                var tablename = Tables.FirstOrDefault(s => s != null && (s == "L" + keyname || s == keyname));
                if (tablename != null)
                    ret.Add(new SQLKey {KeyName = keyname + "ID", TableName = tablename});
            }

            return ret;
        }

        public List<Dictionary<string, object>> RunQuery(string table, string @where)
        {
            string q = $"select top 10 * from {table} {@where}";
            if (cache.ContainsKey(q))
                return cache[q];

            var tableValues = c.Database.DynamicSQlQueryToDict(q);
            tableValues.ForEach(s => DictionaryExtras.RemoveEmptyKeyValues(ref s));
            cache[q] = tableValues;
            return tableValues;
        }

        private void AddRows(ref DataTableExporter csv, string table, ref List<string> seentables, string where,
            bool alphaHeaderCols, int depth = 0)
        {
            if (seentables.Contains(table))
                return;

            //get data
            var tableValues = RunQuery(table, @where);

            //insert
            var i = 0;
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
                    AddRows(ref csv, t.TableName, ref seentables, lwhere, alphaHeaderCols, depth + 1);
                }

                if (depth == 0)
                {
                    //finalise
                    csv.AddPage();
                }

                i++;
            }
        }

        private void GetHeaderRows(ref DataTableExporter csv, ref List<string> headers, string table,
            ref List<string> seentables, string where, int depth = 0)
        {
            if (seentables.Contains(table))
                return;

            //get data
            var tableValues = RunQuery(table, @where);

            //insert
            var i = 0;
            foreach (var r in tableValues)
            {
                if (depth == 0)
                {
                    seentables = new List<string>();
                }
                seentables.Add(table);

                //
                var list = headers;
                var unique = r.Keys.Where(s => list.Contains(s) == false).ToList();
                if (headers.Any() == false)
                    headers.Add("");

                headers.AddRange(unique);
                //
                var ts = GetParsedForeignKeyTables(r.Keys.ToList(), table);
                //recurse with these tables
                foreach (var t in ts)
                {
                    var key = t.KeyName;
                    var val = r[key];
                    var lwhere = $" where {key} = {val}";
                    GetHeaderRows(ref csv, ref headers, t.TableName, ref seentables, lwhere, depth + 1);
                }

                if (depth == 0)
                {
                    return;
                }
                i++;
            }
        }

        private List<string> GetHeaderRows(Dictionary<string, object> tablevalues)
        {
            //foreach (var rows in tablevalues)
            //{

            //    var unique = rows.Keys.Where(s => HeaderRows.Contains(s) == false).ToList();
            //    if (HeaderRows.Any() == false)
            //        HeaderRows.Add("");

            //    HeaderRows.AddRange(unique);
            //}
            return null;
        }

        public class SQLKey
        {
            public string TableName { get; set; }
            public string KeyName { get; set; }
        }
    }
}