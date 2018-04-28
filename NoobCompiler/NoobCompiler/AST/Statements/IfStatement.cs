using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NoobCompiler.AST.Expressions;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Statements
{
   public class IfStatement : Statement, IConditional
    {
        public IConditional ParentIf { get; set; }
        public Label ExitIf { get; set; }
        public Label Else { get; set; }


        public Expression Expression { get; set; }
        public Statement TrueStatement { get; set; }
        public Statement FalseStatement { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            Label lb = rc.DefineLabel(LabelType.IF);
            ExitIf = rc.DefineLabel(lb.Name + "_EXIT");
            Else = rc.DefineLabel(lb.Name + "_ELSE");
            ParentIf = rc.EnclosingIf;
            rc.EnclosingIf = this;


            rc.CreateNewState();
            rc.CurrentGlobalScope |= ResolveScopes.If;

            // enter if
            Expression = (Expression) Expression.DoResolve(rc);
            TrueStatement = (Statement) TrueStatement.DoResolve(rc);
            FalseStatement = (Statement)FalseStatement.DoResolve(rc);

            if(Expression.IsVoid)
                ResolveContext.Report.Error(3, Location, "cannot evaluate void type in if statement");

            rc.RestoreOldState();
            // exit current if
            rc.EnclosingIf = ParentIf;


            return base.DoResolve(rc);
        }
    }
}
