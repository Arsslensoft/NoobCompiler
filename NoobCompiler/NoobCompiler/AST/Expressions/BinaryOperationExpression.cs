using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NCAsm.x86;
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
                ResolveContext.Report.Error(110, Location, "cannot evaluate void type in one operand");

            return base.DoResolve(rc);
        }


        /// <summary>Emit code</summary>
        /// <returns>Success or fail</returns>
        public override bool Emit(EmitContext ec)
        {
            Left.EmitToStack(ec);
            //ec.MarkOptimizable(); // Marks last instruction as last push
            Right.EmitToStack(ec);
            //ec.MarkOptimizable(); // Marks last instruction as last push


            ec.EmitComment(Left.CommentString() + " " +GetOperatorString()+" "+ Right.CommentString());
   
            if (Operator == Operators.Add)
            {
                ec.EmitPop(RegistersEnum.CX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Add() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.CX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitPush(RegistersEnum.AX);
            }
            else if (Operator == Operators.Sub)
            {
                ec.EmitPop(RegistersEnum.CX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Sub() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.CX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitPush(RegistersEnum.AX);
            }
            else if (Operator == Operators.Mul)
            {
                ec.EmitPop(RegistersEnum.CX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Multiply() { DestinationReg = RegistersEnum.CX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitPush(RegistersEnum.CX);
            }
            else if (Operator == Operators.Div)
            {
                ec.EmitPop(RegistersEnum.CX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Xor() { DestinationReg = EmitContext.D, SourceReg = EmitContext.D, Size = 80 });
                ec.EmitInstruction(new Divide() { DestinationReg = RegistersEnum.CX, Size = 80 });
                ec.EmitPush(RegistersEnum.CX);
            }
            else if (Operator == Operators.Mod)
            {
                ec.EmitPop(RegistersEnum.CX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Xor() { DestinationReg = EmitContext.D, SourceReg = EmitContext.D, Size = 80 });
                ec.EmitInstruction(new Divide() { DestinationReg = RegistersEnum.CX, Size = 80 });
                ec.EmitInstruction(new Mov() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.DX, Size = 80 });
                ec.EmitPush(RegistersEnum.AX);
            }
            else if (Operator == Operators.And)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new And() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitPush(RegistersEnum.AX);
            }
            else if (Operator == Operators.Or)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Or() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitPush(RegistersEnum.AX);
            }
            // Comparison
            else if (Operator == Operators.Equal)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBoolean(RegistersEnum.AL, ConditionalTestEnum.Equal, ConditionalTestEnum.NotEqual);
                ec.EmitPush(RegistersEnum.AX);
            }
            else if (Operator == Operators.NotEqual)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBoolean(RegistersEnum.AL, ConditionalTestEnum.NotEqual, ConditionalTestEnum.Equal);
                ec.EmitPush(RegistersEnum.AX);
            }
            else if (Operator == Operators.LT)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBoolean(RegistersEnum.AL, ConditionalTestEnum.LessThan, ConditionalTestEnum.GreaterThanOrEqualTo);
                ec.EmitPush(RegistersEnum.AX);
            }
            else if (Operator == Operators.GT)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBoolean(RegistersEnum.AL, ConditionalTestEnum.GreaterThan, ConditionalTestEnum.LessThanOrEqualTo);
                ec.EmitPush(RegistersEnum.AX);
            }
            else if (Operator == Operators.GTE)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBoolean(RegistersEnum.AL, ConditionalTestEnum.GreaterThanOrEqualTo, ConditionalTestEnum.LessThan);
                ec.EmitPush(RegistersEnum.AX);
            }
            else if (Operator == Operators.LTE)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBoolean(RegistersEnum.AL, ConditionalTestEnum.LessThanOrEqualTo, ConditionalTestEnum.GreaterThan);
                ec.EmitPush(RegistersEnum.AX);
            }


            return base.Emit(ec);
        }

        public override bool EmitToStack(EmitContext ec)
        {
            return Emit(ec);
        }

        string GetOperatorString()
        {
            if (Operator == Operators.Add)
                return "+";
            else if (Operator == Operators.Sub)
                return "-";
            else if (Operator == Operators.Mul)
                return "*";
            else if (Operator == Operators.Div)
                return "div";
            else if (Operator == Operators.Mod)
                return "mod";
            else if (Operator == Operators.And)
                return "and";
            else if (Operator == Operators.Or)
                return "or";
            // Comparison
            else if (Operator == Operators.Equal)
                return "==";
            else if (Operator == Operators.NotEqual)
                return "<>";
            else if (Operator == Operators.LT)
                return "<";
            else if (Operator == Operators.GT)
                return ">";
            else if (Operator == Operators.GTE)
                return ">=";
            else if (Operator == Operators.LTE)
                return "<=";

            return "";
        }
        public override bool EmitBranchable(EmitContext ec, Label truecase, bool v)
        {
        
            Left.EmitToStack(ec);
            //ec.MarkOptimizable(); // Marks last instruction as last push
            Right.EmitToStack(ec);
            //ec.MarkOptimizable(); // Marks last instruction as last push


            ec.EmitComment(Operator + "  " + Left.CommentString() + ",  " + Right.CommentString());


            if (Operator == Operators.Equal)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBooleanBranch(v, truecase, ConditionalTestEnum.Equal, ConditionalTestEnum.NotEqual);
            }
            else if (Operator == Operators.NotEqual)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBooleanBranch(v, truecase, ConditionalTestEnum.NotEqual, ConditionalTestEnum.Equal);
            }
            else if (Operator == Operators.LT)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBooleanBranch(v, truecase, ConditionalTestEnum.LessThan, ConditionalTestEnum.GreaterThanOrEqualTo);
            }
            else if (Operator == Operators.GT)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBooleanBranch(v, truecase, ConditionalTestEnum.GreaterThan, ConditionalTestEnum.LessThanOrEqualTo);
    
            }
            else if (Operator == Operators.GTE)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBooleanBranch(v, truecase, ConditionalTestEnum.GreaterThanOrEqualTo, ConditionalTestEnum.LessThan);
            }
            else if (Operator == Operators.LTE)
            {
                ec.EmitPop(RegistersEnum.BX);
                ec.EmitPop(RegistersEnum.AX);
                ec.EmitInstruction(new Compare() { DestinationReg = RegistersEnum.AX, SourceReg = RegistersEnum.BX, Size = 80, OptimizingBehaviour = OptimizationKind.PPO });
                ec.EmitBooleanBranch(v, truecase, ConditionalTestEnum.LessThanOrEqualTo, ConditionalTestEnum.GreaterThan);
            }

            // jumps
            ec.EmitBooleanBranch(v, truecase, ConditionalTestEnum.Equal, ConditionalTestEnum.NotEqual);

            return true;
        }

        public override string CommentString()
        {
            return Left.CommentString() + " " + GetOperatorString() + " " + Right.CommentString();
        }
    }
}
