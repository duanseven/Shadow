﻿using System;
using System.CodeDom;
using System.Data;
using NSun.Data;
using System.Runtime.Serialization;
using GeneratorV2.Commons;

namespace Generator
{
    public class QueryTableCodeGenerator : CodeGeneratorBase
    {
        public bool IsReadOnly { get; private set; }

        public QueryTableCodeGenerator(string outputFileName, string outputLang, string outputNamespace, DBQuery connStr, string name, bool isReadOnly, bool isCastle)
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
            //DataContractAttribute
            CodeTypeReference dataatr = new CodeTypeReference(typeof(DataContractAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(dataatr));
            //TableAttribute
            CodeTypeReference tabatr = new CodeTypeReference(typeof(TableAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(tabatr,
                                                                       new CodeAttributeArgument(
                                                                           new CodePrimitiveExpression(Name))));

            var criteria = _cmdFac.CreateQuery(Name);
            var dt = _queryService.Query(criteria);
            var incr = _queryService.QuerySchemaIncrement(criteria);//自增
            var pri = _queryService.QuerySchemaPrimary(criteria);//主键

            //CodeTypeReference evin = new CodeTypeReference(typeof(PrimaryKeyAttribute));
            //CodeTypeReference datacol = new CodeTypeReference(typeof(DataMemberAttribute));

            //静态构造函数
            CodeTypeConstructor ctyc = new CodeTypeConstructor();
            genClass.Members.Add(ctyc);

            foreach (DataColumn column in dt.Columns)
            {
                //使用老办法解决
                //virtual Id { get; set; }
                //var cfield = new CodeMemberField
                //{
                //    Attributes = MemberAttributes.Public | MemberAttributes.Final,
                //    Name = Util.GetUpper(column.ColumnName.Replace(" ", string.Empty)),//(IsCastle? "virtual ":"") + 
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
            }
        }

    }
}

//CodeMemberProperty prop = new CodeMemberProperty();
//prop.Attributes = MemberAttributes.Public;
//prop.Type = new CodeTypeReference(typeof(string));
//prop.Name = propertyName;
//prop.HasGet = true;
//prop.GetStatements.Add(new CodeSnippetExpression("return " + nameDeclaration));
//prop.HasSet = true;
//prop.SetStatements.Add(new CodeSnippetExpression(nameDeclaration + " = value"));