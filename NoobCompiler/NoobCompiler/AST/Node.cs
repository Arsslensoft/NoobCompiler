using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Base;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST
{
   public abstract class Node : INode, IResolve
    {
        public Location Location { get; set; }

        public virtual bool Resolve(ResolveContext rc)
        {
            rc.CurrentStatementState = ResolverState.Create(rc);
            return true;
        }
        public virtual INode DoResolve(ResolveContext rc)
        {
            return this;
        }
    }
}
