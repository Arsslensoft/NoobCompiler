﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    public abstract class InstructionWithDestination : Instruction, IInstructionWithDestination{
        public InstructionWithDestination()
        {
            
        }

        public InstructionWithDestination(string mnemonic):base(mnemonic)
        {

        }

        public NCAsm.ElementReference DestinationRef {
            get;
            set;
        }

        public RegistersEnum? DestinationReg
        {
            get;
            set;
        }

        public ushort? DestinationValue
        {
            get;
            set;
        }

        public bool DestinationIsIndirect {
            get;
            set;
        }

        public int DestinationDisplacement {
            get;
            set;
        }

        public bool DestinationEmpty
        {
            get;
            set;
        }

        public override bool IsComplete( NCAsm.AsmContext aAssembler )
        {
            if (DestinationRef != null) {
                ulong xAddress;
                return base.IsComplete(aAssembler) && DestinationRef.Resolve(aAssembler, out xAddress);
            }
            return base.IsComplete(aAssembler);
        }

        public override void UpdateAddress( NCAsm.AsmContext aAssembler, ref ulong aAddresss )
        {
            if (DestinationRef != null) {
                DestinationValue = 0xFFFF;
            }
            base.UpdateAddress(aAssembler, ref aAddresss);
        }

        public override void WriteText( NCAsm.AsmContext aAssembler, AssemblyWriter aOutput )
        {
            aOutput.Write(mMnemonic);
            String destination = this.GetDestinationAsString();
            if (!(DestinationEmpty && destination.Equals("")))
            {
                aOutput.Write(" ");
                aOutput.Write(destination);
            }
        }
    }
}