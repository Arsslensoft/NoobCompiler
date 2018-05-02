using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("xor")]
	public class Xor: InstructionWithDestinationAndSourceAndSize {
        public Xor()
        {
            OptimizingBehaviour = OptimizationKind.None;
        }
	}
}
