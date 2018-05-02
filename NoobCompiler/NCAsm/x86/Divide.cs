using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
	/// <summary>
	/// Puts the result of the divide into EAX, and the remainder in EDX
	/// </summary>
    [NCAsm.OpCode("div")]
	public class Divide: InstructionWithDestinationAndSize {
	}

    [NCAsm.OpCode("idiv")]
    public class SignedDivide : InstructionWithDestinationAndSize
    {
    }
}