using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm
{
    public class LiteralAssemblerCode: Instruction
    {
        public LiteralAssemblerCode(string code)
        {
            Code = code;
        }

        public string Code
        {
            get;
            set;
        }

        public override void WriteText(AsmContext aAssembler, AssemblyWriter aOutput)
        {
            aOutput.Write(Code);
        }
    }
}
