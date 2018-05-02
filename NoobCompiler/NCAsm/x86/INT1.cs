namespace NCAsm.x86 {

  // See note in Int3 as to why we need a separate op for Int1 versus Int 0x01
  [NCAsm.OpCode("Int1")]
	public class INT1: Instruction { 
  }

}
