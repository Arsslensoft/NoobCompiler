using System;
using System.Collections.Generic;
using System.Text;
using NCAsm.x86;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Expressions
{
   public class UnaryOperationExpression : Expression
    {
        public Expression Expression { get; set; }
        public Operators Operator { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            Expression = (Expression)Expression.DoResolve(rc);

            if (Expression is IntegralExpression)
            {
                (Expression as IntegralExpression).Value = ~(Expression as IntegralExpression).Value;
                return Expression;
            }

            if (Expression.IsVoid)
                ResolveContext.Report.Error(110, Location, "cannot evaluate void type in one operand");


            return base.DoResolve(rc);
        }
        public override bool Emit(EmitContext ec)
        {


            Expression.EmitToStack(ec);
            ec.EmitComment("!" + Expression.CommentString());
            ec.EmitPop(RegistersEnum.AX);


            ec.EmitInstruction(new Not() { DestinationReg = RegistersEnum.AX, Size = 80 });
            ec.EmitInstruction(new And() { DestinationReg = RegistersEnum.AX, SourceValue = 1, Size = 80 });
            ec.EmitPush(RegistersEnum.AX);



            return true;
        }
        public override bool EmitToStack(EmitContext ec)
        {
            return Emit(ec);
        }

        public override string CommentString()
        {
            return "!" + Expression.CommentString();
        }
    }
}
