﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using NSun.Data.Configuration;


namespace NSun.Data
{
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public class InsertSqlSection : QueryCriteria, ICudSection
    {
        #region Property & Field
        [DataMember]
        internal bool IsAutoGenerated { get; set; }

        [DataMember]
        private readonly List<Assignment> _assignments;

        public ReadOnlyCollection<Assignment> Assignments
        {
            get { return new ReadOnlyCollection<Assignment>(_assignments); }
        }

        #endregion

        #region Construction

        public InsertSqlSection(string tablename)
            : base(tablename)
        {
            _assignments = new List<Assignment>();
            _queryType = QueryType.Insert;
        }

        internal InsertSqlSection(Database db, string tablename)
            : this(tablename)
        {
            Db = db;
        }

        #endregion

        #region Public Methods

        public void AddAssignment(Assignment ag)
        {
            _assignments.Add(ag);
        }

        public InsertSqlSection AddColumn(QueryColumn column, object value)
        {
            AddAssignment(new Assignment(column, new ParameterExpression(value, column.DataType)));
            return this;
        }

        public override object Clone()
        {
            var clone = new InsertSqlSection(Db, TableName);
            foreach (var assignment in Assignments)
            {
                clone._assignments.Add(assignment);
            }
            clone.IdentyColumnName = IdentyColumnName;
            clone.IdentyColumnIsNumber = IdentyColumnIsNumber;
            return clone;
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
