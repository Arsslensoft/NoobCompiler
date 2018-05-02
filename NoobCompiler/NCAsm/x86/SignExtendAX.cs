using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("cdq")]
    public class SignExtendAX : InstructionWithSize {
        public override void WriteText( NCAsm.AsmContext aAssembler, AssemblyWriter aOutput )
        {
            switch (Size) {
                case 32:
                    aOutput.Write("cdq");
                    return;
                case 16:
                    aOutput.Write("cwde");
                    return;
                case 8:
                    aOutput.Write("cbw");
                    return;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}