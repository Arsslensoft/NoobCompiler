using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Statements;

namespace NoobCompiler.AST.Declarations
{
   public class ProgramDeclaration : Declaration
    {
        public string Name { get; set; }
        public List<VariableDeclaration> Variables { get; set; }
        public List<MethodDeclaration> Methods { get; set; }

        public Statement Block { get; set; }

    }
}
