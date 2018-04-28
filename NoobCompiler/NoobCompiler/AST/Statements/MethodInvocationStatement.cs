using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Expressions;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Statements
{
   public class MethodInvocationStatement : Statement
    {
        public MethodSpec Method;
        public string Name { get; set; }
        public List<Expression> Arguments { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            // restore old state temporarly
            rc.BackupCurrentAndSetStatement();

            // resolve parameters
            var para_exprs = new List<Expression>();
            foreach (var expression in Arguments)
            {
                if (expression.IsVoid)
                    ResolveContext.Report.Error(4, expression.Location, "cannot convert int to void");

                para_exprs.Add((Expression) expression.DoResolve(rc));
            }

            Arguments = para_exprs;
            // set back current state
            rc.RestoreOldState();

            // resolve method
            rc.Resolver.ResolveMethod(Name, ref Method, para_exprs.Count);

            if (Method == null)
                ResolveContext.Report.Error(2, Location,
                    "Unknown method " + Name + " with " + para_exprs.Count + " parameters");


            return base.DoResolve(rc);
        }
    }
}
