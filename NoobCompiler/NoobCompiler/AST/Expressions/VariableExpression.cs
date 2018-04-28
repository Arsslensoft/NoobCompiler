using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Expressions
{
  public class VariableExpression : Expression
    {
        public MemberSpec variable;
        public string Name { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            if (variable == null)
            {
                variable = rc.Resolver.TryResolveName(Name);
                if (variable == null)
                    ResolveContext.Report.Error(1, Location, "Unresolved variable '" + Name + "'");
            }
            return base.DoResolve(rc);
        }
    }
}
