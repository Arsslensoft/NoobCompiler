using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NCAsm.x86;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Expressions
{
   public class MethodInvocationExpression : Expression
    {
        protected CallingConventionsHandler ccvh;
        public MethodSpec Method;
        public string Name { get; set; }
        public List<Expression> Arguments { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            IsVoid = true;
            ccvh = new CallingConventionsHandler();

            // resolve parameters
            var para_exprs = new List<Expression>();
            foreach (var expression in Arguments)
            {
                if (expression.IsVoid)
                    ResolveContext.Report.Error(104, expression.Location, "cannot convert int to void");

                para_exprs.Add((Expression)expression.DoResolve(rc));
            }

            Arguments = para_exprs;


            // resolve method
            rc.Resolver.ResolveMethod(Name, ref Method, para_exprs.Count);

            if (Method == null)
                ResolveContext.Report.Error(102, Location,
                    "Unknown method " + Name + " with " + para_exprs.Count + " parameters");
            else IsVoid = !Method.IsFunction;


            return base.DoResolve(rc);
        }

        /// <summary>Emit code</summary>
        /// <returns>Success or fail</returns>
        public override bool Emit(EmitContext ec)
        {
            ccvh.EmitCall(ec, Arguments, Method, false);
            return base.Emit(ec);
        }

        public override bool EmitToStack(EmitContext ec)
        {
            ccvh.EmitCall(ec, Arguments, Method, true);
            ec.EmitSubRoutinePush(ec);
            return base.EmitToStack(ec);
        }
        public override bool EmitBranchable(EmitContext ec, Label truecase, bool v)
        {
                Emit(ec);
                ec.EmitInstruction(new Compare() { DestinationReg = EmitContext.A, SourceValue = (ushort)1 });
                ec.EmitBooleanBranch(v, truecase, ConditionalTestEnum.Equal, ConditionalTestEnum.NotEqual);
            return true;
        }
        public override string CommentString()
        {
            return Name + ((Arguments == null) ? "()" : "(" + Arguments.Count+" params)");
        }
    }
}
