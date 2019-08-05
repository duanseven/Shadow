using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IServiceSchool;
using RemotingModel;

namespace ServiceShool
{
    public class BookService : MarshalByRefObject, IBookService
    {
        #region IBookService 成员

        public IList<BookRemotingModel> GetBooks(NSun.Data.QueryCriteria criteria)
        {
            var db = DBFactory.CreateDBQuery<BookRemotingModel>();
            if (criteria == null)
            {
                return db.ToList();
            }
            return db.ToList(criteria);
        }

        #endregion
    }

    public class StudentService : MarshalByRefObject, IStudentService
    {
        public IList<StudentRemotingModel> GetStudents(NSun.Data.QueryCriteria criteria)
        {
            var db = DBFactory.CreateDBQuery<StudentRemotingModel>();
            if (criteria != null)
            {
                db.ToList(criteria);
            }
            return db.ToList();
        }

        public StudentRemotingModel GetStudent(object key)
        {
            var db = DBFactory.CreateDBQuery<StudentRemotingModel>();
            var c= db.Load(key);
            return c;
        }

        public int Save(StudentRemotingModel entity)
        {
            var db = DBFactory.CreateDBQuery<StudentRemotingModel>();
            return db.Save(entity);
        }
    }

}
