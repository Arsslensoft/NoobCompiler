using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Definitions;
using NoobCompiler.AST.Statements;

namespace NoobCompiler.AST.Declarations
{
  public class MethodDeclaration : Declaration
    {
        public bool IsFunction { get; set; }
        public string Name { get; set; }
        public List<Parameter> Parameters { get; set; }
        public List<VariableDeclaration> LocalVariables { get; set; }
        public Statement Block { get; set; }
    }
}
