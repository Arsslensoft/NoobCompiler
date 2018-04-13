using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST.Expressions
{
   public class ParenthesisExpression : Expression
    {
        public Expression Target { get; set; }
    }
}
