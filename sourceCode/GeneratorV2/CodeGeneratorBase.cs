using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using GeneratorEntity;
using NSun.Data;
using GeneratorV2.ViewModel;
using GeneratorV2.Service;
using GeneratorV2.Commons;

namespace Generator
{
    public abstract class CodeGeneratorBase
    {
        protected readonly DBQuery _cmdFac;
        protected readonly QueryService _queryService;

        public string OutputFileName { get; private set; }
        public string OutputLanguage { get; private set; }
        public string OutputNamespace { get; private set; } 
        public QueryCommandBuilder ConnectionString { get; private set; }
        public string Name { get; private set; }
        public string OldName { get; private set; }
        public bool IsCastle { get; set; }

        public bool IsPartial { get; set; }

        public string NameWithoutSpaces { get { return Name.Replace(" ", string.Empty); } }

        public MainWindowViewModel ViewMode { get; set; } 

        public CodeGeneratorBase(string outputFileName, string outputLang, string outputNamespace, DBQuery connStr, string name, bool isCastle)
        {
            OutputFileName = outputFileName;
            OutputLanguage = outputLang;
            OutputNamespace = outputNamespace;
            OldName = name;
            IsCastle = isCastle;
            _cmdFac = connStr;
            _queryService = new QueryService(connStr);
        }

        public void Generate()
        {
            Name = ViewMode.ConnInfoView.SQLType != SqlType.PostgreSql ? OldName.ToUpper() : OldName;
            FileInfo file = new FileInfo(OutputFileName);

            var table = Path.Combine(file.Directory.FullName, "DomainTable");
            var model = Path.Combine(file.Directory.FullName, "DomainModel");

            if (!Directory.Exists(table) && this.GetType() == typeof(QueryTableCodeGenerator1))
            {
                Directory.CreateDirectory(table);
            }

            if (!Directory.Exists(model))
            {
                Directory.CreateDirectory(model);
            }
            OutputFileName = Path.Combine(this.GetType() == typeof (QueryTableCodeGenerator1) ? table : model, file.Name);

            CodeCompileUnit unit = new CodeCompileUnit();
            //namespace
            CodeNamespace ns =
                new CodeNamespace(string.IsNullOrEmpty(OutputNamespace) ? "NSunQueryEntitys" : OutputNamespace);
            unit.Namespaces.Add(ns);
            //type
            var classname = Util.GetUpper(OldName + ViewMode.FileInfoView.FileNameSuffix);
            if (this.GetType().FullName == typeof(QuerySprocCodeGenerator).FullName)
            {
                classname = Util.GetUpper(OldName + "Sproc");
            }

            CodeTypeDeclaration genClass = new CodeTypeDeclaration(classname);
            ns.Types.Add(genClass);
            //add using
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("NSun"));
            ns.Imports.Add(new CodeNamespaceImport("NSun.Data"));
            ns.Imports.Add(new CodeNamespaceImport("NSun.Data.Collection"));
            
            //set class
            genClass.Attributes = MemberAttributes.Public;
            genClass.IsClass = true;
            genClass.IsPartial = IsPartial;

            //do class
            DoGenerate(genClass);

            //out put class
            CodeDomProvider provider;
            if (OutputLanguage == "C#")
                provider = new Microsoft.CSharp.CSharpCodeProvider();
            else
                provider = new Microsoft.VisualBasic.VBCodeProvider();
                         
            using (StreamWriter sw = new StreamWriter(OutputFileName))
            {
                IndentedTextWriter indentedWriter = new IndentedTextWriter(sw, "  ");
                var options = new CodeGeneratorOptions
                                  {
                                      BlankLinesBetweenMembers = true,
                                      BracingStyle = "C",
                                      VerbatimOrder = true                                      
                };
                provider.GenerateCodeFromCompileUnit(unit, indentedWriter, options);
                sw.Close();
            }
            
        }

        public void GenerateMapping()
        {
            Name = ViewMode.ConnInfoView.SQLType != SqlType.PostgreSql ? OldName.ToUpper() : OldName;
            CodeCompileUnit unit = new CodeCompileUnit();
            //namespace
            CodeNamespace ns =
                new CodeNamespace(string.IsNullOrEmpty(OutputNamespace) ? "NSunQueryEntitys" : OutputNamespace);
            unit.Namespaces.Add(ns);
            //type
            CodeTypeDeclaration genClass = new CodeTypeDeclaration(Util.GetUpper(NameWithoutSpaces.ToLower()));            
            ns.Types.Add(genClass);
            //add using
            ns.Imports.Add(new CodeNamespaceImport("System"));
            ns.Imports.Add(new CodeNamespaceImport("NSun"));
            ns.Imports.Add(new CodeNamespaceImport("NSun.Data"));
            ns.Imports.Add(new CodeNamespaceImport("NSun.Data.Collection"));

            //set class
            genClass.Attributes = MemberAttributes.Public;
            genClass.IsClass = true;
            genClass.IsPartial = true;

            //do class
            DoGenerate(genClass);

            //out put class
            CodeDomProvider provider;
            if (OutputLanguage == "C#")
                provider = new Microsoft.CSharp.CSharpCodeProvider();
            else
                provider = new Microsoft.VisualBasic.VBCodeProvider();
        
            using (StreamWriter sw = new StreamWriter(OutputFileName))
            {
                IndentedTextWriter indentedWriter = new IndentedTextWriter(sw, "  ");
                var options = new CodeGeneratorOptions
                {
                    BlankLinesBetweenMembers = true,
                    BracingStyle = "C",
                    VerbatimOrder = true
                };
                provider.GenerateCodeFromCompileUnit(unit, indentedWriter, options);
                sw.Close();
            }

        } 
        protected abstract void DoGenerate(CodeTypeDeclaration genClass);
    }
}
