using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RemotingModel;
using NSun.Data;

namespace IServiceSchool
{
    public interface IBookService
    {
        IList<BookRemotingModel> GetBooks(NSun.Data.QueryCriteria criteria);
    }

    public interface IStudentService
    {
        StudentRemotingModel GetStudent(object key);
        IList<StudentRemotingModel> GetStudents(NSun.Data.QueryCriteria criteria);

        int Save(StudentRemotingModel entity);
    }
}
