using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NCAsm.x86;
using NoobCompiler.Contexts;

namespace NoobCompiler.Compiler.Specs
{
    public enum ReferenceKind
    {
        Field,
        LocalVariable,
        Parameter,
        Method,
        Register
    }

    public class ReferenceEmitter : ReferenceSpec
    {


        public ReferenceEmitter(MemberSpec ms, int off, ReferenceKind k)
            : base(k)
        {

            Member = ms;
            Offset = off;
        }
        public ReferenceEmitter(MemberSpec ms, int off, ReferenceKind k, RegistersEnum reg)
            : this(ms, off, k)
        {
            Register = reg;

        }

        public override bool EmitFromStack(EmitContext ec)
        {
            ec.EmitComment("Pop Reference Parameter @BP " + Offset);
            if (InitialIndex != Offset)
            {
                BaseEmitter.Offset = InitialIndex;
                return BaseEmitter.ValueOfStackAccess(ec, Offset - InitialIndex);
            }
            else
            {
                BaseEmitter.Offset = Offset;
                return BaseEmitter.ValueOfStack(ec);
            }

        }
        public override bool EmitToStack(EmitContext ec)
        {
            ec.EmitComment("Push Reference Parameter @ " + BaseEmitter.ToString() + " " + InitialIndex.ToString() + "  " + Offset);


            if (InitialIndex != Offset)
            {
                BaseEmitter.Offset = InitialIndex;
                return BaseEmitter.ValueOfAccess(ec, Offset - InitialIndex);
            }
            else
            {
                BaseEmitter.Offset = Offset;
                return BaseEmitter.ValueOf(ec);
            }


        }
        public override bool LoadEffectiveAddress(EmitContext ec)
        {

            ec.EmitComment("AddressOf Reference @BP+" + Offset);
            ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });
            ec.EmitPush(EmitContext.SI);
            return true;
        }
        public override bool ValueOf(EmitContext ec)
        {
            ec.EmitComment("ValueOf Reference @BP+" + Offset);
            ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.DI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });
            ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.DI });

                ec.EmitPush(EmitContext.SI, 16, true);
        

            return true;
        }
        public override bool ValueOfStack(EmitContext ec)
        {

            ec.EmitComment("ValueOf Reference Stack @BP+" + Offset);
            ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.DI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });
            ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.DI });
            ec.EmitPop(EmitContext.SI,16, true);

            return true;
        }
    }
    public class WordEmitter : ReferenceSpec
    {

        public WordEmitter(MemberSpec ms, int off, ReferenceKind k) : base(k)
        {

            Member = ms;
            Offset = off;
        }
        public WordEmitter(MemberSpec ms, int off, ReferenceKind k, RegistersEnum reg)
            : this(ms, off, k)
        {
            Register = reg;

        }

        public override bool EmitFromStack(EmitContext ec)
        {
            if (ReferenceType == ReferenceKind.Field)
            {
                ec.EmitComment("Pop Field @" + Signature.ToString() + " " + Offset);
                ec.EmitInstruction(new Pop() { DestinationRef = ElementReference.New(Signature.ToString()), DestinationDisplacement = Offset, DestinationIsIndirect = true, Size = 16 });
            }
            else if (ReferenceType == ReferenceKind.LocalVariable)
            {
                ec.EmitComment("Pop Var @BP" + Offset);
                ec.EmitInstruction(new Pop() { Size = 16, DestinationReg = EmitContext.BP, DestinationDisplacement = Offset, DestinationIsIndirect = true });
            }
            else if (ReferenceType == ReferenceKind.Register)
            {
                ec.EmitComment("Pop Var @" + Register.ToString() + Offset);
                ec.EmitInstruction(new Pop() { Size = 16, DestinationReg = Register, DestinationDisplacement = Offset, DestinationIsIndirect = true });
            }
            else
            {
                ec.EmitComment("Pop Parameter @BP " + Offset);
                ec.EmitInstruction(new Pop() { DestinationReg = EmitContext.BP, Size = 16, DestinationDisplacement = Offset, DestinationIsIndirect = true });
            }
            return true;
        }
        public override bool EmitToStack(EmitContext ec)
        {

            if (ReferenceType == ReferenceKind.Field)
            {
                ec.EmitComment("Push Field @" + Signature.ToString() + " " + Offset);
                ec.EmitInstruction(new Push() { DestinationRef = ElementReference.New(Signature.ToString()), DestinationDisplacement = Offset, DestinationIsIndirect = true, Size = 16 });
            }
            else if (ReferenceType == ReferenceKind.LocalVariable)
            {
                ec.EmitComment("Push Var @BP" + Offset);
                ec.EmitInstruction(new Push() { DestinationReg = EmitContext.BP, DestinationDisplacement = Offset, DestinationIsIndirect = true, Size = 16 });
            }
            else if (ReferenceType == ReferenceKind.Register)
            {
                ec.EmitComment("Push Var @" + Register.ToString() + Offset);
                ec.EmitInstruction(new Push() { DestinationReg = Register, DestinationDisplacement = Offset, DestinationIsIndirect = true, Size = 16 });

            }
            else
            {
                ec.EmitComment("Push Parameter @BP " + Offset);
                ec.EmitInstruction(new Push() { DestinationReg = EmitContext.BP, Size = 16, DestinationDisplacement = Offset, DestinationIsIndirect = true });
            }
            return true;
        }
        public override bool LoadEffectiveAddress(EmitContext ec)
        {
            if (ReferenceType == ReferenceKind.Field)
            {
                ec.EmitComment("AddressOf Field ");

                ec.EmitInstruction(new Lea() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, SourceDisplacement = Offset, Size = 16, SourceRef = ElementReference.New(Signature.ToString()) });
                ec.EmitPush(EmitContext.SI);
            }
            else if (ReferenceType == ReferenceKind.LocalVariable)
            {

                ec.EmitComment("AddressOf @BP" + Offset);
                ec.EmitInstruction(new Lea() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });
                ec.EmitPush(EmitContext.SI);
            }
            else if (ReferenceType == ReferenceKind.Register)
            {
                ec.EmitComment("AddressOf @" + Register.ToString() + Offset);
                ec.EmitInstruction(new Lea() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = Register, SourceDisplacement = Offset });
                ec.EmitPush(EmitContext.SI);
            }
            else
            {
                ec.EmitComment("AddressOf @BP+" + Offset);
                ec.EmitInstruction(new Lea() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });
                ec.EmitPush(EmitContext.SI);
            }
            return true;
        }
        public override bool ValueOf(EmitContext ec)
        {
            if (ReferenceType == ReferenceKind.Field)
            {
                ec.EmitComment("ValueOf Field ");
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceRef = ElementReference.New(Signature.ToString()) });
                ec.EmitPush(EmitContext.SI, 16, true);

            }
            else if (ReferenceType == ReferenceKind.LocalVariable)
            {
                ec.EmitComment("ValueOf @BP" + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });
                ec.EmitPush(EmitContext.SI, 16, true);
            }
            else if (ReferenceType == ReferenceKind.Register)
            {
                ec.EmitComment("ValueOf @" + Register.ToString() + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = Register, SourceDisplacement = Offset });
                ec.EmitPush(EmitContext.SI, 16, true);
            }
            else
            {
                ec.EmitComment("ValueOf @BP+" + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });

                ec.EmitPush(EmitContext.SI, 16, true);
            }
            return true;
        }
        public override bool ValueOfStack(EmitContext ec)
        {
            if (ReferenceType == ReferenceKind.Field)
            {
                ec.EmitComment("ValueOf Field ");
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceRef = ElementReference.New(Signature.ToString()) });
                ec.EmitPop(EmitContext.SI, 16, true);
            }
            else if (ReferenceType == ReferenceKind.LocalVariable)
            {
                ec.EmitComment("ValueOf Stack @BP" + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });
                ec.EmitPop(EmitContext.SI, 16, true);

            }
            else if (ReferenceType == ReferenceKind.Register)
            {
                ec.EmitComment("ValueOf Stack @" + Register.ToString() + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = Register, SourceDisplacement = Offset });
                ec.EmitPop(EmitContext.SI, 16, true);

            }
            else
            {
                ec.EmitComment("ValueOf  Stack @BP+" + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });

                ec.EmitPop(EmitContext.SI, 16, true);

            }
            return true;
        }
        public override bool ValueOfAccess(EmitContext ec, int off)
        {
            if (ReferenceType == ReferenceKind.Field)
            {
                ec.EmitComment("ValueOf Access Field ");
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceRef = ElementReference.New(Signature.ToString()) });
                ec.EmitPush(EmitContext.SI, 16, true, off);

            }
            else if (ReferenceType == ReferenceKind.LocalVariable)
            {
                ec.EmitComment("ValueOf Access @BP" + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });
                ec.EmitPush(EmitContext.SI, 16, true, off);
            }
            else if (ReferenceType == ReferenceKind.Register)
            {
                ec.EmitComment("ValueOf Access @" + Register.ToString() + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = Register, SourceDisplacement = Offset });
                ec.EmitPush(EmitContext.SI, 16, true, off);

            }
            else
            {
                ec.EmitComment("ValueOf Access @BP+" + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });

                ec.EmitPush(EmitContext.SI, 16, true, off);
            }
            return true;
        }
        public override bool ValueOfStackAccess(EmitContext ec, int off)
        {
            if (ReferenceType == ReferenceKind.Field)
            {
                ec.EmitComment("ValueOf Access Field ");
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceRef = ElementReference.New(Signature.ToString()) });
                ec.EmitPop(EmitContext.SI, 16, true, off);

            }
            else if (ReferenceType == ReferenceKind.LocalVariable)
            {
                ec.EmitComment("ValueOf Stack Access @BP" + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });
                ec.EmitPop(EmitContext.SI, 16, true, off);

            }
            else if (ReferenceType == ReferenceKind.Register)
            {
                ec.EmitComment("ValueOf Stack Access @" + Register.ToString() + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = Register, SourceDisplacement = Offset });
                ec.EmitPop(EmitContext.SI, 16, true, off);


            }
            else
            {
                ec.EmitComment("ValueOf Access Stack @BP+" + Offset);
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.SI, SourceIsIndirect = true, Size = 16, SourceReg = EmitContext.BP, SourceDisplacement = Offset });

                ec.EmitPop(EmitContext.SI, 16, true, off);
            }
            return true;
        }
    }
    public abstract class ReferenceSpec : IEmitter
    {

        public RegistersEnum Register { get; set; }

        public ReferenceSpec BaseEmitter { get; set; }


        public static ReferenceSpec GetEmitter(MemberSpec ms,int idx, ReferenceKind k, bool reference = false)
        {
            ReferenceSpec Emitter = null;


            if (reference)
            {
                Emitter = new ReferenceEmitter(ms, 4, ReferenceKind.Parameter);
                Emitter.BaseEmitter = GetEmitter(new ParameterSpec( ms.Name, (ms as ParameterSpec).MethodHost,  ms.Signature.Location, idx),  idx, ReferenceKind.Parameter);
            }
            else
                Emitter = new WordEmitter(ms, idx, k);


            return Emitter;
        }
        public int InitialIndex { get; set; }
        public virtual int Offset { get; set; }
        public MemberSignature Signature
        {
            get { return Member.Signature; }
        }
        public MemberSpec Member { get; set; }
        public ReferenceKind ReferenceType { get; set; }
        public ReferenceSpec(ReferenceKind rs)
        {
            ReferenceType = rs;
        }


        public virtual bool LoadEffectiveAddress(EmitContext ec)
        {
            return true;
        }

        public virtual bool ValueOfAccess(EmitContext ec, int off)
        {
            return true;
        }

        public virtual bool ValueOfStackAccess(EmitContext ec, int off)
        {
            return true;
        }

        public virtual bool ValueOf(EmitContext ec)
        {
            return true;
        }
        public virtual bool ValueOfStack(EmitContext ec)
        {
            return true;
        }

        public virtual bool EmitToStack(EmitContext ec) { return true; }
        public virtual bool EmitFromStack(EmitContext ec) { return true; }

        public bool PushAllFromRegister(EmitContext ec, RegistersEnum rg, int size, int offset = 0)
        {
            int s = size / 2;

            if (size % 2 != 0)
            {
                ec.EmitInstruction(new Mov() { DestinationReg = RegistersEnum.DL, SourceReg = rg, SourceDisplacement = offset - 1 + size, SourceIsIndirect = true, Size = 8 });
                ec.EmitPush(RegistersEnum.DX);
            }
            for (int i = s - 1; i >= 0; i--)
                ec.EmitInstruction(new Push() { DestinationReg = rg, DestinationDisplacement = offset + 2 * i, DestinationIsIndirect = true, Size = 16 });

            return true;
        }
        public bool PopAllToRegister(EmitContext ec, RegistersEnum rg, int size, int offset = 0)
        {

            int s = size / 2;


            for (int i = 0; i < s; i++)
                ec.EmitInstruction(new Pop() { DestinationReg = rg, DestinationDisplacement = offset + 2 * i, DestinationIsIndirect = true, Size = 16 });
            if (size % 2 != 0)
            {
                ec.EmitPop(RegistersEnum.DX);
                ec.EmitInstruction(new Mov() { DestinationReg = rg, DestinationDisplacement = offset - 1 + size, DestinationIsIndirect = true, Size = 8, SourceReg = RegistersEnum.DL });

            }
            return true;
        }
        public bool PushAllFromRef(EmitContext ec, ElementReference re, int size, int offset = 0)
        {
            ec.EmitComment("Push Field [TypeOf " + re.Name + "] Offset=" + offset);
            int s = size / 2;

            if (size % 2 != 0)
            {
                ec.EmitInstruction(new Mov() { DestinationReg = RegistersEnum.DL, SourceRef = re, SourceDisplacement = offset + size - 1, SourceIsIndirect = true, Size = 8 });
                ec.EmitPush(RegistersEnum.DX);
            }

            for (int i = s - 1; i >= 0; i--)
                ec.EmitInstruction(new Push() { DestinationRef = re, DestinationDisplacement = offset + 2 * i, DestinationIsIndirect = true, Size = 16 });

            return true;
        }
        public bool PopAllToRef(EmitContext ec, ElementReference re, int size, int offset = 0)
        {
            ec.EmitComment("Pop Field [TypeOf " + re.Name + "] Offset=" + offset);
            int s = size / 2;


            for (int i = 0; i < s; i++)
                ec.EmitInstruction(new Pop() { DestinationRef = re, DestinationDisplacement = offset + 2 * i, DestinationIsIndirect = true, Size = 16 });

            if (size % 2 != 0)
            {
                ec.EmitPop(RegistersEnum.DX);
                ec.EmitInstruction(new Mov() { DestinationRef = re, DestinationDisplacement = offset - 1 + size, DestinationIsIndirect = true, Size = 8, SourceReg = RegistersEnum.DL });

            }
            return true;
        }
    }

}
