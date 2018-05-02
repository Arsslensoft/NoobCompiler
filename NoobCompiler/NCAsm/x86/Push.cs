using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("push")]
    public class Push : InstructionWithDestinationAndSize {

        public Push():base("push") {
            Size = 32;
        }
    }
    [NCAsm.OpCode("org")]
    public class Org : InstructionWithDestinationAndSize
    {

        public Org()
            : base("org")
        {
            Size = 80;
        }
    }
}
