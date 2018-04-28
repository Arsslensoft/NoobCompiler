using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Expressions;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Statements
{
   public class AssignmentStatement : Statement
    {

        public MemberSpec variable;
        public string Target { get; set; }
        public Expression Expression { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            Expression = (Expression) Expression.DoResolve(rc);
            if (variable == null)
            {
                variable = rc.Resolver.TryResolveName(Target);
                if (variable == null)
                    ResolveContext.Report.Error(101, Location, "Unresolved variable '" + Target + "'");
            }
            if(Expression.IsVoid)
                ResolveContext.Report.Error(103, Location, "cannot affect void type to '" + Target + "'");
            return base.DoResolve(rc);
        }

        public override bool Emit(EmitContext ec)
        {

            Expression.EmitToStack(ec);
            ec.EmitComment(Target + "=" + Expression.CommentString());
            variable.EmitFromStack(ec);
            return true;
        }
   
    }
}
