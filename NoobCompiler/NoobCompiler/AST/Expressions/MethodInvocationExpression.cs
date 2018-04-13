using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST.Expressions
{
   public class MethodInvocationExpression : Expression
    {
        public string Name { get; set; }
        public List<Expression> Arguments { get; set; }
    }
}
