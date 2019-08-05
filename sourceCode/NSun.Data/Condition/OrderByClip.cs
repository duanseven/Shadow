using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class OrderByClip
    {
        #region Member

        [DataMember]
        private KeyValuePair<ExpressionClip, bool> orderBys;

        public KeyValuePair<ExpressionClip, bool> Orderby
        {
            get { return orderBys; }
        }

        #endregion

        #region Construction

        public OrderByClip(ExpressionClip item, bool descend)
        {
            orderBys = new KeyValuePair<ExpressionClip, bool>(item, descend);
        }
          
        internal OrderByClip()
        {

        }

        #endregion

        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion
    }
}
