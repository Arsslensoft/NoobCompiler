namespace NCAsm.x86 {
    [NCAsm.OpCode("int")]
    public class INT : InstructionWithDestination {
        public override void WriteText( NCAsm.AsmContext aAssembler, AssemblyWriter aOutput )
        {
          //TODO: In base have a property that has the opcode from above and we can reuse it.
            aOutput.Write("Int " + DestinationValue.Value.ToString("X2") + "h");
        }
    }
}