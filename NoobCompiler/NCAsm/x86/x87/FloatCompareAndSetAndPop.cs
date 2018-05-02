using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86.x87
{
    [NCAsm.OpCode("fcomip")]
    public class FloatCompareAndSetAndPop : InstructionWithDestination
    {
    }
    [NCAsm.OpCode("fcompp")]
    public class FloatCompareAnd2Pop : Instruction
    {


    }
    [NCAsm.OpCode("fstsw")]
    public class FloatStoreStatus : InstructionWithDestination
    {


    }
    [NCAsm.OpCode("sahf")]
    public class StoreAHToFlags : Instruction
    {


    }
    [NCAsm.OpCode("fwait")]
    public class FloatWait : Instruction
    {


    }
   
}
