using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("ret")]
	public class Return: InstructionWithDestination {
        public Return() {
            DestinationValue = 0;
        }
    }
    [NCAsm.OpCode("ret")]
    public class SimpleReturn : Instruction
    {
        public SimpleReturn()
        {
       
        }
    }
}