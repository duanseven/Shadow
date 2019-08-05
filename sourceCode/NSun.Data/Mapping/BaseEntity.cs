using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Castle.DynamicProxy;
using NSun.Data;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    public abstract class BaseEntity : NSun.Data.IBaseEntity
    {
        #region Persistence

        private bool _isPersistence;

        public bool IsPersistence()
        {
            return _isPersistence;
        }

        public void SetIsPersistence(bool ispersistence)
        {
            _isPersistence = ispersistence;
        }

        #endregion

        #region IQueryTable 成员

        public virtual string GetTableName()
        {
            return BindTableName();
        }

        private string BindTableName()
        {
            var tabattr = AttributeUtils.GetAttribute<TableAttribute>(this.GetType());
            if (tabattr == null)
            {
                var tablemapping = AttributeUtils.GetAttribute<TableMappingAttribute>(this.GetType());
                if (tablemapping != null)
                {
                    var tm = AttributeUtils.GetAttribute<TableAttribute>(tablemapping.MappingType) as TableAttribute;
                    if (tm != null)
                    {
                        return tm.TableName.ToDatabaseObjectName();
                    }
                }
                return GetTableName().ToDatabaseObjectName();
            }
            return tabattr.TableName.ToDatabaseObjectName();
        }

        #endregion

        #region Construction

        public BaseEntity()
            : this(Guid.Empty)
        { }

        public BaseEntity(Guid key)
        {
            if (IsTransient())
            {
                IdentityKey = IdentityGenerator.NewSequentialGuid();
                //State = EntityState.None;
                return;
            }
            IdentityKey = key;
        }

        #endregion

        //public EntityState State { get; set; }

        //public static BaseEntity Empty
        //{
        //    get
        //    {
        //        return new EmptyEntity();
        //    }
        //}

        [DataMember]
        public Guid IdentityKey { get; set; }

        private bool IsTransient()
        {
            return this.IdentityKey == Guid.Empty;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BaseEntity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            var item = (BaseEntity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.IdentityKey == this.IdentityKey;
        }

        public override int GetHashCode()
        {
            return this.IdentityKey.GetHashCode();
        }

        public static bool operator ==(BaseEntity left, BaseEntity right)
        {

            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
            {

                // type 1
                //var empty = right as EmptyEntity;
                //if (empty != null)
                //{
                //    return left.IdentityKey == empty.IdentityKey;
                //}
                // type 2
                //return left.IdentityKey == Guid.Empty;
                return left.Equals(right);
            }
        }

        public static bool operator !=(BaseEntity left, BaseEntity right)
        {
            return !(left == right);
        }

        //private class EmptyEntity : BaseEntity
        //{
        //    public override Guid IdentityKey
        //    {
        //        get { return Guid.Empty; }
        //        set { }
        //    }
        //}
    }

    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    public abstract class BaseEntityRefObject : MarshalByRefObject, NSun.Data.IBaseEntity
    {
        #region Persistence

        private bool _isPersistence;

        public bool IsPersistence()
        {
            return _isPersistence;
        }

        public void SetIsPersistence(bool ispersistence)
        {
            _isPersistence = ispersistence;
        }

        #endregion

        #region IQueryTable 成员

        public virtual string GetTableName()
        {
            return BindTableName();
        }

        private string BindTableName()
        {
            var tabattr = AttributeUtils.GetAttribute<TableAttribute>(this.GetType());
            if (tabattr == null)
            {
                var tablemapping = AttributeUtils.GetAttribute<TableMappingAttribute>(this.GetType());
                if (tablemapping != null)
                {
                    var tm = AttributeUtils.GetAttribute<TableAttribute>(tablemapping.MappingType) as TableAttribute;
                    if (tm != null)
                    {
                        return tm.TableName.ToDatabaseObjectName();
                    }
                }
                return GetTableName().ToDatabaseObjectName();
            }
            return tabattr.TableName.ToDatabaseObjectName();
        }

        #endregion

        #region Construction

        public BaseEntityRefObject()
            : this(Guid.Empty)
        { }

        public BaseEntityRefObject(Guid key)
        {
            if (IsTransient())
            {
                IdentityKey = IdentityGenerator.NewSequentialGuid();
                //State = EntityState.None;
                return;
            }
            IdentityKey = key;
        }

        #endregion

        //public EntityState State { get; set; }

        //public static BaseEntity Empty
        //{
        //    get
        //    {
        //        return new EmptyEntity();
        //    }
        //}

        [DataMember]
        public Guid IdentityKey { get; set; }

        private bool IsTransient()
        {
            return this.IdentityKey == Guid.Empty;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is BaseEntity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            var item = (BaseEntityRefObject)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.IdentityKey == this.IdentityKey;
        }

        public override int GetHashCode()
        {
            return this.IdentityKey.GetHashCode();
        }

        public static bool operator ==(BaseEntityRefObject left, BaseEntityRefObject right)
        {

            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
            {

                // type 1
                //var empty = right as EmptyEntity;
                //if (empty != null)
                //{
                //    return left.IdentityKey == empty.IdentityKey;
                //}
                // type 2
                //return left.IdentityKey == Guid.Empty;
                return left.Equals(right);
            }
        }

        public static bool operator !=(BaseEntityRefObject left, BaseEntityRefObject right)
        {
            return !(left == right);
        }

        //private class EmptyEntity : BaseEntity
        //{
        //    public override Guid IdentityKey
        //    {
        //        get { return Guid.Empty; }
        //        set { }
        //    }
        //}
    }
}
