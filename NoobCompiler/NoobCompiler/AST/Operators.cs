using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST
{
    public enum Operators
    {
        Add,
        Sub,
        Mul,
        Div,
        Mod,
        And,
        Or,

        Equal,
        NotEqual,
        GT,
        LT,
        GTE,
        LTE,
        LogicalNot
    }
}
