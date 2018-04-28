using System;
using System.Collections.Generic;
using System.Text;
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
                ResolveContext.Report.Error(10, Location, "cannot evaluate void type in one operand");


            return base.DoResolve(rc);
        }
    }
}
