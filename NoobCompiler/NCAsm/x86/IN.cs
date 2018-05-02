using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("in")]
    public class IN : InstructionWithDestinationAndSize {
        public override void WriteText( NCAsm.AsmContext aAssembler, AssemblyWriter aOutput )
        {
            base.WriteText(aAssembler, aOutput);
            aOutput.Write(", DX");
        }
    }
}
