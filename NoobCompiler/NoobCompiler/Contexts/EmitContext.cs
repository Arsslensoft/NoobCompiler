using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NCAsm.x86;

namespace NoobCompiler.Contexts
{
    public interface IEmitter
    {


        bool EmitToStack(EmitContext ec);
        bool EmitFromStack(EmitContext ec);
        bool ValueOf(EmitContext ec);
        bool ValueOfStack(EmitContext ec);
        bool LoadEffectiveAddress(EmitContext ec);
    }
    /// <summary>
    /// Special Emit for expressions
    /// </summary>
    public interface IEmitExpr
    {
        bool EmitToStack(EmitContext ec);
        bool EmitFromStack(EmitContext ec);
        bool EmitToRegister(EmitContext ec, RegistersEnum rg);
        bool EmitBranchable(EmitContext ec, Label truecase, bool val);
    }

    public interface IEmitAddress
    {
        bool LoadEffectiveAddress(EmitContext ec);

    }
    /// <summary>
    /// Basic Emit for CodeGen
    /// </summary>
    public interface IEmit
    {
        /// <summary>
        /// Emit code
        /// </summary>
        /// <returns>Success or fail</returns>
        bool Emit(EmitContext ec);


        /// <summary>
        /// Emit 3 address code
        /// </summary>
        /// <returns>Success or fail</returns>
        bool EmitIntermediate(IntermediateEmitContext ec);
    }
    public class IntermediateEmitContext
    {
        public static Dictionary<LabelType, int> Labels = new Dictionary<LabelType, int>();
        public static string GenerateLabelName(LabelType lb)
        {
            if (Labels.ContainsKey(lb))
            {
                Labels[lb]++;
                return lb.ToString() + "_" + Labels[lb].ToString();
            }
            else
            {
                Labels.Add(lb, 0);
                return lb.ToString() + "_" + Labels[lb].ToString();
            }
        }

    }
    public class EmitContext
    {
        public static Dictionary<LabelType, int> Labels = new Dictionary<LabelType, int>();
        public static string GenerateLabelName(LabelType lb)
        {
            if (Labels.ContainsKey(lb))
            {
                Labels[lb]++;
                return lb.ToString() + "_" + Labels[lb].ToString();
            }
            else
            {
                Labels.Add(lb, 0);
                return lb.ToString() + "_" + Labels[lb].ToString();
            }
        }

        public const RegistersEnum A = RegistersEnum.AX;
        public const RegistersEnum B = RegistersEnum.BX;
        public const RegistersEnum C = RegistersEnum.CX;
        public const RegistersEnum D = RegistersEnum.DX;
        public const RegistersEnum SP = RegistersEnum.SP;
        public const RegistersEnum BP = RegistersEnum.BP;
        public const RegistersEnum DI = RegistersEnum.DI;
        public const RegistersEnum SI = RegistersEnum.SI;
        internal AsmContext ag;
        List<string> _names;

        Dictionary<string, VarSpec> Variables = new Dictionary<string, VarSpec>();
        public RegistersEnum GetLow(RegistersEnum reg)
        {
            return ag.GetLow(reg);
        }
        public RegistersEnum GetHigh(RegistersEnum reg)
        {
            return ag.GetHigh(reg);
        }

        public void EmitSubRoutinePush(EmitContext ec)
        {
            ec.EmitComment("Sub routine push");
            ec.EmitPush(A);
        }
        public void EmitData(DataMember dm, MemberSpec v, bool constant = false)
        {

            if (!Variables.ContainsKey(v.Signature.ToString()))
            {
                v.Reference = ElementReference.New(v.Signature.ToString());
                if (constant)
                    ag.DefineConstantData(dm);
                else ag.DefineData(dm);

            }
        }
        public void Emit()
        {
            ag.Emit(ag.AssemblerWriter);
        }
        public void EmitDataWithConv(string name, MemberSpec v, string value, bool isglobal = false)
        {
            DataMember dm = new DataMember(name, value.ToString()) { IsGlobal = isglobal };
            EmitData(dm, v, false);
        }
        public const byte FALSE = 0;
        public const byte TRUE = 1;
        public void EmitDataWithConv(string name, object value, MemberSpec v, bool constant = false, bool verbatim = false, bool isglobal = false)
        {
            DataMember dm;
            if (value is string)
            {
                if (constant)
                    dm = new DataMember(name, value.ToString(), true, verbatim) { IsGlobal = isglobal };
                else dm = new DataMember(name, value.ToString(), false, verbatim) { IsGlobal = isglobal };
            }
            else if (value is float)
                dm = new DataMember(name, BitConverter.GetBytes((float)value)) { IsGlobal = isglobal };
            else if (value is byte[])
                dm = new DataMember(name, (byte[])value) { IsGlobal = isglobal };
            else if (value is bool)
                dm = new DataMember(name, ((bool)value) ? (new byte[1] { EmitContext.TRUE }) : (new byte[1] { 0 })) { IsGlobal = isglobal };
            else if (value is byte)
                dm = new DataMember(name, new byte[1] { (byte)value }) { IsGlobal = isglobal };
            else if (value is ushort)
                dm = new DataMember(name, new ushort[1] { (ushort)value }) { IsGlobal = isglobal };
            else dm = new DataMember(name, new object[1] { value }) { IsGlobal = isglobal };

            EmitData(dm, v, constant);
        }
      
        EmitContext()
        {

            _names = new List<string>();
        }
        public void SetEntry(string name)
        {
            ag.EntryPoint = name;
        }
        public EmitContext(AssemblyWriter asmw)
        {
            ag = new AsmContext(asmw);
        }

        public EmitContext(AsmContext ac)
        {
            ag = ac;
        }
        public void SetCurrentResolve(ResolveContext rc)
        {
            CurrentResolve = rc;
        }
        public ResolveContext CurrentResolve { get; set; }
        public bool RedirectToAnonymous
        {
            get { return ag.RedirectToAnonymous; }
            set { ag.RedirectToAnonymous = value; }
        }
        public void EmitInstruction(NCAsm.Instruction ins)
        {
            ag.Emit(ins);

        }
        #region Stack
        public void EmitPop(RegistersEnum rg, byte size = 80, bool adr = false, int off = 0)
        {
            if (Registers.Is8Bit(rg))
                rg = ag.GetHolder(rg);
            if (size == 8)
            {
                ag.SetAsUsed(rg);
                RegistersEnum drg = ag.GetNextRegister();
                ag.FreeRegister();
                ag.FreeRegister();

                EmitInstruction(new Pop() { DestinationReg = drg, Size = 16 });
                if (off != 0)
                    EmitInstruction(new Mov() { DestinationReg = rg, Size = 8, DestinationIsIndirect = adr, DestinationDisplacement = off, SourceReg = GetLow(drg) });
                else EmitInstruction(new Mov() { DestinationReg = rg, Size = 8, DestinationIsIndirect = adr, SourceReg = GetLow(drg) });
            }
            else
            {
                if (off != 0)
                    EmitInstruction(new Pop() { DestinationReg = rg, Size = 16, DestinationIsIndirect = adr, DestinationDisplacement = off });
                else EmitInstruction(new Pop() { DestinationReg = rg, Size = 16, DestinationIsIndirect = adr });
            }
        }
        public void EmitPush(bool v)
        {
            EmitInstruction(new Push() { DestinationValue = (v ? (ushort)EmitContext.TRUE : (ushort)0), Size = 16 });
        }
        public void EmitPush(byte v)
        {
            EmitInstruction(new Push() { DestinationValue = v, Size = 16 });
        }
        public void EmitPush(ushort v)
        {
            EmitInstruction(new Push() { DestinationValue = v, Size = 16 });
        }
        public void EmitPush(RegistersEnum rg, byte size = 80, bool adr = false, int off = 0)
        {
            if (Registers.Is8Bit(rg))
                rg = ag.GetHolder(rg);
            if (size == 8)
            {
                ag.SetAsUsed(rg);
                RegistersEnum drg = ag.GetNextRegister();
                ag.FreeRegister();
                ag.FreeRegister();
                EmitInstruction(new MoveZeroExtend() { DestinationReg = drg, Size = 8, SourceReg = rg, SourceDisplacement = off, SourceIsIndirect = adr });

                EmitInstruction(new Push() { DestinationReg = drg, Size = 16 });
            }
            else
                EmitInstruction(new Push() { DestinationReg = rg, Size = 16, DestinationIsIndirect = adr, DestinationDisplacement = off });
        }
        public void EmitPushSigned(RegistersEnum rg, byte size = 80, bool adr = false, int off = 0)
        {
            if (Registers.Is8Bit(rg))
                rg = ag.GetHolder(rg);
            if (size == 8)
            {
                ag.SetAsUsed(rg);
                RegistersEnum drg = ag.GetNextRegister();
                ag.FreeRegister();
                ag.FreeRegister();
                EmitInstruction(new MoveSignExtend() { DestinationReg = drg, Size = 8, SourceReg = rg, SourceDisplacement = off, SourceIsIndirect = adr });

                EmitInstruction(new Push() { DestinationReg = drg, Size = 16 });
            }
            else
                EmitInstruction(new Push() { DestinationReg = rg, Size = 16, DestinationIsIndirect = adr, DestinationDisplacement = off });
        }
        #endregion
        #region Mov
        public void EmitMovFromRegister(RegistersEnum rg, RegistersEnum src, byte size = 80, bool adr = false, int off = 0)
        {


            if (size == 8)
            {
                if (off != 0)
                    EmitInstruction(new Mov() { DestinationReg = rg, Size = 8, DestinationIsIndirect = adr, DestinationDisplacement = off, SourceReg = GetLow(src) });
                else EmitInstruction(new Mov() { DestinationReg = rg, Size = 8, DestinationIsIndirect = adr, SourceReg = GetLow(src) });
            }
            else
            {
                if (off != 0)
                    EmitInstruction(new Mov() { DestinationReg = rg, Size = 16, DestinationIsIndirect = adr, DestinationDisplacement = off, SourceReg = src });
                else EmitInstruction(new Mov() { DestinationReg = rg, Size = 16, DestinationIsIndirect = adr, SourceReg = src });
            }
        }
        public void EmitMovToRegister(RegistersEnum dst, bool v)
        {
            EmitInstruction(new Mov() { DestinationReg = dst, SourceValue = (v ? (ushort)EmitContext.TRUE : (ushort)0), Size = 16 });
        }
        public void EmitMovToRegister(RegistersEnum dst, byte v)
        {
            EmitInstruction(new Mov() { DestinationReg = dst, SourceValue = v, Size = 16 });
        }
        public void EmitMovToRegister(RegistersEnum dst, ushort v)
        {
            EmitInstruction(new Mov() { DestinationReg = dst, SourceValue = v, Size = 16 });
        }
        public void EmitMovToRegister(RegistersEnum rg, RegistersEnum src, byte size = 80, bool adr = false, int off = 0)
        {
            if (size == 8)
                EmitInstruction(new MoveZeroExtend() { DestinationReg = rg, Size = 8, SourceReg = src, SourceDisplacement = off, SourceIsIndirect = adr });
            else
                EmitInstruction(new Mov() { DestinationReg = rg, Size = 16, SourceReg = src, SourceDisplacement = off, SourceIsIndirect = adr });
        }
        public void EmitMovToRegisterSigned(RegistersEnum rg, RegistersEnum src, byte size = 80, bool adr = false, int off = 0)
        {
            if (size == 8)
                EmitInstruction(new MoveSignExtend() { DestinationReg = rg, Size = 8, SourceReg = src, SourceDisplacement = off, SourceIsIndirect = adr });
            else
                EmitInstruction(new Mov() { DestinationReg = rg, Size = 16, SourceReg = src, SourceDisplacement = off, SourceIsIndirect = adr });
        }
        #endregion

        public void MarkLabel(Label lb)
        {
            ag.MarkLabel(lb);
            if (ag.Externals.Contains(lb.Name))
                ag.Externals.Remove(lb.Name);
        }
        public Label DefineLabel(string name)
        {
            return ag.DefineLabel(name);
        }
        public Label DefineLabel()
        {
            return ag.DefineLabel(GenerateLabelName(LabelType.LABEL));
        }
        public Label DefineLabel(LabelType lbt, string suffix = null)
        {
            if (suffix == null)
                return ag.DefineLabel(GenerateLabelName(lbt));
            else return ag.DefineLabel(GenerateLabelName(lbt) + "_" + suffix);
        }

        public void EmitLoadFloat(RegistersEnum rg, byte size = 32, bool adr = false, int off = 0)
        {
            EmitInstruction(new NCAsm.x86.x87.FloatLoad() { DestinationReg = rg, Size = 16, DestinationIsIndirect = adr, DestinationDisplacement = off });
        }
        public void EmitStoreFloat(RegistersEnum rg, byte size = 32, bool adr = false, int off = 0)
        {
            EmitInstruction(new NCAsm.x86.x87.FloatStoreAndPop() { DestinationReg = rg, Size = size, DestinationIsIndirect = adr, DestinationDisplacement = off });
        }


        public void EmitBooleanBranch(bool v, Label truecase, ConditionalTestEnum tr, ConditionalTestEnum fls)
        {
            if (v)
                EmitInstruction(new ConditionalJump() { Condition = tr, DestinationLabel = truecase.Name });
            else EmitInstruction(new ConditionalJump() { Condition = fls, DestinationLabel = truecase.Name });

        }
        public void EmitBoolean(RegistersEnum rg, ConditionalTestEnum tr, ConditionalTestEnum fls)
        {
            //  EmitInstruction(new Xor() { SourceReg = ag.GetHolder(rg), DestinationReg =  ag.GetHolder(rg), Size = 80 });
            EmitInstruction(new ConditionalSet() { Condition = tr, DestinationReg = GetLow(rg), Size = 80 });
            EmitInstruction(new MoveZeroExtend() { SourceReg = GetLow(rg), DestinationReg = ag.GetHolder(rg), Size = 80 });
            /*
              EmitInstruction(new ConditionalMove() { Condition = tr, DestinationReg = rg, Size = 80, SourceValue = TRUE });
              EmitInstruction(new ConditionalMove() { Condition = fls, DestinationReg = rg, Size = 80, SourceValue = 0 });
             * 
             * */
        }
        public void EmitBooleanWithJump(RegistersEnum rg, ConditionalTestEnum TR)
        {
            string lbname = EmitContext.GenerateLabelName(LabelType.BOOL_EXPR);
            Label truelb = DefineLabel(lbname + "_TRUE");
            Label falselb = DefineLabel(lbname + "_FALSE");
            Label boolexprlb = DefineLabel(lbname + "_END");
            // jumps
            EmitInstruction(new ConditionalJump() { Condition = TR, DestinationLabel = truelb.Name });
            EmitInstruction(new Jump() { DestinationLabel = falselb.Name }); // false
            // emit true and false
            // true
            MarkLabel(truelb);
            EmitInstruction(new Mov() { DestinationReg = EmitContext.A, SourceValue = TRUE, Size = 8 });
            EmitInstruction(new Jump() { DestinationLabel = boolexprlb.Name }); // exit
            // false
            MarkLabel(falselb);
            EmitInstruction(new Mov() { DestinationReg = EmitContext.A, SourceValue = 0, Size = 8 });
            // mark exit
            MarkLabel(boolexprlb);
        }
        public void EmitBoolean(RegistersEnum rg, bool v)
        {
            EmitInstruction(new Mov() { DestinationReg = rg, SourceValue = (v ? (ushort)EmitContext.TRUE : (ushort)0), Size = 80 });
        }
        public void EmitCall(MethodSpec m)
        {
            EmitInstruction(new Call() { DestinationLabel = m.Signature.ToString() });
        }
        public void EmitComment(string str)
        {
            ag.Emit(new Comment(ag, str));
        }


    }
}
