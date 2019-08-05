using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace IStudentService
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IStudentService
    {
        [OperationContract]
        IList<RemotingModel.StudentRemotingModel> GetStudent(NSun.Data.QueryCriteria criteria);

        [OperationContract]
        int Save(RemotingModel.StudentRemotingModel student);
    }
}
