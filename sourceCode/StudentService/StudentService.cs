using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemotingModel;

namespace StudentService
{
    public class StudentService : IStudentService.IStudentService
    {
        #region IStudentService 成员

        public IList<RemotingModel.StudentRemotingModel> GetStudent(NSun.Data.QueryCriteria criteria)
        {
            var db = DBFactory.CreateDBQuery<StudentRemotingModel>();
            if (criteria != null)
            {
                return db.ToList(criteria);
            }
            return db.ToList();
        }

        #endregion

        public int Save(StudentRemotingModel student)
        {
            var db = DBFactory.CreateDBQuery<StudentRemotingModel>();
            return db.Save(student);
        }
    }
}
