using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("and")]
	public class And: InstructionWithDestinationAndSourceAndSize {

        public And()
        {
            OptimizingBehaviour = OptimizationKind.None;
        }
	}
}
