namespace NSun.Data
{
    internal class DbQueryAndArgument
    {
        public DBQuery DbQuery { get; set; }

        public object Argument { get; set; }

        public UnitOfWorkType WorkType { get; set; }
    }

    internal enum UnitOfWorkType
    {
        Insert,
        Update,
        Delete,
        Custom,
        Select,
        StoredProcedure,
        Exec
    }
}