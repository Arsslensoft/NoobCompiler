using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST.Expressions
{
   public class UnaryOperationExpression : Expression
    {
        public Expression Expression { get; set; }
        public Operators Operator { get; set; }
    }
}
