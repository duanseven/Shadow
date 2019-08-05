using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneratorV2.ViewModel;
using NSun.Data;
using System.Data;
using System.Reflection;
using GeneratorV2.Model;
using System.IO;
using NSun.Data.OracleClient;

namespace GeneratorV2.Service
{
    public class Services
    {
        public SqlType SQLType { get; set; }

        public string Connstr { get; set; }

        public Services(SqlType st, string connstr)
        {
            SQLType = st;
            Connstr = connstr;
        }

        public SQLOutInfo GetBindList()
        {
            SQLOutInfo reslist = null;
            switch (SQLType)
            {
                case SqlType.Sqlserver8:
                    reslist = new MSSQL2000();
                    break;
                case SqlType.Sqlserver9:
                case SqlType.Sqlserver10:
                    reslist = new MSSQL2005();
                    break;
                case SqlType.MsAccess:
                    reslist = new MSAccess();
                    break;
                case SqlType.Oracle:
                case SqlType.Oracle9:
                    reslist = new Oracle();
                    break;
                case SqlType.MySql:
                    reslist = new MySql();
                    break;
                case SqlType.MySql8:
                    reslist = new MySql8();
                    break;
                case SqlType.Db2:
                    reslist = new Db2();
                    break;
                case SqlType.Sqlite:
                    reslist = new Sqlite();
                    break;
                case SqlType.PostgreSql:
                    reslist = new PostgreSql();
                    break;
                default:
                    reslist = new MSSQL2005();
                    break;
            }
             reslist.Bind(Connstr);
            return reslist;
        }

    }

    public abstract class SQLOutInfo : NotificationObject, ICloneable
    {

        private List<TVPSelect> tables;
        public List<TVPSelect> Tables { get { return tables; } set { tables = value; RaisePropertyChanged("Tables"); } }
        private List<TVPSelect> views;
        public List<TVPSelect> Views { get { return views; } set { views = value; RaisePropertyChanged("Views"); } }
        private List<TVPSelect> sproc;
        public List<TVPSelect> Sproc { get { return sproc; } set { sproc = value; RaisePropertyChanged("Sproc"); } }

        public SQLOutInfo()
        {
            Tables = new List<TVPSelect>();
            Views = new List<TVPSelect>();
            Sproc = new List<TVPSelect>();
        }

        public abstract void Bind(string connstr);

        #region ICloneable 成员

        public object Clone()
        {
            var cloneobj = (SQLOutInfo)MemberwiseClone();
            cloneobj.Tables = new List<TVPSelect>(this.Tables);
            cloneobj.Views = new List<TVPSelect>(this.Views);
            cloneobj.Sproc = new List<TVPSelect>(this.Sproc);
            return cloneobj;
        }

        #endregion
    }

    public class MSSQL2000 : SQLOutInfo
    {
        public override void Bind(string connstr)
        {
            DBQuery<GeneratorV2.Sql.sysobjects> query =                
            DBFactory.CreateDBQuery<GeneratorV2.Sql.sysobjects>(SqlType.Sqlserver8, connstr);            

            Tables = (from c in query.ToList<string>(query.CreateQuery()
                                .Where(Sql.sysobjectsMapping.xtype == "U" && Sql.sysobjectsMapping.status >= 0).SortBy(Sql.sysobjectsMapping.name.Desc).
                                Select(
                                    Sql.sysobjectsMapping.name))
                      select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();
            Views = (from c in query.ToList<string>(query.CreateQuery()
                                   .Where(Sql.sysobjectsMapping.xtype == "V" && Sql.sysobjectsMapping.status >= 0).SortBy(Sql.sysobjectsMapping.name.Desc).
                                   Select(
                                       Sql.sysobjectsMapping.name))
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();
            Sproc = (from c in query.ToList<string>(query.CreateQuery()
                                 .Where(Sql.sysobjectsMapping.xtype == "P" && Sql.sysobjectsMapping.status >= 0).SortBy(Sql.sysobjectsMapping.name.Desc).
                                 Select(
                                     Sql.sysobjectsMapping.name))
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

        }
    }

    public class MSSQL2005 : SQLOutInfo
    {
        public override void Bind(string connstr)
        {
            DBQuery<GeneratorV2.Sql.sysobjects> query =
                DBFactory.CreateDBQuery<GeneratorV2.Sql.sysobjects>(SqlType.Sqlserver9, connstr);    
                //new DBQuery<GeneratorV2.Sql.sysobjects>(new Database(SqlType.Sqlserver9, connstr));

            Tables = (from c in query.ToList<string>(query.CreateQuery()
                                                  .Join(new Sql.schemas(), Sql.sysobjectsMapping.uid == Sql.schemasMapping.schema_id)
                                                  .Where(Sql.sysobjectsMapping.xtype == "U" &&
                                                         Sql.sysobjectsMapping.name != "sysdiagrams")
                                                  .SortBy(Sql.sysobjectsMapping.name.Desc)
                                                  .Select((Sql.schemasMapping.name + "." + Sql.sysobjectsMapping.name).As("name")))
                      select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();
            Views = (from c in query.ToList<string>(query.CreateQuery()
                                                 .Join(new Sql.schemas(), Sql.sysobjectsMapping.uid == Sql.schemasMapping.schema_id)
                                                 .Where(Sql.sysobjectsMapping.xtype == "V" &&
                                                        Sql.sysobjectsMapping.name != "sysdiagrams")
                                                 .SortBy(Sql.sysobjectsMapping.name.Desc)
                                                 .Select((Sql.schemasMapping.name + "." + Sql.sysobjectsMapping.name).As("name")))
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

            Sproc = (from c in query.ToList<string>(query.CreateQuery()
                                                .Join(new Sql.schemas(), Sql.sysobjectsMapping.uid == Sql.schemasMapping.schema_id)
                                                .Where(Sql.sysobjectsMapping.xtype == "P" &&
                                                       !Sql.sysobjectsMapping.name.StartsWith("dt_") &&
                                                       !Sql.sysobjectsMapping.name.Like("sp_%diagram%"))
                                                .SortBy(Sql.sysobjectsMapping.name.Desc)
                                                .Select((Sql.schemasMapping.name + "." + Sql.sysobjectsMapping.name).As("name")))
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

        }
    }

    public class MSAccess : SQLOutInfo
    {
        public override void Bind(string connstr)
        {
            ADODB.ConnectionClass conn = new ADODB.ConnectionClass();
            conn.Provider = connstr.IndexOf("12") != -1
                                ? "Microsoft.ACE.OLEDB.12.0"
                                : "Microsoft.Jet.OLEDB.4.0";

            var constr = connstr.Substring(
                connstr.IndexOf("data source") +
                "data source".Length).Trim('=', ' ').Trim
                (';', ' ');
            conn.Open(constr, null, null, 0);

            ADODB.Recordset rsTables = conn.GetType().InvokeMember("OpenSchema",
                BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaTables }) as ADODB.Recordset;
            ADODB.Recordset rsViews = conn.GetType().InvokeMember("OpenSchema",
                BindingFlags.InvokeMethod, null, conn, new object[] { ADODB.SchemaEnum.adSchemaViews }) as ADODB.Recordset;

            DataTable tab = new DataTable();
            DataTable viw = new DataTable();
            tab.Columns.Add(new DataColumn("name", typeof(string)));
            viw.Columns.Add(new DataColumn("name", typeof(string)));
            while (!rsViews.EOF)
            {
                if (!(rsViews.Fields["TABLE_NAME"].Value as string).StartsWith("MSys"))
                {
                    DataRow dr = tab.NewRow();
                    dr["name"] = rsViews.Fields["TABLE_NAME"].Value.ToString();
                    tab.Rows.Add(dr);
                }
                rsViews.MoveNext();
            }
            while (!rsTables.EOF)
            {
                if (!(rsTables.Fields["TABLE_NAME"].Value as string).StartsWith("MSys"))
                {
                    bool isView = false;
                    foreach (DataRow item in tab.Rows)
                    {
                        if (item["name"].Equals(rsTables.Fields["TABLE_NAME"].Value.ToString()))
                        {
                            isView = true;
                            break;
                        }
                    }
                    if (!isView)
                    {
                        DataRow dr = tab.NewRow();
                        dr["name"] = rsTables.Fields["TABLE_NAME"].Value.ToString();
                        tab.Rows.Add(dr);
                    }
                }
                rsTables.MoveNext();
            }
            Tables = (from c in tab.Rows.OfType<DataRow>() select new TVPSelect() { Ischeck = false, Tabname = c["name"].ToString() }).ToList();
            Views = (from c in viw.Rows.OfType<DataRow>() select new TVPSelect() { Ischeck = false, Tabname = c["name"].ToString() }).ToList();
            Sproc = new List<TVPSelect>();

            rsTables.Close();
            rsViews.Close();
            conn.Close();
        }
    }

    public class Oracle : SQLOutInfo
    {
        public override void Bind(string connstr)
        {
            //select TABLE_NAME from user_tables 
            //where (not table_name like '%$%') 
            //and TABLE_NAME <> 'SQLPLUS_PRODUCT_PROFILE' 
            //and TABLE_NAME <> 'HELP'    

            DBQuery<Sql.user_tables> query =
                DBFactory.CreateDBQuery<GeneratorV2.Sql.user_tables>(SqlType.Oracle, connstr);                    
                //new DBQuery<GeneratorV2.Sql.user_tables>(new Database(SqlType.Oracle, connstr));

            Tables = (from c in query.ToList<string>(query.CreateQuery().Where(!Sql.user_tablesMapping.table_name.Like("%$%") &&
                                                                        Sql.user_tablesMapping.table_name !=
                                                                        "SQLPLUS_PRODUCT_PROFILE" &&
                                                                        Sql.user_tablesMapping.table_name != "HELP")
                                                  .Select((Sql.user_tablesMapping.table_name).As("name")))
                      select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

            
            //.Select((Sql.user_tablesMapping.owner.Link(".").Link(Sql.user_tablesMapping.table_name).As("name"))).SortBy(new NSun.Data.ExpressionClip("\"name\"",DbType.String).Asc))
            //select VIEW_NAME from user_views 
            //where (not view_name like '%$%') 
            //and (not view_name like 'MVIEW_%') 
            //and (not view_name like 'CTX_%') 
            //and (not view_name = 'PRODUCT_PRIVS') 
            DBQuery<Sql.user_views> query2 =
                  DBFactory.CreateDBQuery<GeneratorV2.Sql.user_views>(SqlType.Oracle, connstr);     
                //new DBQuery<GeneratorV2.Sql.user_views>(new Database(SqlType.Oracle, connstr));

            Views = (from c in query2.ToList<string>(query2.CreateQuery().Where(!Sql.user_viewsMapping.view_name.Like("%$%") &&
                                                                       !Sql.user_viewsMapping.view_name.Like("MVIEW_%") &&
                                                                       !Sql.user_viewsMapping.view_name.Like("CTX_%") &&
                                                                       Sql.user_viewsMapping.view_name != "PRODUCT_PRIVS")
                                                 .Select(Sql.user_viewsMapping.view_name.As("name")))
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

            DBQuery<Sql.SYS_ALL_OBJECT> query3 =
                  DBFactory.CreateDBQuery<GeneratorV2.Sql.SYS_ALL_OBJECT>(SqlType.Oracle, connstr);    
            //    new DBQuery<Sql.SYS_ALL_OBJECT>(new Database(SqlType.Oracle, connstr));

            var select1 = query3.CreateQuery().Where(Sql.SYS_ALL_OBJECTMapping._object_type == "PROCEDURE" &&
                                        Sql.SYS_ALL_OBJECTMapping._owner != "FLOWS_020100" &&
                                        !Sql.SYS_ALL_OBJECTMapping._object_name.Like("%$%")).Select(Sql.SYS_ALL_OBJECTMapping._object_name.As("name"));
            Sproc = (from c in query3.ToList<string>(select1) select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

        }
    }

    public class MySql8 : SQLOutInfo
    {
        public override void Bind(string connstr)
        {
            DBQuery<Sql.information_schema_tables> query =
                  DBFactory.CreateDBQuery<GeneratorV2.Sql.information_schema_tables>(SqlType.MySql, connstr);
            //new DBQuery<Sql.information_schema_tables>(new Database(SqlType.MySql, connstr));
            DBQuery<Sql.TABLES> query2 =
                DBFactory.CreateDBQuery<GeneratorV2.Sql.TABLES>(SqlType.MySql, connstr);
            //new DBQuery<Sql.TABLES>(new Database(SqlType.MySql, connstr));
            
            DBQuery<Sql.mysql_Routines> query4 =
                DBFactory.CreateDBQuery<GeneratorV2.Sql.mysql_Routines>(SqlType.MySql, connstr);
            var baseName = query4.GetDbTransaction().Connection.Database;
            //SELECT table_name FROM information_schema.tables where table_type='BASE TABLE'
            Tables = (from c in query.ToList<string>(
                     query.CreateQuery().Where(Sql.information_schema_tablesMapping.table_type == "BASE TABLE"&& Sql.information_schema_tablesMapping.table_table_schema== baseName)
                     .Select(Sql.information_schema_tablesMapping.table_name.As("name"))
                 )
                      select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

            //select table_name from TABLES where TABLE_TYPE = 'VIEW'
            Views = (from c in query2.ToList<string>(
                     query2.CreateQuery().Where(Sql.TABLESMapping.table_type == "VIEW" && Sql.information_schema_tablesMapping.table_table_schema == baseName)
                     .Select(Sql.TABLESMapping.table_name.As("name"))
                 )
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();            
            Sproc = (from c in query4.ToList<string>(
                query4.CreateQuery().Where(Sql.mysql_RoutinesMapping.ROUTINE_TYPE == "PROCEDURE" && Sql.mysql_RoutinesMapping.ROUTINE_SCHEMA== baseName)
                .Select(Sql.mysql_RoutinesMapping.ROUTINE_NAME.As("name"))
            )
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();
        }
    }

    public class MySql : SQLOutInfo
    {
        public override void Bind(string connstr)
        {            
            DBQuery<Sql.information_schema_tables> query =
                 DBFactory.CreateDBQuery<GeneratorV2.Sql.information_schema_tables>(SqlType.MySql, connstr);  
                //new DBQuery<Sql.information_schema_tables>(new Database(SqlType.MySql, connstr));
            DBQuery<Sql.TABLES> query2 =
                DBFactory.CreateDBQuery<GeneratorV2.Sql.TABLES>(SqlType.MySql, connstr);
            //new DBQuery<Sql.TABLES>(new Database(SqlType.MySql, connstr));
             
            DBQuery<Sql.mysql_proc> query3 =
                DBFactory.CreateDBQuery<GeneratorV2.Sql.mysql_proc>(SqlType.MySql, connstr);

            //SELECT table_name FROM information_schema.tables where table_type='BASE TABLE'
            Tables = (from c in query.ToList<string>(
                     query.CreateQuery().Where(Sql.information_schema_tablesMapping.table_type == "BASE TABLE").Select(Sql.information_schema_tablesMapping.table_name.As("name"))
                 )
                      select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

            //select table_name from TABLES where TABLE_TYPE = 'VIEW'
            Views = (from c in query2.ToList<string>(
                     query2.CreateQuery().Where(Sql.TABLESMapping.table_type == "VIEW").Select(Sql.TABLESMapping.table_name.As("name"))
                 )
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

            Sproc = (from c in query3.ToList<string>(
                     query3.CreateQuery().Where(Sql.mysql_procMapping.type == "PROCEDURE").Select(Sql.mysql_procMapping.name.As("name"))
                 )
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

        }
    }

    public class Db2 : SQLOutInfo
    {

        public override void Bind(string connstr)
        {
            DBQuery<Sql.SYSCAT_TABLES> query =
                 DBFactory.CreateDBQuery<GeneratorV2.Sql.SYSCAT_TABLES>(SqlType.Db2, connstr);  
                //new DBQuery<Sql.SYSCAT_TABLES>(new Database(SqlType.Db2, connstr));

            Tables = (from c in query.ToList<string>(
                  query.CreateQuery().Where(Sql.SYSCAT_TABLESMapping.owner != "SYSTEM" && Sql.SYSCAT_TABLESMapping.ownertype == "U" && Sql.SYSCAT_TABLESMapping.type == "T").Select(Sql.SYSCAT_TABLESMapping.table_name.As("name"))
                  )
                      select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

            Views = (from c in query.ToList<string>(
                query.CreateQuery().Where(Sql.SYSCAT_TABLESMapping.owner != "SYSTEM" && Sql.SYSCAT_TABLESMapping.ownertype == "U" && Sql.SYSCAT_TABLESMapping.type == "V")
                  .Select(Sql.SYSCAT_TABLESMapping.table_name.As("name"))
                )
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();
            //SYSCAT_ROUTINES
            DBQuery<Sql.SYSCAT_ROUTINES> query2 =
                  DBFactory.CreateDBQuery<GeneratorV2.Sql.SYSCAT_ROUTINES>(SqlType.Db2, connstr); 
                //new DBQuery<Sql.SYSCAT_ROUTINES>(new Database(SqlType.Db2, connstr));
            Sproc = (from c in query2.ToList<string>(
               query2.CreateQuery().Where(Sql.SYSCAT_ROUTINESMapping._TEXT.StartsWith("CREATE PROCEDURE"))
                 .Select(Sql.SYSCAT_ROUTINESMapping._ROUTINENAME.As("name"))
               )
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();
            Sproc = null;

        }
    }

    public class Sqlite : SQLOutInfo
    {
        public override void Bind(string connstr)
        {            
            DBQuery<Sql.sqlite_master> query =
                DBFactory.CreateDBQuery<GeneratorV2.Sql.sqlite_master>(SqlType.Sqlite, connstr); 
                //new DBQuery<Sql.sqlite_master>(new Database(SqlType.Sqlite, connstr));
            List<string> systemTables = new List<string>();
            systemTables.Add("fulltext");
            systemTables.Add("fulltext_content");
            systemTables.Add("fulltext_term");
            systemTables.Add("keyinfotest");
            systemTables.Add("xp_proc");
            systemTables.Add("sqlite_master");
            systemTables.Add("sqlite_sequence");

            Tables = (from c in query.ToList<string>(
                 query.CreateQuery().Where(Sql.sqlite_masterMapping.type == "table").Select(Sql.sqlite_masterMapping.tbl_name.As("name"))
                 )
                      select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

            Views = (from c in query.ToList<string>(
                   query.CreateQuery().Where(Sql.sqlite_masterMapping.type == "view").Select(Sql.sqlite_masterMapping.tbl_name.As("name"))
                   )
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();
            List<string> listrow = new List<string>();
            foreach (TVPSelect systemTable in Tables)
            {
                if (systemTables.Contains(systemTable.Tabname.ToLower()))
                {
                    listrow.Add(systemTable.Tabname);
                }
            }
            foreach (var dataRow in listrow)
            {
                Tables.RemoveAll(p => p.Tabname == dataRow);
            }
            listrow.Clear();
            foreach (TVPSelect systemTable in Views)
            {
                if (systemTables.Contains(systemTable.Tabname.ToString().ToLower()))
                {
                    listrow.Add(systemTable.Tabname);

                }
            }
            foreach (var dataRow in listrow)
            {
                Views.RemoveAll(p => p.Tabname == dataRow);
            }
        }
    }

    public class PostgreSql : SQLOutInfo
    {

        public override void Bind(string connstr)
        {
            DBQuery<Sql.pg_tables> query =
                 DBFactory.CreateDBQuery<GeneratorV2.Sql.pg_tables>(SqlType.PostgreSql, connstr); 
                //new DBQuery<Sql.pg_tables>(new Database(SqlType.PostgreSql, connstr));
            DBQuery<Sql.pg_views> query2 =
                 DBFactory.CreateDBQuery<GeneratorV2.Sql.pg_views>(SqlType.PostgreSql, connstr); 
                //new DBQuery<Sql.pg_views>(new Database(SqlType.PostgreSql, connstr));

            Tables = (from c in query.ToList<string>(
                 query.CreateQuery().Where(Sql.pg_tablesMapping.schemaname == "public")
                 .Select(Sql.pg_tablesMapping.tablename.As("name"))
                 )
                      select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

            Views = (from c in query2.ToList<string>(
                 query2.CreateQuery().Where(Sql.pg_viewsMapping.schemaname == "public").Select(Sql.pg_viewsMapping.tablename.As("name"))
                 )
                     select new TVPSelect() { Ischeck = false, Tabname = c }).ToList();

        }
    }
}
