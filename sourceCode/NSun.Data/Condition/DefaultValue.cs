using System;

namespace NSun.Data
{
    public class DefaultValue<T>
    {
        public static T Default
        {
            get { return CommonUtils.DefaultValue<T>(); }
        }
    }
}