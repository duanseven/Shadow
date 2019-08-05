using System;
using System.Data;
using NSun.Data;

namespace NSun.Data.Lambda
{
    public static class ColumnExpand
    {
        #region Function

        #region String

        public static bool Like(this string str, string likeStr) { return true; }

        public static bool Contains(this string str, string subString) { return true; }

        public static bool StartsWith(this string str, string prefix) { return true; }

        public static bool EndsWith(this string str, string suffix) { return true; }

        public static int Length(this string tag) { return tag.Length; }

        public static string ToUpper(this string tag) { return tag; }

        public static string ToLower(this string tag) { return tag; }

        public static string Trim(this string tag) { return tag; }

        public static string Substring(this string tag, int start) { return tag; }

        public static string Substring(this string tag, int start, int length) { return tag; }

        public static int IndexOf(this string tag, string subString) { return tag.IndexOf(subString); }

        public static string Replace(this string tag, string subString, string replaceString) { return tag; }

        public static int ToNumber(this string tag) { return Convert.ToInt32(tag); }


        #endregion

        #region DateTime

        public static object GetYear(this DateTime tag) { return null; }

        //public static object Datediff(this DateTime tag, DateTime time, ColumnFormatter.DatePartType yyyyMMdd) { return null; }

        public static object GetMonth(this DateTime tag) { return null; }

        public static object GetDay(this DateTime tag) { return null; }

        public static object GetCurrentDate(this DateTime tag) { return null; }

        public static object GetCurrentUtcDate(this DateTime tag) { return null; }

        public static object GetYear(this DateTime? tag) { return null; }

        //public static object Datediff(this DateTime? tag, DateTime time, ColumnFormatter.DatePartType yyyyMMdd) { return null; }

        public static object GetMonth(this DateTime? tag) { return null; }

        public static object GetDay(this DateTime? tag) { return null; }

        public static object GetCurrentDate(this DateTime? tag) { return null; }

        public static object GetCurrentUtcDate(this DateTime? tag) { return null; }

        #endregion

        #region Aggregation

        public static T Distinct<T>(this T tag) { return tag; }

        public static T Count<T>(this T tag) { return tag; }

        public static T Count<T>(this T tag, bool isDistinct) { return tag; }

        public static T Sum<T>(this T tag) { return tag; }

        public static T Min<T>(this T tag) { return tag; }

        public static T Max<T>(this T tag) { return tag; }

        public static T Avg<T>(this T tag) { return tag; }

        #endregion

        #region  Equals and Not Equals

        public static bool In(this object tag, params object[] objs) { return true; }

        //public static bool In(this object tag, ExpressionClip fun) { return true; }

        public static bool NotIn(this object tag, params object[] objs) { return true; }

        //public static bool NotIn(this object tag, ExpressionClip fun) { return true; }

        #endregion

        #region Between

        public static bool Between(this object tag, object left, object right) { return true; }

        #endregion

        #region Order

        public static OrderByClip Desc(this object tag)
        {
            return null;
        }

        public static OrderByClip Asc(this object tag)
        {
            return null;
        }

        #endregion

        #region SelfdboMethod

        public static object CustomMethodExtensions<T>(this T tag, DbType returndbType, bool isSysMethod, bool methodName, params object[] pars)
        {
            return tag;
        }

        public static object CustomMethodExtensions<T>(this T tag, DbType returndbType, string dboName, string methodName, params object[] pars)
        {
            return tag;
        }

        public static object CustomMethodExtensions<T>(this T tag, DbType returndbType, string dboName, string methodName, bool isColumninMethod, params object[] pars)
        {
            return tag;
        }

        #endregion

        #endregion
    }
}
