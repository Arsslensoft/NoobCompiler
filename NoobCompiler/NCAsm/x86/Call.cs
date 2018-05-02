using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.x86 {
    [NCAsm.OpCode("call")]
	public class Call: JumpBase {
        public Call():base("call") {
            mNear = false;
        }
	}
}