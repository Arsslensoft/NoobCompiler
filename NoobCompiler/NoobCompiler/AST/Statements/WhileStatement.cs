using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Expressions;

namespace NoobCompiler.AST.Statements
{
   public class WhileStatement : Statement
    {
        public Expression Expression { get; set; }
        public Statement Statement { get; set;  }
    }
}
