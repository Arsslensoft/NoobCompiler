using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NCAsm.x86;
using NoobCompiler.Contexts;
using IEmit = NoobCompiler.Contexts.IEmit;
using IEmitExpr = NoobCompiler.Contexts.IEmitExpr;


namespace NoobCompiler.AST.Expressions
{
public class Expression : Node, IEmit, IEmitExpr
    {
        public virtual bool IsVoid { get; set; }
        public virtual bool EmitToStack(EmitContext ec)
        {
            return true;
        }
        public virtual bool EmitFromStack(EmitContext ec)
        {
           return true;
        }
        public virtual bool EmitToRegister(EmitContext ec, RegistersEnum rg)
        {
            return true;
        }
        public virtual bool EmitBranchable(EmitContext ec, Label truecase, bool v)
        {
            return true;
        }

        /// <summary>
        /// Emit 3 address code
        /// </summary>
        /// <returns>Success or fail</returns>
        public virtual bool EmitIntermediate(IntermediateEmitContext ec)
        {
            return true;
        }
        public virtual bool Emit(EmitContext ec)
        {
            return true;
        }
    }
}
