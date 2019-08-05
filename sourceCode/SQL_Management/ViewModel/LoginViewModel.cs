using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using SQL_Management.Model;

namespace SQL_Management.ViewModel
{
    public class LoginViewModel : NotificationObject
    {
        #region Command

        public DelegateCommand ConnectionCommand { get; set; }

        public DelegateCommand CancelCommand { get; set; }

        #endregion

        #region Event

        public event Action<object> CancelHandler;

        public event Action<object> ConnectionHandler;

        #endregion

        #region Model

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

        private bool isCheckSave;

        public bool IsCheckSave
        {
            get { return isCheckSave; }
            set { isCheckSave = value; RaisePropertyChanged("IsCheckSave"); }
        }

        private BindingList<NSun.Data.SqlType> listSQLType;

        public BindingList<NSun.Data.SqlType> ListSQLType
        {
            get { return listSQLType; }
        }

        #endregion

        #region Handler

        public LoginViewModel()
        {
            ConnectionCommand = new DelegateCommand { ExecuteAction = this.Connection };
            CancelCommand = new DelegateCommand { ExecuteAction = Cancel };

            //New Model
            connInfoview = new ConnectionInfo();
            //listSqlType
            listSQLType =
                new BindingList<NSun.Data.SqlType>(
                    (IList<NSun.Data.SqlType>)
                    typeof(NSun.Data.SqlType).GetFields().Where(p => p.FieldType == typeof(NSun.Data.SqlType)).Select<System.Reflection.FieldInfo, NSun.Data.SqlType>(
                        p => (NSun.Data.SqlType)p.GetValue(null)).ToList());

        }

        private void Connection(object parameter)
        {
            if (ConnectionHandler != null)
            {
                var db = new NSun.Data.Database(ConnInfoView.SqlType, ConnInfoView.ConnectionString);
                try
                {
                    using (db.CreateConnection(true))
                    {
                        ConnectionHandler(true);
                    }
                }
                catch (Exception)
                {
                    ConnectionHandler(false);
                }
            }
            else
            {
                throw new Exception("未注册连接事件");
            }
        }

        private void Cancel(object parameter)
        {
            if (CancelHandler != null)
            {
                CancelHandler(parameter);
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        #endregion
    }
}
