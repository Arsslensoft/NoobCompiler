using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NCAsm.x86;
using NoobCompiler.AST.Definitions;
using NoobCompiler.AST.Statements;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Declarations
{
  public class MethodDeclaration : Declaration
  {
      public MethodSpec Method;
        public bool IsFunction { get; set; }
        public string Name { get; set; }
        public List<Parameter> Parameters { get; set; }
        public List<VariableDeclaration> LocalVariables { get; set; }
        public Statement Block { get; set; }
      private CallingConventionsHandler ccvh;
       
        public override INode DoResolve(ResolveContext rc)
        {
            ccvh = new CallingConventionsHandler();

            Method = new MethodSpec(Name, Parameters.Count,IsFunction,Location);
            MethodSpec m = null;
            rc.Resolver.ResolveMethod(Name, ref m, Parameters.Count);
            if (m != null)
                ResolveContext.Report.Error(108, Location, "Duplicate method signature");
            else
                rc.KnowMethod(Method);
            rc.CurrentMethod = Method;
            Method.LastParameterEndIdx = (ushort)(2 * Parameters.Count + 2);

            var paramaters = new List<Parameter>();
            foreach (var parameter in Parameters)
                paramaters.Add((Parameter)parameter.DoResolve(rc));

            Parameters = paramaters;

            var locals = new List<VariableDeclaration>();
            foreach (var variableDeclaration in LocalVariables)
                locals.Add((VariableDeclaration)variableDeclaration.DoResolve(rc));

            LocalVariables = locals;

            Block = (Statement) Block.DoResolve(rc);

            return base.DoResolve(rc);
        }

      /// <summary>
      /// Emit code
      /// </summary>
      /// <returns>Success or fail</returns>
      public override bool Emit(EmitContext ec)
      {
          Label mlb = ec.DefineLabel(Method.Signature.ToString());
          mlb.Method = true;
          ec.MarkLabel(mlb);
          ec.EmitComment("Method: Name = " + Method.Name);
          // create stack frame
          ec.EmitComment("create stackframe");
          ec.EmitInstruction(new Push() { DestinationReg = EmitContext.BP, Size = 80 });
          ec.EmitInstruction(new Mov() { DestinationReg = EmitContext.BP, SourceReg = EmitContext.SP, Size = 80 });
          // allocate variables

          ushort size = 0;
          List<VarSpec> locals = ec.CurrentResolve.GetLocals();
          foreach (VarSpec v in locals)
              size += 2;


           
            if (size != 0)         // no allocation
                ec.EmitInstruction(new Sub() { DestinationReg = EmitContext.SP, SourceValue = size, Size = 80 });
            //EMit params
            var parameters = new List<ParameterSpec>();
            if (Parameters.Count > 0)
            {
                ec.EmitComment("Parameters Definitions");
                foreach (var par in Parameters)
                {
                    ec.EmitComment("Parameter " + par.Name + " @BP" + par.ParameterName.StackIdx);
                    parameters.Add(par.ParameterName);
                }
            }

            if (locals.Count > 0)
            {
                ec.EmitComment("Local Vars Definitions");
                foreach (VarSpec v in locals)
                    ec.EmitComment("Local " + v.Name + " @BP" + v.VariableStackIndex);
            }
            ec.EmitComment("Block");
            // Emit Code
            if (Block != null)
                Block.Emit(ec);

            ec.EmitComment("return label");
            // Return Label
            ec.MarkLabel(ec.DefineLabel(Method.Signature + "_ret"));
       
            // Destroy Stack Frame
            ec.EmitComment("destroy stackframe");
            ec.EmitInstruction(new Leave());
            // ret
            ccvh.EmitDecl(ec, ref parameters, CallingConventions.StdCall);
            return true;
    
      }
  }
}
