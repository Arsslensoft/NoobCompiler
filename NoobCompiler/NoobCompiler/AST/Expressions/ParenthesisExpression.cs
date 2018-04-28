using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NCAsm.x86;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Expressions
{
   public class ParenthesisExpression : Expression
    {
        public Expression Target { get; set; }
        
        public override INode DoResolve(ResolveContext rc)
        {
            return Target.DoResolve(rc);
        }

        public override bool Resolve(ResolveContext rc)
        {
            return Target.Resolve(rc);
        }

        /// <summary>Emit code</summary>
        /// <returns>Success or fail</returns>
        public override bool Emit(EmitContext ec)
        {
            return Target.Emit(ec);
        }

        public override bool EmitToStack(EmitContext ec)
        {
            return Target.EmitToStack(ec);
        }

        public override bool EmitBranchable(EmitContext ec, Label truecase, bool v)
        {
            return Target.EmitBranchable(ec, truecase, v);
        }

        public override bool EmitFromStack(EmitContext ec)
        {
            return Target.EmitFromStack(ec);
        }

        public override bool EmitToRegister(EmitContext ec, RegistersEnum rg)
        {
            return Target.EmitToRegister(ec, rg);
        }

        public override string CommentString()
        {
            return "(" + Target.CommentString() + ")";
        }
    }
}
