using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NoobCompiler.Base;

namespace NoobCompiler.Contexts
{

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
        public int VariableStackIndex { get; set; }
        public int InitialStackIndex { get; set; }

        //public int VariableStackIndex
        //{
        //    get
        //    {
        //        return Emitter.Offset;
        //    }
        //    set
        //    {
        //        Emitter.Offset = value;
        //    }

        //}
        //public int InitialStackIndex
        //{
        //    get
        //    {
        //        return Emitter.InitialIndex;

        //    }
        //    set
        //    {

        //        Emitter.InitialIndex = value;
        //    }
        //}
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
           // InitialStackIndex = 0;

        }
        public override string ToString()
        {
            return Signature.ToString();
        }
        //public override bool EmitToStack(EmitContext ec)
        //{

        //    return Emitter.EmitToStack(ec);
        //}
        //public override bool EmitFromStack(EmitContext ec)
        //{

        //    return Emitter.EmitFromStack(ec);
        //}

        //public override bool LoadEffectiveAddress(EmitContext ec)
        //{


        //    return Emitter.LoadEffectiveAddress(ec);
        //}
        //public override bool ValueOf(EmitContext ec)
        //{

        //    return Emitter.ValueOf(ec);
        //}
        //public override bool ValueOfStack(EmitContext ec)
        //{

        //    return Emitter.ValueOfStack(ec);
        //}


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
    /// Local Variable Specs
    /// </summary>
    public class ParameterSpec : MemberSpec, IEquatable<ParameterSpec>
    {
        public int StackIdx { get; set; }
        public int InitialStackIndex { get; set; }
        MethodSpec method;

        //public int StackIdx
        //{
        //    get
        //    {
        //        return Emitter.Offset;

        //    }
        //    set
        //    {
        //        Emitter.Offset = value;
        //    }
        //}
        //public int InitialStackIndex
        //{
        //    get
        //    {
        //        return Emitter.InitialIndex;

        //    }
        //    set
        //    {
        //        if (Emitter == null)
        //        {

        //        }
        //        Emitter.InitialIndex = value;
        //    }
        //}
        //public ReferenceSpec Emitter { get; set; }


        public MethodSpec MethodHost
        {
            get
            {
                return method;
            }
        }
        public bool IsParameter
        {
            get
            {
                return true;
            }
        }

        public ParameterSpec(string name, MethodSpec host,  Location loc, int initstackidx)
            : base(name, new MemberSignature(host.Name + "_param_" + name, loc))
        {
            method = host;


            //Emitter = ReferenceSpec.GetEmitter(this, memberType, 4, ReferenceKind.Parameter, access, IsReference);


            //InitialStackIndex = initstackidx;
        }
        public override string ToString()
        {
            return Signature.ToString();
        }

        //public override bool EmitToStack(EmitContext ec)
        //{

        //    return Emitter.EmitToStack(ec);
        //}
        //public override bool EmitFromStack(EmitContext ec)
        //{

        //    return Emitter.EmitFromStack(ec);
        //}

        //public override bool LoadEffectiveAddress(EmitContext ec)
        //{


        //    return Emitter.LoadEffectiveAddress(ec);
        //}
        //public override bool ValueOf(EmitContext ec)
        //{

        //    return Emitter.ValueOf(ec);
        //}
        //public override bool ValueOfStack(EmitContext ec)
        //{

        //    return Emitter.ValueOfStack(ec);
        //}

  
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
       /* public ReferenceSpec Emitter { get; set; }
     
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
        }*/
        public FieldSpec(string name,Location loc)
            : base(name, new MemberSignature(name, loc))
        {

/*
            Emitter = ReferenceSpec.GetEmitter(this, memberType, 0, ReferenceKind.Field, access);
            InitialFieldIndex = 0;*/
        }

     /*   public override bool EmitToStack(EmitContext ec)
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
        }*/
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
