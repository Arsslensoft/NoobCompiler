using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Compiler;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Expressions
{
   public class BinaryOperationExpression : Expression
    {
        public Expression Left { get; set; }
        public Expression Right { get; set; }
        public Operators Operator { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            Right = (Expression)Right.DoResolve(rc);
            Left = (Expression)Left.DoResolve(rc);

            if (Left is IntegralExpression && Right is IntegralExpression)
                return new ConstantFolding(Operator, this).TryEvaluate().DoResolve(rc);

            if(Left.IsVoid || Right.IsVoid)
                ResolveContext.Report.Error(10, Location, "cannot evaluate void type in one operand");

            return base.DoResolve(rc);
        }
    }
}
