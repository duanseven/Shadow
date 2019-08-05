using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSun.Data.Test.Domain
{
    #region Teach

    [Serializable]
    [TableMapping(typeof(TeachMapping))]
    public class Teach : BaseEntityRefObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pass { get; set; }

        public NSun.Data.Collection.List<Class> Classes { get; set; }
    }

    [Table("TEACH")]
    public class TeachMapping
    {
        static TeachMapping()
        {
            _id = new IdQueryColumn("TEACH.ID", System.Data.DbType.Int32, true);
            _name = new QueryColumn("TEACH.NAME", System.Data.DbType.String);
            _pass = new QueryColumn("TEACH.PASS", System.Data.DbType.String);

            _classes = new RelationQueryColumn(_id, typeof(ClassMapping), ClassMapping._teachid, RelationType.OneToMany,LoadType.Read);
        }

        public static IdQueryColumn _id;
        public static QueryColumn _name;
        public static QueryColumn _pass;

        //Relation
        private static RelationQueryColumn _classes;
    }

    #endregion

    #region Class

    [Serializable]
    [TableMapping(typeof(ClassMapping))]
    public class Class : BaseEntityRefObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Subject { get; set; }
        public int TeachId { get; set; }

        public Teach TeachInfo { get; set; }
    }

    [Table("CLASS")]
    public class ClassMapping
    {
        static ClassMapping()
        {
            _id = new IdQueryColumn("CLASS.ID", System.Data.DbType.Int32, true);
            _name = new QueryColumn("CLASS.NAME", System.Data.DbType.String);
            _subject = new QueryColumn("CLASS.SUBJECT", System.Data.DbType.String);
            _teachid = new QueryColumn("CLASS.TEACHID", System.Data.DbType.Int32);
            _teachinfo = new RelationQueryColumn(_teachid, typeof (TeachMapping), TeachMapping._id,
                                                 RelationType.ManyToOne, LoadType.Read);
        }

        public static IdQueryColumn _id;
        public static QueryColumn _name;
        public static QueryColumn _subject;
        public static QueryColumn _teachid;
        public static RelationQueryColumn _teachinfo;
    }

    #endregion

    #region RelationClassToStudent

    [Serializable]
    [TableMapping(typeof(RelationClassToStudentMapping))]
    public class RelationClassToStudent : BaseEntityRefObject
    {
        public int Id { get; set; }
        public int Studentid { get; set; }
        public int Classid { get; set; }
    }

    [Table("RELATIONCLASSTOSTUDENT")]
    public class RelationClassToStudentMapping
    {
        static RelationClassToStudentMapping()
        {
            _id = new IdQueryColumn("RELATIONCLASSTOSTUDENT.ID", System.Data.DbType.Int32, true);
            _studentid = new QueryColumn("RELATIONCLASSTOSTUDENT.STUDENTID", System.Data.DbType.Int32);
            _classid = new QueryColumn("RELATIONCLASSTOSTUDENT.CLASSID", System.Data.DbType.Int32);
        }

        public static IdQueryColumn _id;
        public static QueryColumn _studentid;
        public static QueryColumn _classid;
    }

    #endregion

    #region Student

    [TableMapping(typeof(StudentMapping))]
    public class Student : BaseEntityRefObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Pass { get; set; }
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
        public int Classid { get; set; }
        public Byte[] Stamp { get; set; }
    }

    [Table("STUDENT")]
    public class StudentMapping
    {
        static StudentMapping()
        {
            _id = new IdQueryColumn("STUDENT.ID", System.Data.DbType.Int32, true);
            _name = new QueryColumn("STUDENT.NAME", System.Data.DbType.String);
            _pass = new QueryColumn("STUDENT.PASS", System.Data.DbType.String);
            _birthday = new QueryColumn("STUDENT.BIRTHDAY", System.Data.DbType.DateTime);
            _age = new QueryColumn("STUDENT.AGE", System.Data.DbType.Int32);
            _classid = new QueryColumn("STUDENT.CLASSID", System.Data.DbType.Int32);
            _stamp = new VersionQueryColumn("STUDENT.STAMP", System.Data.DbType.Binary);
        }

        public static IdQueryColumn _id;
        public static QueryColumn _name;
        public static QueryColumn _pass;
        public static QueryColumn _birthday;
        public static QueryColumn _age;
        public static QueryColumn _classid;
        public static VersionQueryColumn _stamp;
    }

    #endregion

    #region Book

    [Serializable]
    [TableMapping(typeof(BookMapping))]
    public class Book : BaseEntityRefObject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    [Table("BOOK")]
    public class BookMapping
    {
        static BookMapping()
        {
            _id = new IdQueryColumn("BOOK.ID", System.Data.DbType.Int32, true);
            _name = new QueryColumn("BOOK.NAME", System.Data.DbType.String);
            _price = new QueryColumn("BOOK.PRICE", System.Data.DbType.Decimal);
        }

        public static IdQueryColumn _id;
        public static QueryColumn _name;
        public static QueryColumn _price;

    }

    #endregion

    #region StudentInfo

    [Serializable]
    [TableMapping(typeof(StudentInfoMapping))]
    public class StudentInfo : BaseEntityRefObject
    {
        public int Id { get; set; }
        public int Studentid { get; set; }
        public string Code { get; set; }
        public string Address { get; set; }
    }

    [Table("STUDENTINFO")]
    public class StudentInfoMapping
    {
        static StudentInfoMapping()
        {
            _id = new IdQueryColumn("STUDENTINFO.ID", System.Data.DbType.Int32, true);
            _studentid = new QueryColumn("STUDENTINFO.STUDENTID", System.Data.DbType.Int32);
            _code = new QueryColumn("STUDENTINFO.CODE", System.Data.DbType.String);
            _address = new QueryColumn("STUDENTINFO.ADDRESS", System.Data.DbType.String);
        }

        public static IdQueryColumn _id;
        public static QueryColumn _studentid;
        public static QueryColumn _code;
        public static QueryColumn _address;
    }

    #endregion
}
