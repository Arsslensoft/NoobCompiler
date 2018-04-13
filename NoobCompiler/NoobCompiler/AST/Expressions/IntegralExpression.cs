using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST.Expressions
{
   public class IntegralExpression : Expression
    {
        public int Value { get; set; }
    }
}
