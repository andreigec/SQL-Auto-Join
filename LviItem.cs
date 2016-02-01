namespace SQLRegex
{
    public class LviItem
    {
        public string table;
        public string column;
        public string rowValue;
        public string postRegex;
        public string where;

        public LviItem(string table, string column, string rowValue, string postRegex, string where)
        {
            this.where = where;
            this.column = column;
            this.postRegex = postRegex;
            this.rowValue = rowValue;
            this.table = table;
        }
    }
}