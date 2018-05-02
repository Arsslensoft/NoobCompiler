using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86
{
    public class ExternalLabel: Instruction
    {
        public ExternalLabel(string aName):base()
        {
            Name = aName;
        }

        public string Name
        {
            get;
            set;
        }

        public override void WriteText( NCAsm.AsmContext aAssembler, AssemblyWriter aOutput )
        {
            aOutput.Write("extern ");
            aOutput.Write(Name);
        }
    }
}