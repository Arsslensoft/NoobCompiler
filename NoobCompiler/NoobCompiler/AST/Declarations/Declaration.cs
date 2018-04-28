using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Contexts;


namespace NoobCompiler.AST.Declarations
{
   public class Declaration : Node, IEmit
    {
        /// <summary>
        /// Emit code
        /// </summary>
        /// <returns>Success or fail</returns>
        public virtual bool Emit(EmitContext ec)
        {
            return true;
        }

        /// <summary>
        /// Emit 3 address code
        /// </summary>
        /// <returns>Success or fail</returns>
        public bool EmitIntermediate(IntermediateEmitContext ec)
        {
            throw new NotImplementedException();
        }
    }
}
