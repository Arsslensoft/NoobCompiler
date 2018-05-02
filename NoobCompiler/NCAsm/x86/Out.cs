using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("out")]
    public class Out : InstructionWithDestinationAndSize {
        public override void WriteText( NCAsm.AsmContext aAssembler, AssemblyWriter aOutput )
        {
            aOutput.Write(mMnemonic);
            aOutput.Write(" DX, ");
            aOutput.Write(this.GetDestinationAsString());
        }
	}
}