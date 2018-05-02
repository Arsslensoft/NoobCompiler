using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("add")]
	public class Add: InstructionWithDestinationAndSourceAndSize {
        public Add() : base("add")
        {
            OptimizingBehaviour = OptimizationKind.None;
        }
	}
}