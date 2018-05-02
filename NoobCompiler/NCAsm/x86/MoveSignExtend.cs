namespace NCAsm.x86 {
    [NCAsm.OpCode("movsx")]
	public class MoveSignExtend : InstructionWithDestinationAndSourceAndSize
	{

		public override void WriteText(NCAsm.AsmContext aAssembler, AssemblyWriter aOutput)
		{
			if (Size == 0)
			{
				Size = 32;
			}
			aOutput.Write(mMnemonic);
			if (!DestinationEmpty)
			{
				aOutput.Write(" ");
				aOutput.Write(this.GetDestinationAsString());
				aOutput.Write(", ");
				aOutput.Write(SizeToString(Size));
				aOutput.Write(" ");
				aOutput.Write(this.GetSourceAsString());
			}
		}
	}
}