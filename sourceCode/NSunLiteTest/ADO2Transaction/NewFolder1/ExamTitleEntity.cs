
//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.239
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace NSunLiteTest.ADO2Transaction
{
    using System;
    using NSun;
    using NSun.Data; 


    [System.SerializableAttribute()]
    [NSun.Data.TableAttribute("EXAMTITLE")]
    public partial class ExamTitleEntity : NSun.Data.BaseEntity
    {

        public static NSun.Data.QueryColumn _id = new NSun.Data.QueryColumn("EXAMTITLE.ID", System.Data.DbType.Int32);

        private int id;

        [NSun.Data.PrimaryKeyAttribute(true)]
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

        public static NSun.Data.QueryColumn _title = new NSun.Data.QueryColumn("EXAMTITLE.TITLE", System.Data.DbType.String);

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                title = value;
            }
        }

        public static NSun.Data.QueryColumn _trueselect = new NSun.Data.QueryColumn("EXAMTITLE.TRUESELECT", System.Data.DbType.String);

        private string trueselect;

        public string Trueselect
        {
            get
            {
                return trueselect;
            }
            set
            {
                trueselect = value;
            }
        }

        public static NSun.Data.QueryColumn _typeid = new NSun.Data.QueryColumn("EXAMTITLE.TYPEID", System.Data.DbType.Int32);

        private System.Nullable<int> typeid;

        public System.Nullable<int> Typeid
        {
            get
            {
                return typeid;
            }
            set
            {
                typeid = value;
            }
        }

        public static NSun.Data.QueryColumn _tittype = new NSun.Data.QueryColumn("EXAMTITLE.TITTYPE", System.Data.DbType.Int32);

        private System.Nullable<int> tittype;

        public System.Nullable<int> Tittype
        {
            get
            {
                return tittype;
            }
            set
            {
                tittype = value;
            }
        }

        //public HashSetMapping<SelectItemEntity> SelectItems
        //{
        //    get
        //    {
        //        return new HashSetMapping<SelectItemEntity>(
        //             DBFactory.dbSelectItem, SelectItemEntity._titleid == Id);
        //    }
        //}

    }
}
