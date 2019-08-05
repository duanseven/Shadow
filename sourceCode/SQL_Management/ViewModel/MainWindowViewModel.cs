using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQL_Management.Model;
using NSun.Data;

namespace SQL_Management.ViewModel
{
    public class MainWindowViewModel : NotificationObject
    {
        private DBQueryFactory factory;

        public bool IsLogin { get; set; }

        public MainWindowViewModel()
        {

        }

        public void LoadLogin(object parameter)
        {
            var login = new Login();
            login.ShowDialog();
            factory = login.Factory;
            IsLogin = null != factory;
            if (IsLogin)
            {
                TreeData = new List<string>()
                               {
                                   "1",
                                   "2",
                                   "3",
                                   "4",
                                   "5"
                               };
            }
        }

        private List<string> treeData;

        public List<string> TreeData
        {
            get
            {
                return treeData;
            }
            set
            {
                treeData = value;
                RaisePropertyChanged("TreeData");
            }
        }
    }
}
