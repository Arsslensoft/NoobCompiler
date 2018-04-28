using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NCAsm.x86;
using NoobCompiler.AST.Expressions;
using NoobCompiler.Base;
using NoobCompiler.Compiler.Specs;

namespace NoobCompiler.Contexts
{
    public enum CallingConventions
    {
        Cdecl = 1,
        StdCall = 2,
        FastCall = 3,
        Pascal = 4,
        Default = 0
    }
    public class CallingConventionsHandler
    {
        public static void EmitLongReturnRoutine(EmitContext ec, bool will_return)
        {
            int return_size = 2;


            if (return_size > 2)
                ec.EmitInstruction(new Sub() { DestinationReg = EmitContext.SP, SourceValue = (ushort)return_size });
        }
        public static void EmitNoReturnPop(EmitContext ec)
        {
            int return_size = 2;

            if (return_size > 2)
                ec.EmitInstruction(new Add() { DestinationReg = EmitContext.SP, SourceValue = (ushort)return_size });
        }
        int GetParameterSize(bool reference)
        {
                return 2;
           
        }
        void HandleLeftToRight(ref List<ParameterSpec> L_R, ref int last_param)
        {
            int paramidx = 4; // Initial Stack Position
            for (int i = L_R.Count - 1; i >= 0; i--)
            {
                L_R[i].StackIdx = paramidx;
                L_R[i].InitialStackIndex = paramidx;

                paramidx += GetParameterSize(L_R[i].IsReference);

            }
            last_param = paramidx;
        }
        void HandleRightToLeft(ref List<ParameterSpec> L_R, CallingConventions ccv, ref int last_param)
        {
            if (ccv != CallingConventions.FastCall)
            {
                int paramidx = 4; // Initial Stack Position
                for (int i = 0; i < L_R.Count; i++)
                {
                    L_R[i].StackIdx = paramidx;
                    L_R[i].InitialStackIndex = paramidx;

                    paramidx += GetParameterSize(L_R[i].IsReference);

                }
                last_param = paramidx;
            }
            else if (L_R.Count > 2 && CallingConventions.FastCall == ccv)
            {
                int paramidx = 4; // Initial Stack Position
                for (int i = 2; i < L_R.Count; i++)
                {
                    L_R[i].StackIdx = paramidx;
                    L_R[i].InitialStackIndex = paramidx;
                    paramidx += GetParameterSize(L_R[i].IsReference);
                }

                last_param = paramidx;
            }


        }
        public void EmitFastCall(EmitContext ec, int par)
        {
            if (par >= 2)
            {
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.BP, DestinationDisplacement = -2, DestinationIsIndirect = true, SourceReg = EmitContext.C });
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.BP, DestinationDisplacement = -4, DestinationIsIndirect = true, SourceReg = EmitContext.D });
            }
            else if (par == 1)
                ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.BP, DestinationDisplacement = -2, DestinationIsIndirect = true, SourceReg = EmitContext.C });

        }
        public void ReserveFastCall(ResolveContext rc, List<ParameterSpec> par)
        {
            if (par.Count >= 2)
            {
                rc.LocalStackIndex -= 2;
                par[0].StackIdx = rc.LocalStackIndex;
                par[0].InitialStackIndex = rc.LocalStackIndex;
                rc.LocalStackIndex -= 2;
                par[1].StackIdx = rc.LocalStackIndex;
                par[1].InitialStackIndex = rc.LocalStackIndex;
            }
            else if (par.Count == 1)
            {
                rc.LocalStackIndex -= 2;
                par[0].StackIdx = rc.LocalStackIndex;
                par[0].InitialStackIndex = rc.LocalStackIndex;
            }

        }


        public void SetParametersIndex(ref List<ParameterSpec> L_R, CallingConventions ccv, ref int param_idx)
        {
            if (ccv == CallingConventions.StdCall || ccv == CallingConventions.Cdecl || ccv == CallingConventions.FastCall)
                HandleRightToLeft(ref L_R, ccv, ref param_idx);
            else if (ccv == CallingConventions.Pascal || ccv == CallingConventions.Default)
                HandleLeftToRight(ref L_R, ref param_idx);

        }
        public void EmitDecl(EmitContext ec, ref List<ParameterSpec> L_R, CallingConventions ccv)
        {
            int size = 0;
            foreach (ParameterSpec p in L_R)
            {

                size += GetParameterSize(p.IsReference);
            }

            if (ccv == CallingConventions.StdCall || ccv == CallingConventions.Pascal || ccv == CallingConventions.Default)
            {
                if (size > 0)
                    ec.EmitInstruction(new Return() { DestinationValue = (ushort)size });
                else ec.EmitInstruction(new SimpleReturn());
            }
            else if (ccv == CallingConventions.Cdecl)
                ec.EmitInstruction(new SimpleReturn());
            else if (ccv == CallingConventions.FastCall)
            {
                if (size > 4)
                    ec.EmitInstruction(new Return() { DestinationValue = (ushort)(size - 4) });
                else ec.EmitInstruction(new SimpleReturn());
            }




        }
        public void EmitCall(EmitContext ec, List<Expression> exp, MethodSpec method, bool will_return)
        {
            EmitLongReturnRoutine(ec, will_return);

            int size = 0;
            if (method.CallingConvention == CallingConventions.Pascal || method.CallingConvention == CallingConventions.Default)
            {
                foreach (Expression e in exp)
                    e.EmitToStack(ec);
            }
            else if (method.CallingConvention == CallingConventions.StdCall || method.CallingConvention == CallingConventions.Cdecl)
            {

                for (int i = exp.Count - 1; i >= 0; i--)
                {
                    exp[i].EmitToStack(ec);
                    size += GetParameterSize(false);
                }

            }
            else if (method.CallingConvention == CallingConventions.FastCall)
            {
                for (int i = exp.Count - 1; i >= 0; i--)
                {
                    exp[i].EmitToStack(ec);
                    size += GetParameterSize(false);
                }
                if (exp.Count >= 2)
                {
                    ec.EmitPop(EmitContext.C);
                    ec.EmitPop(EmitContext.D);
                }
                else if (exp.Count == 1)
                    ec.EmitPop(EmitContext.C);

            }
            // call
            ec.EmitCall(method);

            if (method.CallingConvention == CallingConventions.Cdecl && size > 0)
                ec.EmitInstruction(new Add() { DestinationReg = EmitContext.SP, SourceValue = (ushort)size, Size = 80 });

            if (!will_return)
                EmitNoReturnPop(ec);
        }
        public void EmitCall(EmitContext ec, List<Expression> exp, MemberSpec method, CallingConventions ccv, bool will_return)
        {
            EmitLongReturnRoutine(ec, will_return);
            int size = 0;
            if (ccv == CallingConventions.Pascal || ccv == CallingConventions.Default)
            {
                foreach (Expression e in exp)
                {
                    e.EmitToStack(ec);
                 
                }
            }
            else if (ccv == CallingConventions.StdCall || ccv == CallingConventions.Cdecl)
            {

                for (int i = exp.Count - 1; i >= 0; i--)
                {
                    exp[i].EmitToStack(ec);

                    size += GetParameterSize( false);


                }

            }
            else if (ccv == CallingConventions.FastCall)
            {
                for (int i = exp.Count - 1; i >= 0; i--)
                {

                    exp[i].EmitToStack(ec);

                    size += GetParameterSize( false);

              
                }
                if (exp.Count >= 2)
                {
                    ec.EmitPop(EmitContext.C);
                    ec.EmitPop(EmitContext.D);
                }
                else if (exp.Count == 1)
                    ec.EmitPop(EmitContext.C);

            }

            // call
            method.EmitToStack(ec);
            ec.EmitPop(RegistersEnum.AX);
            ec.EmitInstruction(new Call() { DestinationReg = RegistersEnum.AX });

            if (ccv == CallingConventions.Cdecl  && size > 0)
                ec.EmitInstruction(new Add() { DestinationReg = EmitContext.SP, SourceValue = (ushort)size, Size = 80 });

            if (!will_return)
                EmitNoReturnPop( ec);
        }

    }
    /// <summary>
    /// Member Signature [Types, member, variable]
    /// </summary>
    public struct MemberSignature : IEquatable<MemberSignature>
    {

        string _signature;
        public string Signature { get { return _signature; } }
        Location _loc;
        public Location Location { get { return _loc; } }
  
        public MemberSignature( string name,int parameters, Location loc)
        {
            _signature = name + "_" + parameters;
            _loc = loc;
        }
        public MemberSignature(string name, Location loc)
        {
            _signature = name;
            _loc = loc;

        }
        public static bool operator !=(MemberSignature a, MemberSignature b)
        {
            return a.Signature != b.Signature;
        }
        public static bool operator ==(MemberSignature a, MemberSignature b)
        {
            return a.Signature == b.Signature;
        }

        public bool Equals(MemberSignature ns)
        {
            return ns.Signature == Signature;
        }
        public override bool Equals(object obj)
        {
            if (obj is MemberSignature)
                return Signature == ((MemberSignature)obj).Signature;
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override string ToString()
        {
            return Signature;
        }


    }
    public abstract class MemberSpec : IEmitter
    {
        protected MemberSignature _sig;
        protected string _name;

    

        public MemberSpec(string name, MemberSignature sig)
        {
            _name = name;
            _sig = sig;
        }


        public ElementReference Reference { get; set; }
        /// <summary>
        /// Member name
        /// </summary>
        public virtual string Name
        {
            get { return _name; }
            internal set { _name = value; }
        }
   
        /// <summary>
        /// Member Signature for struct_node_
        /// </summary>
        public MemberSignature Signature
        {
            get { return _sig; }
            internal set
            {
                _sig = value;
            }

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

    }

    /// <summary>
    /// Local Variable Specs
    /// </summary>
    public class VarSpec : MemberSpec, IEquatable<VarSpec>
    {

        MethodSpec method;


        public int VariableStackIndex
        {
            get
            {
                return Emitter.Offset;
            }
            set
            {
                Emitter.Offset = value;
            }

        }
        public int InitialStackIndex
        {
            get
            {
                return Emitter.InitialIndex;

            }
            set
            {

                Emitter.InitialIndex = value;
            }
        }
        public ReferenceSpec Emitter { get; set; }
        public bool Initialized { get; set; }
        public MethodSpec MethodHost
        {
            get
            {
                return method;
            }
        }

        public VarSpec(string name, MethodSpec host,Location loc)
            : base(name, new MemberSignature(host.Name + "_" + name, loc))
        {
            method = host;
            Initialized = false;
            Emitter = ReferenceSpec.GetEmitter(this,  0, ReferenceKind.LocalVariable);
            InitialStackIndex = 0;

        }
        public override string ToString()
        {
            return Signature.ToString();
        }
        public override bool EmitToStack(EmitContext ec)
        {

            return Emitter.EmitToStack(ec);
        }
        public override bool EmitFromStack(EmitContext ec)
        {

            return Emitter.EmitFromStack(ec);
        }

        public override bool LoadEffectiveAddress(EmitContext ec)
        {


            return Emitter.LoadEffectiveAddress(ec);
        }
        public override bool ValueOf(EmitContext ec)
        {

            return Emitter.ValueOf(ec);
        }
        public override bool ValueOfStack(EmitContext ec)
        {

            return Emitter.ValueOfStack(ec);
        }

        public override bool ValueOfAccess(EmitContext ec, int off)
        {
            return Emitter.ValueOfAccess(ec, off);
        }

        public override bool ValueOfStackAccess(EmitContext ec, int off)
        {
            return Emitter.ValueOfStackAccess(ec, off);
        }


        public bool Equals(VarSpec tp)
        {
            return tp.Signature == Signature;
        }

    }


    /// <summary>
    /// Method Specs
    /// </summary>
    public class MethodSpec : MemberSpec, IEquatable<MethodSpec>
    {
        public CallingConventions CallingConvention
        {
            get { return CallingConventions.StdCall; }
        }
        public List<ParameterSpec> Parameters { get; set; }
   
        public ushort LastParameterEndIdx { get; set; }

        public bool IsFunction { get; set; }

        public MethodSpec(string name, int param, bool isFunction, Location loc)
            : base(name, new MemberSignature(name, param, loc))
        {
            IsFunction = isFunction;
               Parameters = new List<ParameterSpec>();
            LastParameterEndIdx = 0;

        }
        public override string ToString()
        {
            return Signature.ToString();
        }

        public bool Equals(MethodSpec tp)
        {
            return tp.Signature == Signature;
        }

        public bool MatchSignature(MemberSignature msig, string name, int param, ref bool hastemplate)
        {
            hastemplate = false;

            if (msig == Signature)
                return true;
            else if (Name == name)
            {
                if (param == 0)
                    return true;
                else
                    return param == Parameters.Count;     
            }
            else return false;
        }

    }

    /// <summary>
    /// Parameter Variable Specs
    /// </summary>
    public class ParameterSpec : MemberSpec, IEquatable<ParameterSpec>
    {

        MethodSpec method;

        public int StackIdx
        {
            get
            {
                return Emitter.Offset;

            }
            set
            {
                Emitter.Offset = value;
            }
        }
        public int InitialStackIndex
        {
            get
            {
                return Emitter.InitialIndex;

            }
            set
            {
                if (Emitter == null)
                {

                }
                Emitter.InitialIndex = value;
            }
        }
        public ReferenceSpec Emitter { get; set; }



        public MethodSpec MethodHost
        {
            get
            {
                return method;
            }
        }
 
        public bool IsReference { get; set; }
        public ParameterSpec(string name, MethodSpec host,  Location loc, int initstackidx, bool isReference = false)
            : base(name, new MemberSignature(host.Name + "_param_" + name, loc))
        {
            method = host;
            IsReference = isReference;
            Emitter = ReferenceSpec.GetEmitter(this, 4, ReferenceKind.Parameter, isReference);
            InitialStackIndex = initstackidx;
        }
        public override string ToString()
        {
            return Signature.ToString();
        }

        public override bool EmitToStack(EmitContext ec)
        {

            return Emitter.EmitToStack(ec);
        }
        public override bool EmitFromStack(EmitContext ec)
        {

            return Emitter.EmitFromStack(ec);
        }

        public override bool LoadEffectiveAddress(EmitContext ec)
        {


            return Emitter.LoadEffectiveAddress(ec);
        }
        public override bool ValueOf(EmitContext ec)
        {

            return Emitter.ValueOf(ec);
        }
        public override bool ValueOfStack(EmitContext ec)
        {

            return Emitter.ValueOfStack(ec);
        }

        public override bool ValueOfAccess(EmitContext ec, int off)
        {
            return Emitter.ValueOfAccess(ec, off);
        }

        public override bool ValueOfStackAccess(EmitContext ec, int off)
        {
            return Emitter.ValueOfStackAccess(ec, off);
        }


        public bool Equals(ParameterSpec tp)
        {
            return tp.Signature == Signature;
        }


    }

    /// <summary>
    /// Global Variable Specs
    /// </summary>
    public class FieldSpec : MemberSpec, IEquatable<FieldSpec>
    {
       public ReferenceSpec Emitter { get; set; }
     
       public int FieldOffset
        {

            get
            {
                return Emitter.Offset;
            }
            set
            {
                Emitter.Offset = value;
            }
        }
        public int InitialFieldIndex
        {
            get
            {
                return Emitter.InitialIndex;

            }
            set
            {

                Emitter.InitialIndex = value;
            }
        }
        public FieldSpec(string name,Location loc)
            : base(name, new MemberSignature(name, loc))
        {
            Emitter = ReferenceSpec.GetEmitter(this,  0, ReferenceKind.Field);
            InitialFieldIndex = 0;
        }

        public override bool EmitToStack(EmitContext ec)
        {

            return Emitter.EmitToStack(ec);
        }
        public override bool EmitFromStack(EmitContext ec)
        {

            return Emitter.EmitFromStack(ec);
        }

        public override bool LoadEffectiveAddress(EmitContext ec)
        {


            return Emitter.LoadEffectiveAddress(ec);
        }
        public override bool ValueOf(EmitContext ec)
        {

            return Emitter.ValueOf(ec);
        }
        public override bool ValueOfStack(EmitContext ec)
        {

            return Emitter.ValueOfStack(ec);
        }

        public override bool ValueOfAccess(EmitContext ec, int off)
        {
            return Emitter.ValueOfAccess(ec, off);
        }

        public override bool ValueOfStackAccess(EmitContext ec, int off)
        {
            return Emitter.ValueOfStackAccess(ec, off);
        }
        public override string ToString()
        {
            return Signature.ToString();
        }
        public bool Equals(FieldSpec tp)
        {
            return tp.Signature == Signature;
        }

    }
}
