using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    public interface IInstructionWithDestination {
        NCAsm.ElementReference DestinationRef {
            get;
            set;
        }

        RegistersEnum? DestinationReg
        {
            get;
            set;
        }

        ushort? DestinationValue
        {
            get;
            set;
        }

        bool DestinationIsIndirect {
            get;
            set;
        }

        int DestinationDisplacement {
            get;
            set;
        }

        bool DestinationEmpty
        {
            get;
            set;
        }
    }
}
