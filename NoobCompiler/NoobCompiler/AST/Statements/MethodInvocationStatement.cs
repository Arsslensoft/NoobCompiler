using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Expressions;

namespace NoobCompiler.AST.Statements
{
   public class MethodInvocationStatement : Statement
    {
        public string Name { get; set; }
        public List<Expression> Arguments { get; set; }
    }
}
