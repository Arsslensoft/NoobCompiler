using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("cmp")]
	public class Compare: InstructionWithDestinationAndSourceAndSize {
        public Compare() : base("cmp")
        {
      
            OptimizingBehaviour = OptimizationKind.None;
        


        }
	}
}