using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST.Expressions
{
   public class BinaryOperationExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public Operators Operator { get; set; }
    }
}
