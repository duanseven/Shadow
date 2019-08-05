using System;
using NSun.Data;
using NSunLiteTest.ADO2Transaction;
using NSun;

namespace NSunLiteTest.ADO2Transaction
{
	public static class DBFactory
	{
		public readonly static Database db;
        private readonly static Database dbcopy;
	    private static readonly Database dbexam;
	    private static readonly Database dbaccess;
		static DBFactory()
		{
		    db = new Database("db", SqlType.Sqlserver9);
            dbcopy = new Database("dbcopy", SqlType.Sqlserver9);
		    dbexam = new Database("dbexam", SqlType.Sqlserver9);
		    dbaccess = new Database("db3", SqlType.MsAccess);

            //dbUsers =new DBQuery<UsersEntity>(db);
            //dbUs = new DBQuery<UsEntity>(dbcopy);

            //dbSelectItem = new DBQuery<SelectItemEntity>(dbexam);
            //dbExamTitle = new DBQuery<ExamTitleEntity>(dbexam);

            //dbmsaccess = new DBQuery<UsersInfo>(dbaccess);
        }
        public readonly static DBQuery<SelectItemEntity> dbSelectItem;
        public readonly static DBQuery<ExamTitleEntity> dbExamTitle;

		public readonly static DBQuery<UsersEntity> dbUsers;
		public readonly static DBQuery<UsEntity> dbUs;

        public readonly static DBQuery<UsersInfo> dbmsaccess;
	}
}