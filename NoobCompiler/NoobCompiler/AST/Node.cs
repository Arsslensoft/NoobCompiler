using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Base;

namespace NoobCompiler.AST
{
   public abstract class Node : INode
    {
        public Location Location { get; set; }

    }
}
