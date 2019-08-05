using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using NSun.Data.Configuration;

namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class FromClip
    {
        #region KnownTypes

        static Type[] KnownTypes()
        {
            return KnownTypeRegistry.Instance.KnownTypes;
        }

        #endregion

        #region Protected Members

        [DataMember]
        protected readonly string tableOrViewName;

        [DataMember]
        protected readonly string aliasName;

        [DataMember]
        protected readonly Dictionary<string, KeyValuePair<string, Condition>> joins = new Dictionary<string, KeyValuePair<string, Condition>>();

        [DataMember]
        protected readonly List<JoinType> joinTypes = new List<JoinType>();

        [DataMember]
        internal bool isCostomTable { get; set; }

        [DataMember]
        internal bool IsOnlock { get; set; }

        #endregion

        #region Properties

        public string TableOrViewName
        {
            get
            {
                return tableOrViewName;
            }
        }

        public string AliasName
        {
            get
            {
                return aliasName;
            }
        }

        public Dictionary<string, KeyValuePair<string, Condition>> Joins
        {
            get
            {
                return joins;
            }
        }

        public List<JoinType> JoinTypes
        {
            get
            {
                return joinTypes;
            }
        }

        #endregion

        #region Constructors

        public FromClip(string tableOrViewName, string aliasName)
        {
            this.tableOrViewName = tableOrViewName;
            this.aliasName = aliasName;
        }

        public FromClip(string tableOrViewName)
            : this(tableOrViewName, tableOrViewName)
        {
        }

        #endregion

        #region Public Members

        public FromClip Join(string tableOrViewName, Condition onWhere)
        {
            return Join(tableOrViewName, tableOrViewName, onWhere);
        }

        public FromClip Join(string tableOrViewName, string aliasName, Condition onWhere)
        {
            if (joins.ContainsKey(aliasName))
            {
                throw new Exception("In joins list: aliasName - " + aliasName);
            }

            joins.Add(aliasName, new KeyValuePair<string, Condition>(tableOrViewName, onWhere));
            joinTypes.Add(JoinType.Inner);

            return this;
        }

        public FromClip LeftJoin(string tableOrViewName, Condition onWhere)
        {
            return Join(tableOrViewName, tableOrViewName, onWhere);
        }

        public FromClip LeftJoin(string tableOrViewName, string aliasName, Condition onWhere)
        {
            if (joins.ContainsKey(aliasName))
            {
                throw new Exception("In joins list: aliasName - " + aliasName);
            }

            joins.Add(aliasName, new KeyValuePair<string, Condition>(tableOrViewName, onWhere));
            joinTypes.Add(JoinType.Left);

            return this;
        }

        public FromClip RightJoin(string tableOrViewName, Condition onWhere)
        {
            return Join(tableOrViewName, tableOrViewName, onWhere);
        }

        public FromClip RightJoin(string tableOrViewName, string aliasName, Condition onWhere)
        {
            if (joins.ContainsKey(aliasName))
            {
                throw new Exception("In joins list: aliasName - " + aliasName);
            }

            joins.Add(aliasName, new KeyValuePair<string, Condition>(tableOrViewName, onWhere));
            joinTypes.Add(JoinType.Right);

            return this;
        }

        public FromClip FullJoin(string tableOrViewName, Condition onWhere)
        {
            return Join(tableOrViewName, tableOrViewName, onWhere);
        }

        public FromClip FullJoin(string tableOrViewName, string aliasName, Condition onWhere)
        {
            if (joins.ContainsKey(aliasName))
            {
                throw new Exception("In joins list: aliasName - " + aliasName);
            }

            joins.Add(aliasName, new KeyValuePair<string, Condition>(tableOrViewName, onWhere));
            joinTypes.Add(JoinType.Full);

            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!tableOrViewName.Contains("[") && !isCostomTable)
            {
                sb.Append('[');
                sb.Append(tableOrViewName);
                sb.Append(']');
            }
            else
                sb.Append(tableOrViewName);

            if (aliasName != tableOrViewName)
            {
                sb.Append(' ');
                sb.Append('[');
                sb.Append(aliasName.TrimStart('[').TrimEnd(']'));
                sb.Append(']');
            }

            if (IsOnlock)
                sb.Append(" (NOLOCK)");

            int i = 0;
            foreach (string joinAliasName in joins.Keys)
            {
                string joinTypeStr;
                if (joinTypes[i] == JoinType.Inner)
                    joinTypeStr = "INNER JOIN";
                else if (joinTypes[i] == JoinType.Left)
                    joinTypeStr = "LEFT OUTER JOIN";
                else if (joinTypes[i] == JoinType.Right)
                    joinTypeStr = "RIGHT OUTER JOIN";
                else if (joinTypes[i] == JoinType.Full)
                    joinTypeStr = "FULL OUTER JOIN";
                else
                    joinTypeStr = "INNER JOIN";

                if (sb.ToString().Contains(joinTypeStr))
                {
                    sb = new StringBuilder('(' + sb.ToString() + ')');
                }

                KeyValuePair<string, Condition> keyWhere = joins[joinAliasName];
                sb.Append(' ');
                sb.Append(joinTypeStr);
                sb.Append(' ');
                if (keyWhere.Key.IndexOf("select", StringComparison.OrdinalIgnoreCase) == -1)
                {
                    if (!keyWhere.Key.Contains("["))
                    {
                        sb.Append('[');
                        sb.Append(keyWhere.Key);
                        sb.Append(']');
                    }
                    else
                        sb.Append(keyWhere.Key);
                }
                else
                {
                    if (!keyWhere.Key.Contains("("))
                    {
                        sb.Append('(');
                        sb.Append(keyWhere.Key);
                        sb.Append(')');
                    }
                    else
                        sb.Append(keyWhere.Key);
                }

                if (joinAliasName != keyWhere.Key)
                {
                    sb.Append(' ');
                    if (!joinAliasName.Contains("["))
                    {
                        sb.Append('[');
                        sb.Append(joinAliasName);
                        sb.Append(']');
                    }
                    else
                        sb.Append(joinAliasName);
                }
                sb.Append(" ON ");

                sb.Append(ExpressionHelper.ToConditionCacheableSql(keyWhere.Value));

                ++i;
            }

            return sb.ToString();
        }

        #endregion
    }
}
