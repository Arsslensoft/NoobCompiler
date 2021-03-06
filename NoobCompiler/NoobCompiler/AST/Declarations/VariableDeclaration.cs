﻿using System;
using System.Collections.Generic;
using System.Text;
using NCAsm;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Declarations
{
   public class VariableDeclaration : Declaration
   {
       public MemberSpec Variable;
        public string Name { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            if (rc.CurrentMethod != null)
            {
                var ps = rc.Resolver.ResolveParameter(Name);
                if (ps != null)
                    ResolveContext.Report.Error(106, Location, "a parameter is already defined with that name '" + Name + "'");

                var vs = rc.Resolver.ResolveVar(Name);   
                if (vs != null)
                    ResolveContext.Report.Error(107, Location, "a local variable is already defined with that name '" + Name + "'");

                Variable = new VarSpec(Name, rc.CurrentMethod, Location);
                rc.KnowVar((VarSpec)Variable);
            }
            else
            {
                var fs = rc.Resolver.ResolveField(Name);
                if (fs != null)
                    ResolveContext.Report.Error(107, Location, "a global variable is already defined with that name '" + Name + "'");
                Variable = new FieldSpec(Name, Location);
                rc.KnowField((FieldSpec)Variable);
            }
            return base.DoResolve(rc);
        }

       /// <summary>
       /// Emit code
       /// </summary>
       /// <returns>Success or fail</returns>
       public override bool Emit(EmitContext ec)
       {
            if(Variable is FieldSpec)
                ec.EmitDataWithConv(Variable.Signature.ToString(), (ushort)0, Variable);

            return base.Emit(ec);
       }
   }
}
