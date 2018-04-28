using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST;
using NoobCompiler.AST.Expressions;
using NoobCompiler.Base;

namespace NoobCompiler.Compiler
{
    public class ConstantFolding
    {
        #region Constant Folding
        readonly Operators _op;
        readonly BinaryOperationExpression _operation;
        public Location Location
        {
            get
            {
                return _operation.Location;
            }
        }
        public ConstantFolding(Operators op, BinaryOperationExpression oper)
        {
            _op = op;
            _operation = oper;
        }
        int GetValue(Expression rexp, Expression lexp)
        {
            IntegralExpression lce = ((IntegralExpression)lexp);
            IntegralExpression rce = ((IntegralExpression)rexp);


            if (_op == Operators.Add)
                return (lce.Value + rce.Value);
            else if (_op == Operators.Sub)
                return (lce.Value - rce.Value);
            else if (_op == Operators.Mul)
                return (lce.Value * rce.Value);
            else if (_op == Operators.Div)
                return (lce.Value / rce.Value);
            else if (_op == Operators.Mod)
                return (lce.Value % rce.Value);
            else if (_op == Operators.And)
                return (lce.Value & rce.Value);
            else if (_op == Operators.Or)
                return (lce.Value | rce.Value);
            else return EvalueComparison(lce.Value, rce.Value) ? 1 : 0;

        }


        bool EvalueComparison(int lv, int rv)
        {
            if (_op == Operators.Equal)
                return lv == rv;
            else if (_op == Operators.NotEqual)
                return lv != rv;
            else if (_op == Operators.GT)
                return lv > rv;
            else if (_op == Operators.LT)
                return lv < rv;
            else if (_op == Operators.GTE)
                return lv >= rv;
            else if (_op == Operators.LTE)
                return lv <= rv;
            else return false;
        }
     
        public Expression TryEvaluate()
        {
            try
            {
                return new IntegralExpression()
                {
                    Value = GetValue(_operation.Right, _operation.Left),
                    Location = _operation.Location
                };
            }
            catch
            {
                return _operation;
            }

        }

        #endregion
    }
}
