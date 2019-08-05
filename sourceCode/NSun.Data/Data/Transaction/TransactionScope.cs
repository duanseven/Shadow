using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace NSun.Data.Transactions
{
    public class TransactionScope : IDisposable
    {
        private Transaction transaction = Transaction.Current;

        public bool Completed { get; private set; }

        public TransactionScope(Database db)
            : this(db, IsolationLevel.Unspecified)
        {

        }

        public TransactionScope(DBQuery db)
            : this(db, IsolationLevel.Unspecified)
        {
        }

        public TransactionScope(DBQuery db, IsolationLevel isolationLevel)
        {
            if (null == transaction)
            {
                if (null == db.Transaction)
                {
                    DbTransaction dbTransaction = db.GetDbTransaction();//.Db.BeginTransaction(isolationLevel);
                    Transaction.Current = new CommittableTransaction(dbTransaction);
                }
                else
                {
                    DbTransaction dbTransaction = db.Transaction;
                    Transaction.Current = new CommittableTransaction(dbTransaction);
                }
            }
            else
            {
                Transaction.Current = transaction.DependentClone();
            }
        }

        public TransactionScope(Database db, IsolationLevel isolationLevel)
        {
            if (null == transaction)
            {
                DbTransaction dbTransaction = db.BeginTransaction(isolationLevel);
                Transaction.Current = new CommittableTransaction(dbTransaction);
            }
            else
            {
                Transaction.Current = transaction.DependentClone();
            }
        }

        public void Complete()
        {
            this.Completed = true;
        }

        public void Dispose()
        {
            Transaction current = Transaction.Current;
            Transaction.Current = transaction;
            if (!this.Completed)
            {
                current.Rollback();
            }
            CommittableTransaction committableTransaction = current as CommittableTransaction;
            if (null != committableTransaction)
            {
                if (this.Completed)
                {
                    committableTransaction.Commit();
                }
                committableTransaction.Dispose();
            }
        }
    }
}
