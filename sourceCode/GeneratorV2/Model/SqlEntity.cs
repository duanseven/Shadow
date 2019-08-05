using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSun.Data;

namespace GeneratorV2.Sql
{
    #region sysobjects

    [TableMapping(typeof(sysobjectsMapping))]
    public class sysobjects : BaseEntity
    {
        public string Name { get; set; }
        public int Uid { get; set; }
        public string Xtype { get; set; }
        public int Status { get; set; } 
    }

    [Table("sysobjects")]
    public class sysobjectsMapping
    {
        public static IdQueryColumn name = new IdQueryColumn("sysobjects.name", System.Data.DbType.String,false);
        public static QueryColumn uid = new QueryColumn("sysobjects.uid", System.Data.DbType.String);
        public static QueryColumn xtype = new QueryColumn("sysobjects.xtype", System.Data.DbType.String);
        public static QueryColumn status = new QueryColumn("sysobjects.status", System.Data.DbType.Int32);
    }

    #endregion

    #region schemas

    [TableMapping(typeof(schemasMapping))]
    public class schemas : BaseEntity
    {        
        public string Name { get; set; }
        public int SCHEMA_id { get; set; }      
    }

    [Table("sys.schemas")]
    public class schemasMapping
    {
        public static IdQueryColumn name = new IdQueryColumn("sys.schemas.name", System.Data.DbType.String);
        public static QueryColumn schema_id = new QueryColumn("sys.schemas.SCHEMA_id", System.Data.DbType.String);
    }

    #endregion

    #region user_tables

    [TableMapping(typeof(user_tablesMapping))]
    public class user_tables : BaseEntity
    {
        public string Owner { get; set; }
        public string Table_Name { get; set; }        
    }

    [Table("USER_TABLES")]
    public class user_tablesMapping
    {
        public static QueryColumn owner = new QueryColumn("USER_TABLES.OWNER", System.Data.DbType.String);
        public static IdQueryColumn table_name = new IdQueryColumn("USER_TABLES.TABLE_NAME", System.Data.DbType.String);
    }

    #endregion

    #region user_views

    [TableMapping(typeof(user_viewsMapping))]
    public class user_views : BaseEntity
    {        
        public string View_Name { get; set; }       
    }

    [Table("USER_VIEWS")]
    public class user_viewsMapping
    {
        public static IdQueryColumn view_name = new IdQueryColumn("USER_VIEWS.VIEW_NAME", System.Data.DbType.String); 
    }

    #endregion

    #region information_schema_tables
    
    [TableMapping(typeof(information_schema_tablesMapping))]
    public class information_schema_tables : BaseEntity
    { 
        public string Table_Name { get; set; }
        public string Table_Type { get; set; }
        public string Table_TABLE_SCHEMA { get; set; }
    }

    [Table("information_schema.tables")]
    public class information_schema_tablesMapping
    {
        public static IdQueryColumn table_name = new IdQueryColumn("information_schema.tables.table_name", System.Data.DbType.String);
        public static QueryColumn table_type = new QueryColumn("information_schema.tables.table_type", System.Data.DbType.String);
        public static QueryColumn table_table_schema = new QueryColumn("information_schema.tables.table_schema", System.Data.DbType.String);
    }

    #endregion

    #region TABLES

    [TableMapping(typeof(TABLESMapping))]
    public class TABLES : BaseEntity
    { 
        public string Table_Name { get; set; }
        public string TABLE_TYPE { get; set; }
        public string Table_TABLE_SCHEMA { get; set; }
    }

    [Table("information_schema.TABLES")]
    public class TABLESMapping
    {
        public static IdQueryColumn table_name = new IdQueryColumn("information_schema.TABLES.table_name", System.Data.DbType.String);
        public static QueryColumn table_type = new QueryColumn("information_schema.tables.table_type", System.Data.DbType.String);
        public static QueryColumn table_table_schema = new QueryColumn("information_schema.tables.table_schema", System.Data.DbType.String);
    }

    #endregion

    #region SYSCAT_TABLES
    
    //SELECT tabname FROM SYSCAT.TABLES  WHERE OWNERTYPE = 'U' AND OWNER <> 'SYSTEM' AND TYPE = 'T' 
    //SELECT tabname FROM SYSCAT.TABLES WHERE OWNERTYPE = 'U' AND OWNER <> 'SYSTEM' AND TYPE = 'V'
    [TableMapping(typeof(SYSCAT_TABLESMapping))]
    public class SYSCAT_TABLES : BaseEntity
    {
        public string tabname { get; set; }
        public string OWNERTYPE { get; set; }
        public string OWNER { get; set; }
        public string TYPE { get; set; }
    }

    [Table("SYSCAT.TABLES")]
    public class SYSCAT_TABLESMapping
    {
        public static IdQueryColumn table_name = new IdQueryColumn("SYSCAT.TABLES.TABNAME", System.Data.DbType.String);
        public static QueryColumn ownertype = new QueryColumn("SYSCAT.TABLES.OWNERTYPE", System.Data.DbType.String);
        public static QueryColumn owner = new QueryColumn("SYSCAT.TABLES.OWNER", System.Data.DbType.String);
        public static QueryColumn type = new QueryColumn("SYSCAT.TABLES.TYPE", System.Data.DbType.String);
    }

    #endregion

    #region pg_tables
   
    //select tablename from pg_tables where schemaname = 'public'
    //select viewname from pg_views where schemaname = 'public'
    [TableMapping(typeof(pg_tablesMapping))]
    public class pg_tables : BaseEntity
    { 
        public string Tablename { get; set; }
        public string Schemaname { get; set; } 
    }

    [Table("pg_tables")]
    public class pg_tablesMapping
    {
        public static IdQueryColumn tablename = new IdQueryColumn("pg_tables.tablename", System.Data.DbType.String);
        public static QueryColumn schemaname = new QueryColumn("pg_tables.schemaname", System.Data.DbType.String);
    }

    #endregion

    #region pg_views

    [TableMapping(typeof(pg_viewsMapping))]
    public class pg_views : BaseEntity
    { 
        public string Viewname { get; set; }
        public string Schemaname { get; set; } 
    }

    [Table("pg_views")]
    public class pg_viewsMapping
    {
        public static IdQueryColumn tablename = new IdQueryColumn("pg_views.viewname", System.Data.DbType.String);
        public static QueryColumn schemaname = new QueryColumn("pg_views.schemaname", System.Data.DbType.String);
    }

    #endregion

    #region sqlite_master

    //select tbl_name from sqlite_master where type = 'table'
    [TableMapping(typeof(sqlite_masterMapping))]
    public class sqlite_master : BaseEntity
    {
        public string Tbl_name { get; set; }
        public string Type { get; set; }
    }
    [Table("sqlite_master")]
    public class sqlite_masterMapping
    {
        public static IdQueryColumn tbl_name = new IdQueryColumn("sqlite_master.tbl_name", System.Data.DbType.String);
        public static QueryColumn type = new QueryColumn("sqlite_master.type", System.Data.DbType.String);
    }

    #endregion

    #region mysql_proc

    [TableMapping(typeof(mysql_procMapping))]
    public class mysql_proc : BaseEntity
    {
        //PROCEDURE
        public string Db { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }        
    }

    [Table("mysql.proc")]
    public class mysql_procMapping
    {
        public static QueryColumn db = new QueryColumn("db", System.Data.DbType.String);
        public static QueryColumn name = new QueryColumn("name", System.Data.DbType.String);
        public static QueryColumn type = new QueryColumn("Type", System.Data.DbType.String);
    }

    #endregion

    #region MySql8

    [TableMapping(typeof(mysql_RoutinesMapping))]
    public class mysql_Routines : BaseEntity
    {
        //PROCEDURE
        public string ROUTINE_SCHEMA { get; set; }
        public string ROUTINE_NAME { get; set; }
        public string ROUTINE_TYPE { get; set; }
    }

    [Table("information_schema.ROUTINES")]
    public class mysql_RoutinesMapping
    {
        public static QueryColumn ROUTINE_SCHEMA = new QueryColumn("ROUTINE_SCHEMA", System.Data.DbType.String);
        public static QueryColumn ROUTINE_NAME = new QueryColumn("ROUTINE_NAME", System.Data.DbType.String);
        public static QueryColumn ROUTINE_TYPE = new QueryColumn("ROUTINE_TYPE", System.Data.DbType.String);
    }

    #endregion

    #region SYS_ALL_OBJECT

    [TableMapping(typeof(SYS_ALL_OBJECTMapping))]
    public class SYS_ALL_OBJECT : BaseEntity
    {
        //PROCEDURE
        public string Owner { get; set; } 
        public string Object_name { get; set; } 
        public string Object_type { get; set; } 
    }

    [Table("SYS.ALL_OBJECTS")]
    public class SYS_ALL_OBJECTMapping
    {
        public static QueryColumn _owner = new QueryColumn("OWNER", System.Data.DbType.String);
        public static QueryColumn _object_name = new QueryColumn("OBJECT_NAME", System.Data.DbType.String);
        public static QueryColumn _object_type = new QueryColumn("OBJECT_TYPE", System.Data.DbType.String);
    }

    #endregion

    #region SYSCAT_ROUTINES

    //SELECT CHAR(ROUTINESCHEMA,20) AS SCHEMA, CHAR(ROUTINENAME,20) AS NAME FROM SYSCAT.ROUTINES WHERE SUBSTR(VARCHAR(TEXT),1,16) = 'CREATE PROCEDURE' 
    [TableMapping(typeof(SYSCAT_ROUTINESMapping))]
    public class SYSCAT_ROUTINES : BaseEntity
    {
        //PROCEDURE
        public string ROUTINENAME { get; set; } 
        public string ROUTINESCHEMA { get; set; } 
        public string TEXT { get; set; } 
    }

    [Table("SYSCAT.ROUTINES")]
    public class SYSCAT_ROUTINESMapping
    {
        public static QueryColumn _ROUTINENAME = new QueryColumn("ROUTINENAME", System.Data.DbType.String);
        public static QueryColumn _ROUTINESCHEMA = new QueryColumn("ROUTINESCHEMA", System.Data.DbType.String);
        public static QueryColumn _TEXT = new QueryColumn("TEXT", System.Data.DbType.String);
    }

    #endregion
}
