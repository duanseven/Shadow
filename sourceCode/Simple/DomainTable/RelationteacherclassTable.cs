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
    
    
    [NSun.Data.TableAttribute("RELATIONTEACHERCLASS")]
    public class RelationteacherclassTable
    {
        
        static RelationteacherclassTable()
        {
            _id = new NSun.Data.IdQueryColumn("RELATIONTEACHERCLASS.ID", System.Data.DbType.Int32, true);
            _teacherid = new NSun.Data.QueryColumn("RELATIONTEACHERCLASS.TEACHERID", System.Data.DbType.Int32);
            _classid = new NSun.Data.QueryColumn("RELATIONTEACHERCLASS.CLASSID", System.Data.DbType.Int32);
        }
        
        public static NSun.Data.IdQueryColumn _id;
        
        public static NSun.Data.QueryColumn _teacherid;
        
        public static NSun.Data.QueryColumn _classid;
    }
}
