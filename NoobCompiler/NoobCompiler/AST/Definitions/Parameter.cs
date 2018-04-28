using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Definitions
{
  public class Parameter : Node
    {
        public ParameterSpec ParameterName { get; set; }
        public string Name { get; set; }
        public bool IsVariable { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            var ps = rc.Resolver.ResolveParameter(Name);
            if(ps != null)
                ResolveContext.Report.Error(105, Location, "duplicate parameter definition '" + Name + "'");

            ParameterName = new ParameterSpec(Name, rc.CurrentMethod, Location, 4, IsVariable);
            
            rc.KnowVar(ParameterName);
            return base.DoResolve(rc);
        }
    }
}
