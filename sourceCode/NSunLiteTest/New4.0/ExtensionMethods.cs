using System; 
using System.Collections.Generic;
using NSun.Data;
using NSunLiteTest;

namespace f
{
	public static class StudentExtensionMethods
	{
		public static PhoneEntity Phone(this StudentEntity info){
			return DBFactoryNew.Instance.CreateDBQuery<PhoneEntity>().Load(info.Phoneid);
		}
		public static SchoolEntity School(this StudentEntity info){
			return DBFactoryNew.Instance.CreateDBQuery<SchoolEntity>().Load(info.Schoolid);
		}
	}
	public static class SchoolExtensionMethods
	{
		public static List<StudentEntity> Students(this SchoolEntity info)
		{
		    return null;// DBFactoryNew.Instance.CreateDBQuery<StudentEntity>().CreateQuery().Where(j1 => j1.Schoolid == info.Id).ToList();
		}
	}
	public static class PhoneExtensionMethods
	{
		public static List<StudentEntity> Students(this PhoneEntity info){
            return
                 null;//return  DBFactoryNew.Instance.CreateDBQuery<StudentEntity>().CreateQuery().Where(j1=> j1.Phoneid == info.Id).ToList();
		}
	}
}