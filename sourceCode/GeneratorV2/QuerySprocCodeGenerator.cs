using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using NSun.Data;
using GeneratorV2.Commons;

namespace Generator
{
    public class QuerySprocCodeGenerator : CodeGeneratorBase
    {
        public QuerySprocCodeGenerator(string outputFileName, string outputLang, string outputNamespace, DBQuery connStr, string name)
            : base(outputFileName, outputLang, outputNamespace, connStr, name, false)
        {
        }

        protected override void DoGenerate(CodeTypeDeclaration genClass)
        {
            genClass.BaseTypes.Add(typeof(SprocEntity));

            CodeTypeReference inputparm = new CodeTypeReference(typeof(InputParameterAttribute));
            CodeTypeReference inputoutputparm = new CodeTypeReference(typeof(InputOutputParameterAttribute));
            CodeTypeReference outputparm = new CodeTypeReference(typeof(OutputParameterAttribute));
            CodeTypeReference returnparm = new CodeTypeReference(typeof(ReturnParameterAttribute));

            CodeTypeReference datacol = new CodeTypeReference(typeof(DataMemberAttribute));

            //Serializable
            CodeTypeReference seri = new CodeTypeReference(typeof(SerializableAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(seri));

            CodeTypeReference sprocatr = new CodeTypeReference(typeof(ProcedureAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(sprocatr, new CodeAttributeArgument(new CodePrimitiveExpression(Name)
                )));

            CodeTypeReference dataatr = new CodeTypeReference(typeof(DataContractAttribute));
            genClass.CustomAttributes.Add(new CodeAttributeDeclaration(dataatr));

            var criteria = _cmdFac.CreateStoredProcedure(Name);

            var sqlCommand = _cmdFac.PrepareCommand(criteria, true);
            using (var conn = sqlCommand.Connection)
            {
                conn.Open();
                BindDeriveParameters(sqlCommand);
                conn.Close();
            }

            foreach (System.Data.Common.DbParameter parameter in sqlCommand.Parameters)
            {
                var paramName = parameter.ParameterName.TrimStart("@?:".ToCharArray());
                CodeMemberField field = new CodeMemberField(
                    new CodeTypeReference(
                        ConvertListUtil.GetTypeFromDbType(parameter.DbType))
                         , Util.GetLower(paramName.Replace(" ", string.Empty)));
                field.Attributes = MemberAttributes.Private;
                genClass.Members.Add(field);

                var property = new CodeMemberProperty();
                property.Name = Util.GetUpper(paramName.Replace(" ", string.Empty));
                property.Attributes = MemberAttributes.Public | MemberAttributes.Final;
                property.Type = field.Type;
                property.HasGet = true;
                property.HasSet = true;

                switch (parameter.Direction)
                {
                    case ParameterDirection.Input:
                        property.CustomAttributes.Add(new CodeAttributeDeclaration(inputparm,
                            new CodeAttributeArgument(new CodePrimitiveExpression(paramName)),
                            new CodeAttributeArgument(new CodeFieldReferenceExpression(
                                 new CodeTypeReferenceExpression(parameter.DbType.GetType()), Enum.GetName(parameter.DbType.GetType(), parameter.DbType)
                                ))
                            ));
                        break;
                    case ParameterDirection.InputOutput:
                        property.CustomAttributes.Add(new CodeAttributeDeclaration(inputoutputparm,
                               new CodeAttributeArgument(new CodePrimitiveExpression(paramName)),
                            new CodeAttributeArgument(new CodeFieldReferenceExpression(
                                 new CodeTypeReferenceExpression(parameter.DbType.GetType()), Enum.GetName(parameter.DbType.GetType(), parameter.DbType)
                                )),
                            new CodeAttributeArgument(new CodePrimitiveExpression(parameter.Size))
                            ));
                        break;
                    case ParameterDirection.Output:
                        property.CustomAttributes.Add(new CodeAttributeDeclaration(outputparm,
                               new CodeAttributeArgument(new CodePrimitiveExpression(paramName)),
                            new CodeAttributeArgument(new CodeFieldReferenceExpression(
                                 new CodeTypeReferenceExpression(parameter.DbType.GetType()), Enum.GetName(parameter.DbType.GetType(), parameter.DbType)
                                )),
                            new CodeAttributeArgument(new CodePrimitiveExpression(parameter.Size))
                            ));
                        break;
                    case ParameterDirection.ReturnValue:
                        property.CustomAttributes.Add(new CodeAttributeDeclaration(returnparm,
                               new CodeAttributeArgument(new CodePrimitiveExpression(paramName)),
                            new CodeAttributeArgument(new CodeFieldReferenceExpression(
                                 new CodeTypeReferenceExpression(parameter.DbType.GetType()), Enum.GetName(parameter.DbType.GetType(), parameter.DbType)
                                )),
                            new CodeAttributeArgument(new CodePrimitiveExpression(parameter.Size))
                            ));
                        break;
                }

                CodeVariableReferenceExpression field3 = new CodeVariableReferenceExpression(field.Name);
                CodeMethodReturnStatement propertyReturn = new CodeMethodReturnStatement(field3);
                property.GetStatements.Add(propertyReturn);

                CodeAssignStatement propertyAssignment = new CodeAssignStatement(field3, new CodePropertySetValueReferenceExpression());
                property.SetStatements.Add(propertyAssignment);

                property.CustomAttributes.Add(new CodeAttributeDeclaration(datacol));

                genClass.Members.Add(property);
            }
        }

        private void BindDeriveParameters(System.Data.Common.DbCommand sqlCommand)
        {
            if (sqlCommand is SqlCommand)
            {
                SqlCommandBuilder.DeriveParameters((SqlCommand)sqlCommand);
            }
            if (sqlCommand is System.Data.OracleClient.OracleCommand)
            {
                System.Data.OracleClient.OracleCommandBuilder.DeriveParameters(
                    (System.Data.OracleClient.OracleCommand)sqlCommand);
            }
            if (sqlCommand is MySql.Data.MySqlClient.MySqlCommand)
            {
                MySql.Data.MySqlClient.MySqlCommandBuilder.DeriveParameters(
                    (MySql.Data.MySqlClient.MySqlCommand)sqlCommand);
            }
            if (sqlCommand is IBM.Data.DB2.DB2Command)
            {
                IBM.Data.DB2.DB2CommandBuilder.DeriveParameters((IBM.Data.DB2.DB2Command)sqlCommand);
            }
            if (sqlCommand is Npgsql.NpgsqlCommand)
            {
                Npgsql.NpgsqlCommandBuilder.DeriveParameters((Npgsql.NpgsqlCommand)sqlCommand);
            }
        }

        private CodeExpression CreateSprocParameterDirectionEnumExpression(ParameterDirection parameterDirection)
        {
            return new CodeFieldReferenceExpression(
                new CodeTypeReferenceExpression(
                    typeof(SprocParameterDirection)
                ),
                parameterDirection.ToString()
            );
        }
    }
}
