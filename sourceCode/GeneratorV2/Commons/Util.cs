using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Collections;

namespace GeneratorV2.Commons
{
    public class ListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable)
            {
                return string.Join(Environment.NewLine, ((IEnumerable)value).OfType<string>().ToArray());
            }
            return "no messages yet";
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    } 
    public class Util
    {
        public static string GetUpper(string val)
        {
            if (val.Length > 0)
            {
                return val.Substring(0, 1).ToUpper() + val.Substring(1);
            }
            return val;
        }

        public static string GetLower(string val)
        {
            if (val.Length > 0)
            {
                return val.Substring(0, 1).ToLower() + val.Substring(1);
            }
            return val;
        }


        public static string GetOutputFileName(string path,string name, string outputLang)
        {
            return Path.Combine(path, name + (outputLang == "C#" ? ".cs" : ".vb"));
        }

        public static string GetValue(string AppKey)
        {
            try
            {
                string AppKeyValue;
                AppKeyValue = ConfigurationManager.AppSettings[AppKey];
                return AppKeyValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SetConfig(string key, string value)
        {
            System.Configuration.Configuration config =
System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (config.AppSettings.Settings[key] != null)
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void DBFacotry(string path, string suffix, string space)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using NSun.Data;\r\n");
            sb.Append("namespace ");
            sb.Append(space);
            sb.Append("\r\n{\r\n");
            sb.Append("\tpublic class DBFactory : DbFactoryAbst\r\n");
            sb.Append("\t{\r\n");
            sb.Append("\t\tprivate static readonly DBFactory _instance = new DBFactory();\r\n");
            sb.Append("\t\tpublic static DBFactory Instance\r\n");
            sb.Append("\t\t{\r\n");
            sb.Append("\t\t\tget { return _instance; }\r\n");
            sb.Append("\t\t}\r\n");
            sb.Append("\t\tprivate DBFactory() { }\r\n\r\n");
            sb.Append("\t\tprivate static Database db;\r\n\r\n");

            sb.Append("\t\tstatic DBFactory()\r\n");
            sb.Append("\t\t{\r\n");
            sb.Append("\t\t\tdb = Database.Default;\r\n");
            sb.Append("\t\t}\r\n");
            sb.Append("\t\tpublic override Database GetDatabase()\r\n");
            sb.Append("\t\t{\r\n");
            sb.Append("\t\t\treturn db;\r\n");
            sb.Append("\t\t}\r\n");
            sb.Append("\t}\r\n");
            sb.Append("}");
            File.WriteAllText(path + @"\DBFactory.cs", sb.ToString());
        }

        public static void DBFacotry(string path, string space)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using NSun.Data;\r\n");
            sb.Append("namespace ");
            sb.Append(space);
            sb.Append("\r\n{\r\n");
            sb.Append("\tpublic class DBFactory\r\n");
            sb.Append("\t{\r\n");
                sb.Append("\t\tprivate static readonly DBQueryFactory _instance;\r\n\r\n");
                sb.Append("\t\tpublic static DBQueryFactory Instance\r\n");
                sb.Append("\t\t{\r\n");
                    sb.Append("\t\t\tget { return _instance; }\r\n");    
                sb.Append("\t\t}\r\n\r\n");
                sb.Append("\t\tstatic DBFactory()\r\n");
                sb.Append("\t\t{\r\n");
                    sb.Append("\t\t\t_instance = new DBQueryFactory(\"db\", SqlType.Sqlserver9);\r\n");
                sb.Append("\t\t}\r\n\r\n");
                sb.Append("\t\tpublic static DBQuery<T> CreateDBQuery<T>() where T :class, IBaseEntity\r\n");
                sb.Append("\t\t{\r\n");
                    sb.Append("\t\t\treturn Instance.CreateDBQuery<T>();\r\n");
                sb.Append("\t\t}\r\n\r\n");
                sb.Append("\t\tpublic static DBQuery CreateDBQuery()\r\n");
                sb.Append("\t\t{\r\n");
                    sb.Append("\t\t\treturn Instance.CreateDBQuery();\r\n");
                sb.Append("\t\t}\r\n");
                sb.Append("\t}\r\n");
            sb.Append("}\r\n");
            File.WriteAllText(path + @"\DBFactory.cs", sb.ToString());
        }

        public static void DBFactory(string path, string suffix, string space, List<object> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;\r\n");
            sb.Append("using NSun.Data;\r\n");
            sb.Append("using " + space + ";\r\n");
            sb.Append("using NSun;\r\n\r\n");
            sb.Append("namespace ");
            sb.Append(space);
            sb.Append("\r\n{\r\n");
            sb.Append("\tpublic static class DBFactory\r\n");
            sb.Append("\t{\r\n");
            sb.Append("\t\tprivate readonly static Database db;\r\n");
            sb.Append("\t\tstatic DBFactory()\r\n");
            sb.Append("\t\t{\r\n");
            sb.Append("\t\t\tdb = Database.Default;\r\n");
            foreach (string table in list)
            {
                sb.Append("\t\t\tdb");
                sb.Append(GetUpper(table));
                sb.Append(" =new DBQuery<");
                sb.Append(GetUpper(table) + suffix);
                sb.Append(">(db);\r\n");
            }
            sb.Append("\t\t}\r\n");
            foreach (string table in list)
            {
                sb.Append("\t\tpublic readonly static ");
                sb.Append("DBQuery<");
                sb.Append(GetUpper(table) + suffix);
                sb.Append("> db");
                sb.Append(GetUpper(table));
                sb.Append(";\r\n");
            }
            sb.Append("\t}\r\n");
            sb.Append("}");
            File.WriteAllText(path + @"\DBFactory.cs", sb.ToString());
        } 
    }
}
