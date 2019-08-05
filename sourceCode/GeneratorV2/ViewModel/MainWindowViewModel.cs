using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeneratorV2.Model;
using System.ComponentModel;
using GeneratorV2.Service;
using GeneratorV2.Commons;
using System.Windows.Forms;
using Generator;
using NSun.Data;

namespace GeneratorV2.ViewModel
{
    public class MainWindowViewModel : NotificationObject
    {
        #region Command

        public DelegateCommand BrowseCommand { get; set; }

        public DelegateCommand SearchCommand { get; set; }

        public DelegateCommand ConnectCommand { get; set; }

        public DelegateCommand CheckAllCommand { get; set; }

        public DelegateCommand GeneratorCommand { get; set; }

        #endregion

        #region Construction
         
        public MainWindowViewModel()
        {
            connInfoview = new ConnectionInfo();
            fileInfoView = new FileInfo();
            searchView = new SearchInfo();

            ConnInfoView.ConnectionString = Util.GetValue("ConnectionString");

            BindType();

            this.BrowseCommand = new DelegateCommand();
            this.BrowseCommand.ExecuteAction = new Action<object>(this.Browse);

            this.SearchCommand = new DelegateCommand();
            this.SearchCommand.ExecuteAction = new Action<object>(this.Search);

            this.ConnectCommand = new DelegateCommand();             
            this.ConnectCommand.ExecuteAction = new Action<object>(this.Connect);             

            this.CheckAllCommand = new DelegateCommand();
            this.CheckAllCommand.ExecuteAction = new Action<object>(this.CheckAll);

            this.GeneratorCommand = new DelegateCommand();
            this.GeneratorCommand.ExecuteAction = new Action<object>(this.Generator);
        }

        #endregion

        #region CommandRealize

        private void Browse(object parameter)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog1.Description = "请选择保存的文件夹";
            folderBrowserDialog1.ShowNewFolderButton = true;
            System.Windows.Forms.DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string folderName = folderBrowserDialog1.SelectedPath;
                if (folderName != "")
                {
                    FileInfoView.OutPutPath = folderName;
                }
            }
        }

        private void Connect(object parameter)
        {
            Service = new Services(ConnInfoView.SQLType, ConnInfoView.ConnectionString);
            SqlOutInfo = Service.GetBindList();
            AllSqlOutInfo = SqlOutInfo.Clone() as SQLOutInfo;
        }

        private void Search(object parameter)
        {
            if (SearchView.TableName.Length > 0)
            {
                SqlOutInfo.Tables = SqlOutInfo.Tables.Where(p => p.Tabname.Contains(SearchView.TableName)).ToList();
            }
            else
            {
                SqlOutInfo.Tables = AllSqlOutInfo.Tables;
            }
            if (SearchView.ViewName.Length > 0)
            {
                SqlOutInfo.Views = SqlOutInfo.Views.Where(p => p.Tabname.Contains(SearchView.ViewName)).ToList();
            }
            else
            {
                SqlOutInfo.Views = AllSqlOutInfo.Views;
            }
        }

        private void CheckAll(object parameter)
        {
            if (SqlOutInfo.Tables.Count > 0)
                SqlOutInfo.Tables.ForEach(p => p.Ischeck = IsCheckAll);
            if (SqlOutInfo.Views.Count > 0)
                SqlOutInfo.Views.ForEach(p => p.Ischeck = IsCheckAll);
            if (SqlOutInfo.Sproc.Count > 0)
                SqlOutInfo.Sproc.ForEach(p => p.Ischeck = IsCheckAll);
        }

        private void Generator(object parameter)
        {
            if (FileInfoView.OutPutPath.Length == 0 || (SqlOutInfo.Views.Where(p => p.Ischeck).Count() == 0 && SqlOutInfo.Tables.Where(p => p.Ischeck).Count() == 0 && SqlOutInfo.Sproc.Where(p => p.Ischeck).Count() == 0))
            {
                MessageBox.Show("请输入路径和要生成的表!");
                return;
            }
            List<object> list = new List<object>();
            //table
            foreach (string item in SqlOutInfo.Tables.Where(p => p.Ischeck).Select(p => p.Tabname))
            {
                var s = item.Split('.');
                string tablename = s.Length > 1 ? s[1] : s[0];

                list.Add(tablename);
                if (IsTable)
                {
                    IsPartial = false;
                    //TableMapping
                     new QueryTableCodeGenerator1(
                           Util.GetOutputFileName(FileInfoView.OutPutPath, Util.GetUpper(tablename) + "Table", "C#"),
                         "C#",
                        FileInfoView.FileNameSpace,
                        DBFactory.CreateDBQuery(Service.SQLType, Service.Connstr),                         
                         Util.GetUpper(tablename),
                         false) { ViewMode = this, IsPartial = IsPartial }.Generate();
                    //Table
                    new QueryTableCodeGenerator2(
                         Util.GetOutputFileName(FileInfoView.OutPutPath, Util.GetUpper(tablename) + FileInfoView.FileNameSuffix, "C#"),
                        "C#",
                       FileInfoView.FileNameSpace,
                       DBFactory.CreateDBQuery(Service.SQLType, Service.Connstr),
                        Util.GetUpper(tablename),
                        false, IsMapping) { ViewMode = this, AssemblyName = this.AssemblyName, IsPartial = IsPartial }.Generate();
                }
                else
                {
                    new QueryTableCodeGenerator(
                          Util.GetOutputFileName(FileInfoView.OutPutPath, Util.GetUpper(tablename) + FileInfoView.FileNameSuffix, "C#"),
                        "C#",
                        FileInfoView.FileNameSpace,
                    DBFactory.CreateDBQuery(Service.SQLType, Service.Connstr),
                        Util.GetUpper(tablename),
                        false, IsMapping) { ViewMode = this, IsPartial = IsPartial }.Generate();
                }
            }
            //view
            foreach (string item in SqlOutInfo.Views.Where(p => p.Ischeck).Select(p => p.Tabname))
            {

                var s = item.Split('.');
                string viewname = s.Length > 1 ? s[1] : s[0];

                list.Add(viewname);
                if (IsPartial)
                {
                    new QueryTableCodeGenerator1(
                      Util.GetOutputFileName(FileInfoView.OutPutPath, Util.GetUpper(viewname) + "Table", "C#"),
                       "C#",
                      FileInfoView.FileNameSpace,
                     DBFactory.CreateDBQuery(Service.SQLType, Service.Connstr),
                       Util.GetUpper(viewname),
                       false) { ViewMode = this, IsPartial = IsPartial }.Generate();
                    new QueryTableCodeGenerator2(
                        Util.GetOutputFileName(FileInfoView.OutPutPath,
                                               Util.GetUpper(viewname) + FileInfoView.FileNameSuffix, "C#"),
                        "C#",
                        FileInfoView.FileNameSpace,
                        DBFactory.CreateDBQuery(Service.SQLType, Service.Connstr),
                        Util.GetUpper(viewname),
                        false, IsMapping) { ViewMode = this, AssemblyName = this.AssemblyName, IsPartial = IsPartial }.Generate();
                }
                else
                {
                    new QueryTableCodeGenerator(
                        Util.GetOutputFileName(FileInfoView.OutPutPath, Util.GetUpper(viewname) + FileInfoView.FileNameSuffix, "C#"),
                        "C#",
                        FileInfoView.FileNameSpace,
                    DBFactory.CreateDBQuery(Service.SQLType, Service.Connstr),
                        Util.GetUpper(viewname),
                        false, IsMapping) { ViewMode = this, IsPartial = IsPartial }.Generate();
                }
            }
            //sproc
            foreach (string item in SqlOutInfo.Sproc.Where(p => p.Ischeck).Select(p => p.Tabname))
            {
                var s = item.Split('.');
                string sprocname = s.Length > 1 ? s[1] : s[0];

                //list.Add(sprocname);
                new QuerySprocCodeGenerator(
                    Util.GetOutputFileName(FileInfoView.OutPutPath, Util.GetUpper(sprocname) + "Sproc", "C#"),
                    "C#",
                  FileInfoView.FileNameSpace,
                 DBFactory.CreateDBQuery(Service.SQLType, Service.Connstr),
                    Util.GetUpper(sprocname)) { ViewMode = this }.Generate();                
            }                        
            Util.DBFacotry(FileInfoView.OutPutPath, FileInfoView.FileNameSpace);
            Util.SetConfig("ConnectionString", ConnInfoView.ConnectionString);
            MessageBox.Show("Generator Done!");
        }

        #endregion

        #region OneWayBind

        private void BindType()
        {
            listSQLType = new BindingList<NSun.Data.SqlType>();
            listSQLType.Add(NSun.Data.SqlType.Sqlserver8);
            listSQLType.Add(NSun.Data.SqlType.Sqlserver9);
            listSQLType.Add(NSun.Data.SqlType.Sqlserver10);
            listSQLType.Add(NSun.Data.SqlType.MsAccess);
            listSQLType.Add(NSun.Data.SqlType.Db2);
            listSQLType.Add(NSun.Data.SqlType.MySql);
            listSQLType.Add(NSun.Data.SqlType.MySql8);
            listSQLType.Add(NSun.Data.SqlType.Oracle);
            listSQLType.Add(NSun.Data.SqlType.Oracle9);            
            listSQLType.Add(NSun.Data.SqlType.PostgreSql);
            listSQLType.Add(NSun.Data.SqlType.Sqlite);
        }

        #endregion

        #region Property

        private Services Service { get; set; }

        private BindingList<NSun.Data.SqlType> listSQLType;

        public BindingList<NSun.Data.SqlType> ListSQLType
        {
            get { return listSQLType; }
        }

        public SQLOutInfo AllSqlOutInfo { get; set; } 

        private SQLOutInfo sqlOutInfo;

        public SQLOutInfo SqlOutInfo
        {
            get { return sqlOutInfo; }
            set
            {
                sqlOutInfo = value;
                RaisePropertyChanged("SqlOutInfo");
            }
        }

        private ConnectionInfo connInfoview;

        public ConnectionInfo ConnInfoView
        {
            get { return connInfoview; }
            set
            {
                connInfoview = value;
                RaisePropertyChanged("ConnInfoView");
            }
        }

        private FileInfo fileInfoView;

        public FileInfo FileInfoView
        {
            get { return fileInfoView; }
            set
            {
                fileInfoView = value;
                RaisePropertyChanged("FileInfoView");
            }
        }

        private SearchInfo searchView;

        public SearchInfo SearchView
        {
            get { return searchView; }
            set
            {
                searchView = value;
                RaisePropertyChanged("SearchView");
            }
        }

        private bool isMapping;

        public bool IsMapping
        {
            get { return isMapping; }
            set { isMapping = value; RaisePropertyChanged("IsMapping"); }
        }

        private bool isTable;

        public bool IsTable
        {
            get { return isTable; }
            set { isTable = value; }
        }

        private bool ispartial;

        public bool IsPartial
        {
            get { return ispartial; }
            set { ispartial = value; RaisePropertyChanged("IsPartial"); }
        }

        private bool ischeckall;

        public bool IsCheckAll
        {
            get { return ischeckall; }
            set { ischeckall = value; }
        }

        private string assemblyName;

        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; RaisePropertyChanged("AssemblyName"); }
        }
        #endregion
    }
}
