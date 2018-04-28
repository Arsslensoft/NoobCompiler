using System;
using System.Collections.Generic;
using System.Text;
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

        public override INode DoResolve(ResolveContext rc)
        {
            Method = new MethodSpec(Name, Parameters.Count,IsFunction,Location);
            MethodSpec m = null;
            rc.Resolver.ResolveMethod(Name, ref m, Parameters.Count);
            if (m != null)
                ResolveContext.Report.Error(8, Location, "Duplicate method signature");
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
    }
}
