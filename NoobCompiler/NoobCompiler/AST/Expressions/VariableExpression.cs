using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST.Expressions
{
  public class VariableExpression : Expression
    {
        public string Name { get; set; }
    }
}
