using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Expressions
{
   public class IntegralExpression : Expression, IResolve
    {
        public int Value { get; set; }
        public override string CommentString()
        {
            return Value.ToString();
        }

        /// <summary>Emit code</summary>
        /// <returns>Success or fail</returns>
        public override bool Emit(EmitContext ec)
        {
            return EmitToStack(ec);
        }

        public override bool EmitToStack(EmitContext ec)
        {
            ec.EmitPush((ushort)Value);
            return base.EmitToStack(ec);
        }
    }
}
