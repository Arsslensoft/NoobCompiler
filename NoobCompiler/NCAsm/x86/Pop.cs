using System;
using System.Linq;

namespace NCAsm.x86 {
    [NCAsm.OpCode("pop")]
	public class Pop: InstructionWithDestinationAndSize{
        public Pop() : base("pop")
        {
        }
	}

}