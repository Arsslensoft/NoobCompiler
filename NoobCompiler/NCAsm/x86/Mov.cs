using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
  [NCAsm.OpCode("mov")]
  public class Mov : InstructionWithDestinationAndSourceAndSize {
      public Mov():base("mov")
      {
      }
  }
  [NCAsm.OpCode("lea")]
  public class Lea : InstructionWithDestinationAndSourceAndSize
  {
      public Lea()
          : base("lea")
      {
      }
  }
}
