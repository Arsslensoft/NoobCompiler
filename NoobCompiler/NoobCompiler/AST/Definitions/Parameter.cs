using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST.Definitions
{
  public class Parameter : Node
    {
        public string Name { get; set; }
        public bool IsVariable { get; set; }
  

    }
}
