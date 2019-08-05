using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using NSun.Data;
using GeneratorV2.ViewModel;
using GeneratorV2.Service;
using GeneratorV2.Commons;

namespace Generator
{
    public class MappingGenrator
    {
        private Database db;

        public MappingGenrator(Database _db)
        {
            db = _db;
        }

        public MainWindowViewModel ViewMode { get; set; }

        public void RelationBegin(StringBuilder sb, string space)
        {
            sb.Append("using System;\r\n");
            sb.Append("using " + space + ";\r\n");
            sb.Append("using NSun.Data;\r\n\r\n");
            sb.Append("namespace ");
            sb.Append(space);
            sb.Append("\r\n{\r\n");
        }

        public void RelationEnd(StringBuilder sb, string path)
        {
            sb.Append("}");
            File.WriteAllText(path + @"\ExtensionMethods.cs", sb.ToString());
        }

        public StringBuilder GetTable(string Suffix, IEnumerable<string> list)
        {
            StringBuilder sb = new StringBuilder();
            string classsuffix = "ExtensionMethods";

            foreach (var itemss in list)
            {
                string item = itemss.ToString();
                if (ViewMode.ConnInfoView.SQLType == NSun.Data.SqlType.Sqlserver9 || ViewMode.ConnInfoView.SQLType == NSun.Data.SqlType.Sqlserver10)
                {
                    if (item.Split('.').Length > 1)
                    {
                        item = item.Split('.')[1];
                    }
                }
                sb.AppendFormat("\tpublic static class {0}\r\n", Util.GetUpper(item.ToString()) + classsuffix);
                sb.Append("\t{\r\n");
                SqlCommand com = new SqlCommand("select id,name from sysobjects where xtype='u' and name=@name", (SqlConnection)db.GetConnection());
                com.Parameters.Add(new SqlParameter("@name", item.ToString()));
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                da.Fill(dt);
                DataRow dr = dt.Rows[0];
                DataSet ds = GetPKRK(dr["id"].ToString());
                DataTable rkdt = ds.Tables["one"];
                DataTable pkdt = ds.Tables["many"];
                sb.Append(RkCodeTableV3(Suffix, item.ToString(), rkdt));
                sb.Append(PkCodeTable(Suffix, item.ToString(), pkdt));
                sb.Append("\t}\r\n");
            }

            return sb;
        }

        public DataSet GetPKRK(string spc)
        {
            //获得所有列  
            DataSet ds = new DataSet();
            //外表指本表字段 
            string str = "select DISTINCT (SELECT name from syscolumns where id=fkeyid and colid=fkey1) AS RkCol,(select name from syscolumns where id=rkeyid and colid=rkeyindid) PkCol,o.name as Relatable from sysreferences r,sysobjects o ,syscolumns s where o.id=r.fkeyid AND rkeyid=@id";
            SqlCommand cmdrk = new SqlCommand(str,  (SqlConnection)db.GetConnection());
            cmdrk.Parameters.Add(new SqlParameter("@id", spc));
            SqlDataAdapter rkda = new SqlDataAdapter(cmdrk);
            DataTable rkdt = new DataTable("one");
            rkda.Fill(rkdt);
            ds.Tables.Add(rkdt);
            //本表指外表字段
            str = "select DISTINCT (SELECT name from syscolumns where id=fkeyid and colid=fkey1) AS RkCol,(select name from syscolumns where id=rkeyid and colid=rkeyindid) PkCol,o.name as Relatable from sysreferences r,sysobjects o ,syscolumns s where o.id=r.rkeyid AND fkeyid=@id";
            cmdrk.CommandText = str;
            DataTable pkdt = new DataTable("many");
            rkda.Fill(pkdt);
            ds.Tables.Add(pkdt);
            return ds;
        }

        public StringBuilder PkCodeTable(string suffix, string tab, DataTable dt)
        {
            string table = tab;
            if (ViewMode.ConnInfoView.SQLType == NSun.Data.SqlType.Sqlserver9 || ViewMode.ConnInfoView.SQLType == NSun.Data.SqlType.Sqlserver10)
            {
                if (table.Split('.').Length > 1)
                {
                    table = table.Split('.')[1];
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (DataRow item in dt.Rows)
            {
                sb.Append("\t\tpublic static " + Util.GetUpper(item["Relatable"].ToString()) + suffix + " " + Util.GetUpper(item["Relatable"].ToString()) + "(this " + Util.GetUpper(table) + suffix + " info){\r\n");
                sb.Append("\t\t\treturn DBFactoryNew.Instance.CreateDBQuery<" + Util.GetUpper(item["Relatable"].ToString()) + suffix + ">().Load(info." + Util.GetUpper(item["RkCol"].ToString()) + ");\r\n\t\t}\r\n");
            }
            return sb;
        }

        public StringBuilder RkCodeTableV3(string suffix, string tab, DataTable dt)
        {
            string table = tab;
            if (ViewMode.ConnInfoView.SQLType == NSun.Data.SqlType.Sqlserver9 || ViewMode.ConnInfoView.SQLType == NSun.Data.SqlType.Sqlserver10)
            {
                if (table.Split('.').Length > 1)
                {
                    table = table.Split('.')[1];
                }
            }
            StringBuilder sb = new StringBuilder();
            foreach (DataRow item in dt.Rows)
            {
                sb.Append("\t\tpublic static List<" + Util.GetUpper(item["Relatable"].ToString()) + suffix + "> " + Util.GetUpper(item["Relatable"].ToString()) + "s(this " + Util.GetUpper(table) + suffix + " info){\r\n");
                sb.Append("\t\t\treturn  DBFactoryNew.Instance.CreateDBQuery<" + Util.GetUpper(item["Relatable"].ToString()) + suffix + ">().CreateQuery().Where(j1=> j1." + Util.GetUpper(item["RkCol"].ToString()) + " == info." + Util.GetUpper(item["PkCol"].ToString()) + ").ToList();\r\n\t\t}\r\n");
            }
            return sb;
        }

    }
}
