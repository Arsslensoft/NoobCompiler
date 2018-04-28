using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using NCAsm;
using NCAsm.x86;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Expressions
{
  public class VariableExpression : Expression
    {
        public MemberSpec variable;
        public string Name { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            if (variable == null)
            {
                variable = rc.Resolver.TryResolveName(Name);
                if (variable == null)
                    ResolveContext.Report.Error(101, Location, "Unresolved variable '" + Name + "'");
            }
            return base.DoResolve(rc);
        }

        public virtual bool LoadEffectiveAddress(EmitContext ec)
        {
            return variable.LoadEffectiveAddress(ec);

        }


        public override bool EmitFromStack(EmitContext ec)
        {
            return variable.EmitFromStack(ec);

        }
        public override bool EmitToStack(EmitContext ec)
        {



            return variable.EmitToStack(ec);
        }

        public override bool EmitBranchable(EmitContext ec, Label truecase, bool v)
        {
            EmitToStack(ec);
            ec.EmitPop(EmitContext.A);
            ec.EmitInstruction(new Compare() { DestinationReg = EmitContext.A, SourceValue = (ushort)1 });

            ec.EmitBooleanBranch(v, truecase, ConditionalTestEnum.Equal, ConditionalTestEnum.NotEqual);
            return true;
        }
        public override string CommentString()
        {
            return Name;
        }
    }
}
