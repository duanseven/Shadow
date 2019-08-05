using System;
using System.CodeDom;
using System.Data;
using NSun.Data;
using System.Runtime.Serialization;
using GeneratorV2.Commons;

namespace Generator
{
    public class QueryTableCodeGenerator4 : CodeGeneratorBase
    {
        public bool IsReadOnly { get; private set; }

        public QueryTableCodeGenerator4(string outputFileName, string outputLang, string outputNamespace, DBQuery connStr, string name, bool isReadOnly)
            : base(outputFileName, outputLang, outputNamespace, connStr, name)
        {
            IsReadOnly = isReadOnly;
        }

        protected override void DoGenerate(CodeTypeDeclaration genClass)
        {
            genClass.BaseTypes.Add(typeof(BaseEntity));

            //Serializable
            CodeTypeReference seri = new CodeTypeReference(typeof(SerializableAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(seri));
            //
            CodeTypeReference tabatr = new CodeTypeReference(typeof(TableAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(tabatr, new CodeAttributeArgument(new CodePrimitiveExpression(Name)
                )));

            CodeTypeReference dataatr = new CodeTypeReference(typeof(DataContractAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(dataatr));


            var criteria = _cmdFac.CreateQuery(Name);
            var dt = _queryService.Query(criteria);
            var incr = _queryService.QuerySchemaIncrement(criteria);
            var pri = _queryService.QuerySchemaPrimary(criteria);
            CodeTypeReference evin = new CodeTypeReference(typeof(PrimaryKeyAttribute));

            CodeTypeReference datacol = new CodeTypeReference(typeof(DataMemberAttribute));

            foreach (DataColumn column in dt.Columns)
            {
                CodeMemberField field2 = new CodeMemberField(
                    new CodeTypeReference(
                        ConvertListUtil.GetType(column.DataType.Name, column.AllowDBNull))
                         , Util.GetLower(column.ColumnName.Replace(" ", string.Empty)));
                field2.Attributes = MemberAttributes.Private;
                genClass.Members.Add(field2);

                ////生成属性
                var property = new CodeMemberProperty();
                property.Name = Util.GetUpper(column.ColumnName.Replace(" ", string.Empty));
                property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                property.Type = new CodeTypeReference(ConvertListUtil.GetType(column.DataType.Name, column.AllowDBNull));
                property.HasGet = true;
                property.HasSet = true;

                if (pri != null && pri.ColumnName.ToLower() == column.ColumnName.ToLower())
                {
                    if (incr != null && incr.ColumnName.ToLower() == column.ColumnName.ToLower())
                    {
                        property.CustomAttributes.Add(new CodeAttributeDeclaration(evin, new CodeAttributeArgument(new CodePrimitiveExpression(true)
                      )));
                    }
                    else
                    {
                        property.CustomAttributes.Add(new CodeAttributeDeclaration(evin));
                    }
                }
                else if (column.Ordinal == 0)
                {
                    property.CustomAttributes.Add(new CodeAttributeDeclaration(evin));
                }

                property.CustomAttributes.Add(new CodeAttributeDeclaration(datacol));

                CodeVariableReferenceExpression field3 = new CodeVariableReferenceExpression(Util.GetLower(column.ColumnName.Replace(" ", string.Empty)));
                CodeMethodReturnStatement propertyReturn = new CodeMethodReturnStatement(field3);
                property.GetStatements.Add(propertyReturn);

                CodeAssignStatement propertyAssignment = new CodeAssignStatement(field3, new CodePropertySetValueReferenceExpression());
                property.SetStatements.Add(propertyAssignment);
                genClass.Members.Add(property);
            }
        }

    }
}
