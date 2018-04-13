using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Expressions;

namespace NoobCompiler.AST.Statements
{
   public class AssignmentStatement : Statement
    {
        public string Target { get; set; }
        public Expression Expression { get; set; }
    }
}
