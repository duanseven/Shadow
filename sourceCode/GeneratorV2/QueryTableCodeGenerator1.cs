using System;
using System.CodeDom;
using System.Data;
using NSun.Data;
using System.Runtime.Serialization;

namespace Generator
{
    /// <summary>
    /// QueryColumn  Mapping
    /// </summary>
    public class QueryTableCodeGenerator1 : CodeGeneratorBase
    {
        public bool IsReadOnly { get; private set; }
      
        public QueryTableCodeGenerator1(string outputFileName, string outputLang, string outputNamespace, DBQuery connStr, string name, bool isReadOnly)
            : base(outputFileName, outputLang, outputNamespace, connStr, name, false)
        {
            IsReadOnly = isReadOnly;
        }

        protected override void DoGenerate(CodeTypeDeclaration genClass)
        {
            //genClass.BaseTypes.Add(typeof(BaseEntity)); 
            //genClass.BaseTypes.Add(IsCastle ? typeof(BaseEntity) : typeof(BaseELEntity));

            genClass.Name = IsPartial ? genClass.Name : (OldName + "Table");

            var criteria = _cmdFac.CreateQuery(Name);
            var dt = _queryService.Query(criteria);
            
            var incr = _queryService.QuerySchemaIncrement(criteria);
            var pri = _queryService.QuerySchemaPrimary(criteria);

            //CodeTypeReference evin = new CodeTypeReference(typeof(PrimaryKeyAttribute));
            //CodeTypeReference datacol = new CodeTypeReference(typeof(DataMemberAttribute));
            //genClass.CustomAttributes.Add(new CodeAttributeDeclaration(datacol));

            CodeTypeReference tabatr = new CodeTypeReference(typeof(TableAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(tabatr, new CodeAttributeArgument(new CodePrimitiveExpression(Name)
                )));

            //静态构造函数
            CodeTypeConstructor ctyc = new CodeTypeConstructor();
            genClass.Members.Add(ctyc);

            foreach (DataColumn column in dt.Columns)
            {                
                //public static Column __id = new Column("USERS.ID", System.Data.DbType.Int32);
                var field = new CodeMemberField();
                field.Name = "_" + column.ColumnName.Replace(" ", string.Empty);
                field.Attributes = MemberAttributes.Public | MemberAttributes.Static;

                bool iskey = true;
                //添加主见列
                if (pri != null && pri.ColumnName.ToLower() == column.ColumnName.ToLower())
                {
                    if (incr != null && incr.ColumnName.ToLower() == column.ColumnName.ToLower())
                    {
                        field.Type = new CodeTypeReference(typeof(IdQueryColumn));
                    }
                    else
                    {
                        field.Type = new CodeTypeReference(typeof(IdQueryColumn));
                        iskey = false;
                    }
                }
                else if (column.Ordinal == 0)
                {
                    field.Type = new CodeTypeReference(typeof(IdQueryColumn));
                    iskey = false;
                }
                else
                {
                    field.Type = new CodeTypeReference(typeof(QueryColumn));
                    iskey = false;
                }
                string columnname = ViewMode.ConnInfoView.SQLType == SqlType.PostgreSql ? column.ColumnName : column.ColumnName.ToUpper();
                if (iskey)
                {
                    //field.InitExpression
                    CodeObjectCreateExpression v = new CodeObjectCreateExpression(
                        field.Type,
                        new CodeExpression[]
                            {
                                new CodePrimitiveExpression(Name + "." + columnname),
                                new CodeSnippetExpression(
                                    ConvertListUtil.GetDbType(column.DataType.Name)),
                                new CodePrimitiveExpression(true)
                            }
                        );
                    ctyc.Statements.Add(new CodeAssignStatement(
                                          new CodeVariableReferenceExpression(field.Name), v));
                }
                else
                {
                    CodeObjectCreateExpression v = new CodeObjectCreateExpression(
                        field.Type,
                        new CodeExpression[]
                            {
                                new CodePrimitiveExpression(Name + "." + columnname),
                                new CodeSnippetExpression(ConvertListUtil.GetDbType(column.DataType.Name))
                            }
                        );
                    ctyc.Statements.Add(new CodeAssignStatement(
                                            new CodeVariableReferenceExpression(field.Name), v));
                }
                genClass.Members.Add(field);

                //field.Type = new CodeTypeReference(typeof(QueryColumn));
                //string columnname = ViewMode.ConnInfoView.SQLType == SqlType.PostgreSql ? column.ColumnName : column.ColumnName.ToUpper();
                //field.InitExpression = new CodeObjectCreateExpression(
                //    field.Type,
                //    new CodeExpression[]
                //        {
                //            new CodePrimitiveExpression(Name + "." + columnname),
                //            new CodeSnippetExpression(ConvertListUtil.GetDbType(column.DataType.Name))
                //        }
                //    );
                //genClass.Members.Add(field); 
            }
        } 

    }
}
