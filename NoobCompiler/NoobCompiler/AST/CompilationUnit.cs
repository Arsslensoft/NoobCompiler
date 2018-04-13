using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.AST.Declarations
{
   public class CompilationUnit : Node
    {
            public ProgramDeclaration Program { get; set; }
    }
}
