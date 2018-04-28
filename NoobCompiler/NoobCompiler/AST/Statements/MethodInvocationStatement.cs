using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Expressions;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Statements
{
   public class MethodInvocationStatement : Statement
    {
        protected CallingConventionsHandler ccvh;
        public MethodSpec Method;
        public string Name { get; set; }
        public List<Expression> Arguments { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {

            ccvh = new CallingConventionsHandler();
            // resolve parameters
            var para_exprs = new List<Expression>();
            foreach (var expression in Arguments)
            {
                if (expression.IsVoid)
                    ResolveContext.Report.Error(104, expression.Location, "cannot convert int to void");

                para_exprs.Add((Expression) expression.DoResolve(rc));
            }

            Arguments = para_exprs;
       
            // resolve method
            rc.Resolver.ResolveMethod(Name, ref Method, para_exprs.Count);

            if (Method == null)
                ResolveContext.Report.Error(102, Location,
                    "Unknown method " + Name + " with " + para_exprs.Count + " parameters");


            return base.DoResolve(rc);
        }
        public override bool Emit(EmitContext ec)
        {
            ccvh.EmitCall(ec, Arguments, Method, false);
            return base.Emit(ec);
        }


    }
}
