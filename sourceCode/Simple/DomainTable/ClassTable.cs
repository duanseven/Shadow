//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.17929
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Simple
{
    using System;
    using NSun;
    using NSun.Data;
    using NSun.Data.Collection;


    [NSun.Data.TableAttribute("CLASS")]
    public class ClassTable
    {

        static ClassTable()
        {
            _id = new NSun.Data.IdQueryColumn("CLASS.ID", System.Data.DbType.Int32, true);
            _name = new NSun.Data.QueryColumn("CLASS.NAME", System.Data.DbType.String);
            _studentList = new RelationQueryColumn(_id, typeof(StudentTable), StudentTable._id,
                                                             RelationType.OneToMany);
             

            _teacherList=new RelationQueryColumn(_id, typeof(RelationteacherclassTable), typeof(TeacherTable),
                                             RelationteacherclassTable._classid, //MAPPING
                                             TeacherTable._id, //MAPPINGID
                                             RelationteacherclassTable._teacherid, //OutMaPPING 
                                             RelationType.ManyToMany);
        }

        public static NSun.Data.IdQueryColumn _id;

        public static NSun.Data.QueryColumn _name;        

        public static NSun.Data.RelationQueryColumn _studentList;

        public static NSun.Data.RelationQueryColumn _teacherList;
        //public static NSun.Data.VersionQueryColumn 版本号
    }
}
