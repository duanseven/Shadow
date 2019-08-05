using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
//using SQL_Management.Service;
using SQL_Management.ViewModel;
using SQL_Management.Model;
using NSun.Data;

namespace SQL_Management
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        private LoginViewModel ViewModel;

        public DBQueryFactory Factory { get; set; }

        public Login()
        {
            ViewModel = new LoginViewModel();
            this.DataContext = ViewModel;
            ViewModel.CancelHandler += new Action<object>(ViewModel_CancelHandler);
            ViewModel.ConnectionHandler += new Action<object>(ViewModel_ConnectionHandler);
            InitializeComponent();
        }

        void ViewModel_ConnectionHandler(object obj)
        {
            if ((bool)obj)
            {
                Factory = DBFactory.CreateDBQueryFactory(ViewModel.ConnInfoView.SqlType, ViewModel.ConnInfoView.ConnectionString);
                this.Close();
            }
            else
            {
                MessageBox.Show("连接失败，请检查连接字符串是否正确");
            }
        }

        void ViewModel_CancelHandler(object obj)
        {
            this.Close();
        }
    }
}
