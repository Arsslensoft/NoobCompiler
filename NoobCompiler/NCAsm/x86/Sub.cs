using System;
using System.Linq;

namespace NCAsm.x86 {
    /// <summary>
    /// Subtracts the source operand from the destination operand and 
    /// replaces the destination operand with the result. 
    /// </summary>
    [NCAsm.OpCode("sub")]
    public class Sub : InstructionWithDestinationAndSourceAndSize {
         public Sub()
        {
            OptimizingBehaviour = OptimizationKind.None;
        }
    }
}