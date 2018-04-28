using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Expressions
{
   public class IntegralExpression : Expression, IResolve
    {
        public int Value { get; set; }
    }
}
