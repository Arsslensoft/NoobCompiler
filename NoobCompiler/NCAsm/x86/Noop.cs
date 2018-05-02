namespace NCAsm.x86
{
    [NCAsm.OpCode("NOP")]
    public class Noop : Instruction
    {
    }

    [NCAsm.OpCode("NOP ; INT3")]
    public class DebugNoop : Instruction
    {
    }
}