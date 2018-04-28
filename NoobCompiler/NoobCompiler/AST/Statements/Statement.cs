using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Contexts;


namespace NoobCompiler.AST.Statements
{
   public class Statement : Node,IEmit
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
        public virtual bool EmitIntermediate(IntermediateEmitContext ec)
        {
            return true;
        }
    }
}
