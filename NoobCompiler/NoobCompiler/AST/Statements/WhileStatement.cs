using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NCAsm.x86;
using NoobCompiler.AST.Expressions;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Statements
{
   public class WhileStatement : Statement, ILoop
    {
        public Label EnterLoop { get; set; }
        public Label ExitLoop { get; set; }
        public Label LoopCondition { get; set; }
        public ILoop ParentLoop { get; set; }

        public Expression Expression { get; set; }
        public Statement Statement { get; set;  }

        public override INode DoResolve(ResolveContext rc)
        {
            rc.CreateNewState();
            rc.CurrentGlobalScope |= ResolveScopes.Loop;
            Label lb = rc.DefineLabel(LabelType.WHILE);
            ExitLoop = rc.DefineLabel(lb.Name + "_EXIT");
            LoopCondition = rc.DefineLabel(lb.Name + "_COND");
            EnterLoop = rc.DefineLabel(lb.Name + "_ENTER");
            ParentLoop = rc.EnclosingLoop;
            // enter loop


            Expression = (Expression)Expression.DoResolve(rc);
            Statement = (Statement)Statement.DoResolve(rc);


            if (Expression.IsVoid)
                ResolveContext.Report.Error(104, Location, "cannot evaluate void type in while statement");


            if (Expression is IntegralExpression && (Expression as IntegralExpression).Value != 0)
                ResolveContext.Report.Warning( Location, "Infinite loop");
            rc.RestoreOldState();
            // exit current loop
            rc.EnclosingLoop = ParentLoop;

            return base.DoResolve(rc);
        }
        bool infinite = false;
        void EmitConstantLoop(EmitContext ec)
        {

            IntegralExpression ce = null;

            if (Expression is IntegralExpression)
                ce = (IntegralExpression)Expression;


            bool val = ce.Value != 0;
            if (val)
            { // if true
                infinite = true;

                ec.MarkLabel(EnterLoop);

                Statement.Emit(ec);
                ec.EmitInstruction(new Jump() { DestinationLabel = EnterLoop.Name });
                ec.MarkLabel(ExitLoop);
            }
        }
        public override bool Emit(EmitContext ec)
        {
            if (Expression is IntegralExpression)
                EmitConstantLoop(ec);
            else
            {
                ec.EmitInstruction(new Jump() { DestinationLabel = LoopCondition.Name });

                ec.MarkLabel(EnterLoop);

                Statement.Emit(ec);

                ec.MarkLabel(LoopCondition);
                // emit expression branchable
                Expression.EmitBranchable(ec, EnterLoop, true);
                // exit
                ec.MarkLabel(ExitLoop);
            }
            return true;
        }
    }
}
