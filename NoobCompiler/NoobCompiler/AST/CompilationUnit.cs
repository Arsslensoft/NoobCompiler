using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Declarations
{
   public class CompilationUnit : Node
    {
            public ProgramDeclaration Program { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            return Program.DoResolve(rc);
        }
    }
}
