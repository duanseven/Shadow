﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace GeneratorV2
{
    public class DelegateCommand : ICommand
    { 
        #region ICommand 成员

        public bool CanExecute(object parameter)
        { 
            if (this.CanExecuteFunc == null)
            {
                return true;
            }
            return this.CanExecuteFunc(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        { 
            if (this.ExecuteAction == null)
            {                
                return;
            }
            this.ExecuteAction(parameter);
        }

        #endregion

        public Action<object> ExecuteAction { get; set; }
 

        public Func<object, bool> CanExecuteFunc { get; set; }
    }
}
