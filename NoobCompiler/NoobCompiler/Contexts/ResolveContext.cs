using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using NCAsm;
using NoobCompiler.AST;
using NoobCompiler.AST.Declarations;
using NoobCompiler.AST.Expressions;
using NoobCompiler.AST.Statements;
using NoobCompiler.Base;
using NoobCompiler.Base.Reports;

namespace NoobCompiler.Contexts
{
    /// <summary>
    /// Type & members resolver
    /// </summary>
    public class Resolver
    {
        public ResolveContext CurrentContext { get; set; }
        public MethodSpec CurrentMethod { get; set; }
        public Resolver Parent { get; set; }
        public List<FieldSpec> KnownGlobals { get; set; }
        public List<MethodSpec> KnownMethods { get; set; }
        public List<VarSpec> KnownLocalVars { get; set; }
        public Resolver(ResolveContext rc, MethodSpec mtd = null)
        {
            CurrentContext = rc;
            CurrentMethod = mtd;
            Parent = null;
            KnownGlobals = new List<FieldSpec>();
            KnownMethods = new List<MethodSpec>();
            KnownLocalVars = new List<VarSpec>();
        }
        public bool KnowField(FieldSpec f)
        {
            if (!KnownGlobals.Contains(f))
            {
                KnownGlobals.Add(f);
                return true;

            }
            return false;
        }
        public bool KnowVar(VarSpec v)
        {
            if (!KnownLocalVars.Contains(v))
            {
                KnownLocalVars.Add(v);
                return true;

            }
            return false;
        }
        public bool KnowMethod(MethodSpec m)
        {
            if (!KnownMethods.Contains(m))
            {
                KnownMethods.Add(m);
                return true;

            }
            return false;
        }
        public VarSpec ResolveVar(string name)
        {
            foreach (VarSpec kt in KnownLocalVars)
            {
              
                if (kt.Name == name)
                    return kt;
            }
          
            return null;
        }
        public FieldSpec ResolveField(string name)
        {

                foreach (FieldSpec kt in KnownGlobals)
                {

                    if (kt.Name == name)
                        return kt;
                    
                }
            return null;
        }
        public ParameterSpec ResolveParameter(string name)
        {
            if (CurrentMethod == null)
                return null;
            foreach (ParameterSpec kt in CurrentMethod.Parameters)
            {

                if (kt.Name == name)
                    return kt;
                
            }

            return null;
        }
        public MemberSpec TryResolveName(string name)
        {
            MemberSpec m = ResolveVar(name);
            if (m == null)
            {
                m = ResolveParameter(name);
                if (m == null)
                {
                    m = ResolveField(name);
                    if (m == null)
                        return null;

                    else return m;
                }
                else return m;
            }
            else return m;
        }
        public void ResolveMethod( string name, ref MethodSpec mtd, int par = 0)
        {
            bool hastemplate = false;
        
                MemberSignature msig = new MemberSignature(name, par, Location.Null);
                for (int i = 0; i < KnownMethods.Count; i++)
                {
                    if (KnownMethods[i].MatchSignature(msig, name, par, ref hastemplate))
                    {
                        mtd = KnownMethods[i];
                        return;
                    }
                }
        }


    }

    public interface ILoop
    {
        ILoop ParentLoop { get; set; }
        Label EnterLoop { get; set; }
        Label ExitLoop { get; set; }
        Label LoopCondition { get; set; }

    }

    public interface IConditional
    {
        IConditional ParentIf { get; set; }
        Label Else { get; set; }
        Label ExitIf { get; set; }
    }
    public interface IResolve
    {
        INode DoResolve(ResolveContext rc);
        bool Resolve(ResolveContext rc);
    }
    public enum LabelType
    {
        LOOP,
        IF,
        ELSE,
        WHILE,
        LABEL,
        BOOL_EXPR,
        IF_EXPR,
        FLOAT_REM
    }
    [Flags]
    public enum ResolveScopes
    {
        Normal = 1 << 1,
        Loop = 1,
        If = 1 << 2,
    }
    public class ResolverState
    {
        public ResolveScopes CurrentScopes { get; set; }

        public ResolverState( ResolveScopes c)
        {
            CurrentScopes = c;
        }

        public void Restore(ResolveContext rc)
        {
            rc.CurrentGlobalScope = CurrentScopes;
        }
        public static ResolverState Create(ResolveContext rc)
        {
            return new ResolverState(  rc.CurrentGlobalScope);
        }
    }
    public class ResolveContext : IDisposable
    {

        public IConditional EnclosingIf { get; set; }
        public ILoop EnclosingLoop { get; set; }
        public Switch EnclosingSwitch { get; set; }
        public Label DefineLabel(string name)
        {

            return new Label(name);

        }
        public Label DefineLabel(LabelType lbt, string suffix = null)
        {
            if (suffix == null)
                return new Label(EmitContext.GenerateLabelName(lbt));
            else return new Label(EmitContext.GenerateLabelName(lbt) + "_" + suffix);
        }

        BlockStatement current_block;
        static Report rp;

        public static Report Report { get { return rp; } set { rp = value; } }
        static ResolveContext()
        {
            rp = new ConsoleReporter();
        }
        public int AnonymousParameterIdx { get; set; }
        void Init()
        {

            IsInVarDeclaration = false;
            LocalStackIndex = 0;
            AnonymousParameterIdx = 4;
        }
        public ResolveContext(MethodDeclaration decl)
        {

            Init();
            Resolver = new Resolver( this, new MethodSpec(decl.Name,decl.Parameters.Count, decl.IsFunction, decl.Location));
            ChildContexts = new List<ResolveContext>();
        }
        public ResolveContext()
        {
            Init();
            Resolver = new Resolver(this);
            ChildContexts = new List<ResolveContext>();
        }
        public ResolveContext(BlockStatement b, MethodSpec cm)
        {
            Resolver = new Resolver( this, cm);
            current_block = b;
            Init();
        }
        public List<ResolveContext> ChildContexts { get; set; }
       public bool IsInVarDeclaration { get; set; }
        public ResolveScopes CurrentGlobalScope { get; set; }
        public int LocalStackIndex { get; set; }

        public string CurrentMethodName { get; set; }
        public MethodSpec CurrentMethod { get { return Resolver.CurrentMethod; } set { Resolver.CurrentMethod = value; } }
        public ResolverState CurrentStatementState { get; set; }
        public Resolver Resolver { get; set; }
        Stack<ResolverState> states = new Stack<ResolverState>();
        public void CreateNewState()
        {

            states.Push(ResolverState.Create(this));
        }
        public void RestoreOldState()
        {
            if (states.Count > 0)
            {
                ResolverState rs = states.Pop();
                rs.Restore(this);
            }
        }
        public void BackupCurrentAndSetStatement()
        {
            CreateNewState();
            CurrentStatementState.Restore(this);
        }
        public bool Exist(MemberSpec m, List<MemberSpec> l)
        {
            foreach (MemberSpec ms in l)
                if (m.Signature.ToString() == ms.Signature.ToString())
                    return true;

            return false;
        }
        public void KnowMethod(MethodSpec mtd)
        {
            if (!Exist((MemberSpec)mtd, Resolver.KnownMethods.Cast<MemberSpec>().ToList<MemberSpec>()))
                Resolver.KnownMethods.Add(mtd);

        }
        public void KnowField(FieldSpec mtd)
        {
            if (!Exist((MemberSpec)mtd, Resolver.KnownGlobals.Cast<MemberSpec>().ToList<MemberSpec>()))
                Resolver.KnownGlobals.Add(mtd);
        }
        public void FillKnownByKnown(Resolver kn)
        {
            foreach (FieldSpec fs in kn.KnownGlobals)
                KnowField(fs);

            foreach (MethodSpec ms in kn.KnownMethods)
                KnowMethod(ms);
        }
        public bool KnowVar(VarSpec mtd)
        {
            if (!Exist((MemberSpec)mtd, Resolver.KnownLocalVars.Cast<MemberSpec>().ToList<MemberSpec>()))
            {
                LocalStackIndex -= 2;
                mtd.VariableStackIndex = LocalStackIndex;
                mtd.InitialStackIndex = LocalStackIndex;
                Resolver.KnowVar(mtd);
                return true;
            }
            else return false;
        }
        public bool KnowVar(ParameterSpec mtd)
        {
            if (!Exist((MemberSpec)mtd, CurrentMethod.Parameters.Cast<MemberSpec>().ToList<MemberSpec>()))
            {

                mtd.StackIdx = AnonymousParameterIdx;
                mtd.InitialStackIndex = AnonymousParameterIdx;
                CurrentMethod.Parameters.Add(mtd);
                return true;
            }
            else return false;
        }

        public ResolveContext CreateAsChild( MethodDeclaration md)
        {
            if (ChildContexts != null)
            {
                ResolveContext rc = new ResolveContext(md);
                rc.FillKnownByKnown(Resolver);
                ChildContexts.Add(rc);
                return rc;

            }
            else return null;
        }
        public void UpdateFather(ResolveContext rc)
        {
            foreach (MethodSpec m in rc.Resolver.KnownMethods)
                KnowMethod(m);
         
            foreach (FieldSpec m in rc.Resolver.KnownGlobals)
                KnowField(m);
        }
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        #region IDisposable Members

        public void Dispose()
        {

        }

        #endregion
    }
}
