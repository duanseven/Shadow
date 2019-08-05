using System;
using System.Runtime.Serialization;
using NSun.Data.Configuration;


namespace NSun.Data
{
    //Sproc
    [Serializable]
    [DataContract(Namespace = "http://nsun-shadow.com")]
    [KnownType("KnownTypes")]
    public abstract class SprocEntitys : QueryCriteria, IParameterConditions, IQueryTable
    {
        #region Properties & Filed

        [DataMember]
        private StoredProcedureSection _storedProcedureSection;

        [DataMember]
        private bool isInitParameter = false;

        public string Sql
        {
            get { return TableName; }
            set { TableName = value; }
        }

        #endregion

        #region Construction

        public SprocEntitys()
        {
            _storedProcedureSection = new StoredProcedureSection(GetTableName());
            Sql = _storedProcedureSection.Sql;
            _queryType = QueryType.Sproc;
        } 

        #endregion

        #region GetProcName

        private string BindTableName()
        {
            var tabattr = AttributeUtils.GetAttribute<ProcedureAttribute>(this.GetType());
            return tabattr != null
                       ? tabattr.ProcedureName.ToDatabaseObjectName()
                       : GetTableName().ToDatabaseObjectName();
        }

        public virtual string GetTableName()
        {
            return BindTableName();
        }

        #endregion

        #region Public Methods

        public void InitialParameter()
        {
            isInitParameter = true;
            var propers = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var propertyInfo in propers)
            {
                object value = propertyInfo.GetValue(this, null);
                var inputpam = AttributeUtils.GetAttribute<InputParameterAttribute>(propertyInfo);
                if (inputpam != null)
                {
                    _storedProcedureSection.AddInputParameter(inputpam.ParameterName, inputpam.DbType, value);
                    continue;
                }
                var outputpam = AttributeUtils.GetAttribute<OutputParameterAttribute>(propertyInfo);
                if (outputpam != null)
                {
                    _storedProcedureSection.AddOutputParameter(outputpam.ParameterName, outputpam.DbType, outputpam.Size);
                    continue;
                }
                var inputoutputpam = AttributeUtils.GetAttribute<InputOutputParameterAttribute>(propertyInfo);
                if (inputoutputpam != null)
                {
                    _storedProcedureSection.AddInputOutputParameter(inputoutputpam.ParameterName, inputoutputpam.DbType, value, inputoutputpam.Size);
                    continue;
                }
                var returnpam = AttributeUtils.GetAttribute<ReturnParameterAttribute>(propertyInfo);
                if (returnpam != null)
                {
                    _storedProcedureSection.AddReturnValueParameter(returnpam.ParameterName, returnpam.DbType, returnpam.Size);
                    continue;
                }
                throw new Exception("parameter type error");
            }
        }

        public System.Collections.ObjectModel.ReadOnlyCollection<ParameterEqualsCondition> ParameterConditions
        {
            get
            {
                if (!isInitParameter)
                    InitialParameter();
                return _storedProcedureSection.ParameterConditions;
            }
        }

        internal void BindOutProperty(System.Data.Common.DbCommand sc)
        {
            System.Collections.Generic.Dictionary<string, object> dis =
                _storedProcedureSection.GetOutputParameterValues(sc);
            InitialOutParameter(dis);
        }

        public void InitialOutParameter(System.Collections.Generic.Dictionary<string, object> dis)
        {
            var propers = GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.DeclaredOnly);

            foreach (var propertyInfo in propers)
            {
                var pam = string.Empty;
                var outputpam = AttributeUtils.GetAttribute<OutputParameterAttribute>(propertyInfo);
                if (outputpam != null)
                {
                    pam = outputpam.ParameterName;
                }
                var inputoutputpam = AttributeUtils.GetAttribute<InputOutputParameterAttribute>(propertyInfo);
                if (inputoutputpam != null)
                {
                    pam = inputoutputpam.ParameterName;
                }
                var returnpam = AttributeUtils.GetAttribute<ReturnParameterAttribute>(propertyInfo);
                if (returnpam != null)
                {
                    pam = returnpam.ParameterName;
                }
                if (dis.ContainsKey(pam))
                {
                    propertyInfo.SetValue(this, dis[pam], null);
                }
            }
        }

        public override object Clone()
        {
            return _storedProcedureSection.Clone();
        }

        public override string ToDbCommandText()
        {
            if (!isInitParameter)
                InitialParameter();
            return _storedProcedureSection.ToDbCommandText();
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