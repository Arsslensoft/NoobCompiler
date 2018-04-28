using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using NCAsm;
using NCAsm.x86;
using NoobCompiler.AST.Expressions;
using NoobCompiler.Contexts;
using Expression = NoobCompiler.AST.Expressions.Expression;

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

        /// <summary>
        /// Emit code
        /// </summary>
        /// <returns>Success or fail</returns>
        public override bool Emit(EmitContext ec)
        {
            if (Expression is IntegralExpression)
                EmitIfConstant(ec);
            else
            {
                // emit expression branchable
                ec.EmitComment("if-expression evaluation");
                Expression.EmitBranchable(ec, Else, false);
                ec.EmitComment("(" + Expression.CommentString() + ") is true");
                TrueStatement.Emit(ec);
                ec.EmitInstruction(new Jump() { DestinationLabel = ExitIf.Name });
                ec.MarkLabel(Else);
                ec.EmitComment("Else ");
                FalseStatement.Emit(ec);
                ec.MarkLabel(ExitIf);
            }
            return true;
        }

        void EmitIfConstant(EmitContext ec)
        {
            IntegralExpression ce = null;

            if (Expression is IntegralExpression)
                ce = (IntegralExpression)Expression;

            bool val = ce.Value !=0;
            if (!val) // emit else
                FalseStatement.Emit(ec);
            else TrueStatement.Emit(ec);
        }

    }
}
