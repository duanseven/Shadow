//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.269
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConsoleApplication1
{
    using System;
    using NSun.Data;
    
    
    [System.SerializableAttribute()]
    [NSun.Data.TableAttribute("STU")]    
    public partial class StuEntity : NSun.Data.BaseEntity
    {
        
        public static NSun.Data.QueryColumn _id = new NSun.Data.QueryColumn("STU.ID", System.Data.DbType.Int32);
        
        private int id;
        
        [PrimaryKeyAttribute(true)] 
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
        
        public static NSun.Data.QueryColumn _name = new NSun.Data.QueryColumn("STU.NAME", System.Data.DbType.String);
        
        private string name;
         
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
        
        public static NSun.Data.QueryColumn _pass = new NSun.Data.QueryColumn("STU.PASS", System.Data.DbType.String);
        
        private string pass;
         
        public string Pass
        {
            get
            {
                return pass;
            }
            set
            {
                pass = value;
            }
        }
        
        public static NSun.Data.QueryColumn _age = new NSun.Data.QueryColumn("STU.AGE", System.Data.DbType.Int32);
        
        private System.Nullable<int> age;
         
        public System.Nullable<int> Age
        {
            get
            {
                return age;
            }
            set
            {
                age = value;
            }
        }
    }
}
