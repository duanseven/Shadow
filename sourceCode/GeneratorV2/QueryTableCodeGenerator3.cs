using System;
using System.CodeDom;
using System.Data;
using NSun.Data;
using System.Runtime.Serialization;

namespace Generator
{
    /// <summary>
    /// QueryColumn
    /// </summary>
    public class QueryTableCodeGenerator3 : CodeGeneratorBase
    {
        public bool IsReadOnly { get; private set; }

        public QueryTableCodeGenerator3(string outputFileName, string outputLang, string outputNamespace, DBQuery connStr, string name, bool isReadOnly)
            : base(outputFileName, outputLang, outputNamespace, connStr, name)
        {
            IsReadOnly = isReadOnly;
        }

        protected override void DoGenerate(CodeTypeDeclaration genClass)
        {
            genClass.BaseTypes.Add(typeof(BaseEntity)); 
 
            var criteria = _cmdFac.CreateQuery(Name);
            var dt = _queryService.Query(criteria);
            var incr = _queryService.QuerySchemaIncrement(criteria);
            var pri = _queryService.QuerySchemaPrimary(criteria);
            CodeTypeReference evin = new CodeTypeReference(typeof(PrimaryKeyAttribute));

            CodeTypeReference datacol = new CodeTypeReference(typeof(DataMemberAttribute)); 
            
            foreach (DataColumn column in dt.Columns)
            {                
                //public static Column __id = new Column("USERS.ID", System.Data.DbType.Int32);
                var field = new CodeMemberField();
                field.Name = "_" + column.ColumnName.Replace(" ", string.Empty);
                field.Attributes = MemberAttributes.Public | MemberAttributes.Static;                 

                field.Type = new CodeTypeReference(typeof(QueryColumn));
                string columnname =ViewMode.ConnInfoView.SQLType == SqlType.PostgreSql ? column.ColumnName : column.ColumnName.ToUpper();
                field.InitExpression = new CodeObjectCreateExpression(
                    field.Type,
                    new CodeExpression[]
                        {
                            new CodePrimitiveExpression(Name + "." + columnname),
                            new CodeSnippetExpression(ConvertListUtil.GetDbType(column.DataType.Name))
                        }
                    );
                genClass.Members.Add(field); 
            }
        }  

    }
}
