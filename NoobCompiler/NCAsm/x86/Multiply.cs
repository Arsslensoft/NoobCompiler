using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("mul")]
	public class Multiply: InstructionWithDestinationAndSize {

        public Multiply()
        {
            OptimizingBehaviour = OptimizationKind.None;
        }
	}
    [NCAsm.OpCode("imul")]
    public class SignedMultiply : InstructionWithDestinationAndSize
    {
        public SignedMultiply()
        {
            OptimizingBehaviour = OptimizationKind.None;
        }
    }
}
