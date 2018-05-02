using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("jcc")]
    public class ConditionalJump: JumpBase, IInstructionWithCondition {
        public ConditionalTestEnum Condition {
            get;
            set;
        }

        public override void WriteText( NCAsm.AsmContext aAssembler, AssemblyWriter aOutput )
        {
            mMnemonic = String.Intern("j" + Condition.GetMnemonic() + " ");
            base.WriteText(aAssembler, aOutput);
        }
    }
}