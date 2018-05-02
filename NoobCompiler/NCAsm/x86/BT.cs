using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86
{
    [NCAsm.OpCode("bt")]
    public class BitTest : InstructionWithDestinationAndSourceAndSize
    {
    }
    [NCAsm.OpCode("btc")]
    public class BitTestAndComplement : InstructionWithDestinationAndSourceAndSize
    {
    }
    [NCAsm.OpCode("btr")]
    public class BitTestAndReset : InstructionWithDestinationAndSourceAndSize
    {
    }

    [NCAsm.OpCode("bts")]
    public class BitTestAndSet : InstructionWithDestinationAndSourceAndSize
    {
    }
}
