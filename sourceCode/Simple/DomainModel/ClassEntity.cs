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
    
    
    [System.SerializableAttribute()]
    [System.Runtime.Serialization.DataContractAttribute()]
    [NSun.Data.TableMappingAttribute("Simple,Simple.ClassTable")]
    public class ClassEntity : NSun.Data.BaseEntityRefObject
    {
        
        private int id;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Id
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
            }
        }
        
        private string name;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private NSun.Data.Collection.List<StudentEntity> studentList;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public NSun.Data.Collection.List<StudentEntity> StudentList
        {
            get { return studentList; }
            set { studentList = value; }
        }

        private NSun.Data.Collection.List<TeacherEntity> teacherList;

        [System.Runtime.Serialization.DataMemberAttribute()]
        public NSun.Data.Collection.List<TeacherEntity> TeacherList
        {
            get { return teacherList; }
            set { teacherList = value; }
        }
    }
}
