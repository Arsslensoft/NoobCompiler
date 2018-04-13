using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Expressions;

namespace NoobCompiler.AST.Statements
{
   public class IfStatement : Statement
    {
        public Expression Expression { get; set; }
        public Statement TrueStatement { get; set; }
        public Statement FalseStatement { get; set; }

    }
}
