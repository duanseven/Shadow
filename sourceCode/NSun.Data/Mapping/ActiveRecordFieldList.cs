using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NSun.Data
{

    /// <summary>
    /// The ActiveRecordFieldList class.
    /// </summary>
    public sealed class ActiveRecordFieldList
    {
        internal readonly List<KeyValuePair<string, QueryColumn>> Fields = new List<KeyValuePair<string, QueryColumn>>();

        /// <summary>
        /// Adds the specified field name.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="mappingColumn">The mapping column.</param>
        /// <returns></returns>
        public ActiveRecordFieldList Add(string fieldName, QueryColumn mappingColumn)
        {
            Fields.Add(new KeyValuePair<string, QueryColumn>(fieldName, mappingColumn));

            return this;
        }

        /// <summary>
        /// Removes the specified field name.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
        public ActiveRecordFieldList Remove(string fieldName)
        {
            foreach (KeyValuePair<string, QueryColumn> field in Fields)
            {
                if (field.Key == fieldName)
                {
                    Fields.Remove(field);
                    return this;
                }
            }

            return this;
        }

        /// <summary>
        /// Filters the specified fields.
        /// </summary>
        /// <param name="baseList">The base list.</param>
        /// <param name="specifiedFieldsToFilter">The specified fields to filter.</param>
        /// <returns></returns>
        public static ActiveRecordFieldList FilterSpecifiedFields(ActiveRecordFieldList baseList, string[] specifiedFieldsToFilter)
        { 
            ActiveRecordFieldList list = new ActiveRecordFieldList();
            if (specifiedFieldsToFilter != null && specifiedFieldsToFilter.Length > 0)
            {
                List<string> specifiedFieldsToCreateList = CommonUtils.ToObjectList<string>(specifiedFieldsToFilter.GetEnumerator());
                foreach (KeyValuePair<string, QueryColumn> field in baseList.Fields)
                {
                    if (specifiedFieldsToCreateList.Contains(field.Key))
                        list.Fields.Add(field);
                }
            }
            else
                list = baseList;

            return list;
        }

    }
}
