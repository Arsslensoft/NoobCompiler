using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST.Statements
{
   public class BlockStatement : Statement
    {
        public List<Statement> Statements { get; set; }
        
    }
}
