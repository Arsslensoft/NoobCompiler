﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("or")]
    public class Or : InstructionWithDestinationAndSourceAndSize {
         public Or()
        {
            OptimizingBehaviour = OptimizationKind.None;
        }
    }
}