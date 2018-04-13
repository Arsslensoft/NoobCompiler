using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Base;

namespace NoobCompiler.AST
{
    public interface INode
    {
        Location Location { get; set; }

    }
}
