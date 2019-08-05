using System;
using System.CodeDom;
using System.Data;
using NSun.Data;
using System.Runtime.Serialization;
using GeneratorV2.Commons;

namespace Generator
{
    /// <summary>
    /// Entity Table 
    /// </summary>
    public class QueryTableCodeGenerator2 : CodeGeneratorBase
    {
        public bool IsReadOnly { get; private set; }

        public string AssemblyName { get; set; }

        public QueryTableCodeGenerator2(string outputFileName, string outputLang, string outputNamespace, DBQuery connStr, string name, bool isReadOnly, bool isCastle)
            : base(outputFileName, outputLang, outputNamespace, connStr, name, isCastle)
        {
            IsReadOnly = isReadOnly;
        }

        protected override void DoGenerate(CodeTypeDeclaration genClass)
        {            
            //genClass.BaseTypes.Add(typeof(BaseEntity));
            genClass.BaseTypes.Add(IsCastle ? typeof(BaseEntity) : typeof(BaseEntityRefObject));

            //Serializable
            CodeTypeReference seri = new CodeTypeReference(typeof(SerializableAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(seri));

            CodeTypeReference dataatr = new CodeTypeReference(typeof(DataContractAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(dataatr));
           
            if (IsPartial)
            {
                //TableAttribute
                CodeTypeReference tabatr = new CodeTypeReference(typeof(TableAttribute));
                genClass.CustomAttributes.Add(new CodeAttributeDeclaration(tabatr,
                                                                           new CodeAttributeArgument(
                                                                               new CodePrimitiveExpression(Name))));
            }
            else
            {
                //要做映射的 还需要加程序集
                string ctr = AssemblyName + "," + OutputNamespace + "." + OldName + "Table";
                CodeTypeReference tabatr = new CodeTypeReference(typeof (TableMappingAttribute));
                genClass.CustomAttributes.Add(new CodeAttributeDeclaration(tabatr,
                                                                           new CodeAttributeArgument(
                                                                               new CodePrimitiveExpression(ctr)
                                                                               )));
            }
            var criteria = _cmdFac.CreateQuery(Name);
            var dt = _queryService.Query(criteria);
             
            foreach (DataColumn column in dt.Columns)
            {
                //var cfield = new CodeMemberField
                //{
                //    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                //    Name = (IsCastle? "virtual ":"") + Util.GetUpper(column.ColumnName.Replace(" ", string.Empty)),
                //    Type = new CodeTypeReference(ConvertListUtil.GetType(column.DataType.Name, column.AllowDBNull)),
                //};
                //cfield.CustomAttributes.Add(new CodeAttributeDeclaration(datacol));
                //cfield.Name += " { get; set; }//" + column.Table.TableName + "." + column.ColumnName;
                //genClass.Members.Add(cfield);

                CodeMemberField field2 = new CodeMemberField(
           new CodeTypeReference(
               ConvertListUtil.GetType(column.DataType.Name, column.AllowDBNull))
                , Util.GetLower(column.ColumnName.Replace(" ", string.Empty)));
                field2.Attributes = MemberAttributes.Private;
                genClass.Members.Add(field2);

             
                CodeMemberProperty prop = new CodeMemberProperty();

                CodeTypeReference datacol = new CodeTypeReference(typeof(DataMemberAttribute));
                prop.CustomAttributes.Add((new CodeAttributeDeclaration(datacol)));

                prop.Attributes = IsCastle
                                      ? MemberAttributes.Public
                                      : (MemberAttributes.Public | MemberAttributes.Final);
                prop.Type = new CodeTypeReference(ConvertListUtil.GetType(column.DataType.Name, column.AllowDBNull));
                prop.Name = Util.GetUpper(column.ColumnName.Replace(" ", string.Empty));
                prop.HasGet = true;
                prop.GetStatements.Add(new CodeSnippetExpression("return " + field2.Name));
                prop.HasSet = true;
                prop.SetStatements.Add(new CodeSnippetExpression(field2.Name + " = value"));
                genClass.Members.Add(prop);
            }
        }

    }
}
