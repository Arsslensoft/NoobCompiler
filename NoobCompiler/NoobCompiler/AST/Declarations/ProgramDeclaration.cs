using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.AST.Statements;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Declarations
{
   public class ProgramDeclaration : Declaration
    {
        public string Name { get; set; }
        public List<VariableDeclaration> Variables { get; set; }
        public List<MethodDeclaration> Methods { get; set; }

        public Statement Block { get; set; }


        public override INode DoResolve(ResolveContext rc)
        {
            var vd = new List<VariableDeclaration>();
            foreach (var variableDeclaration in Variables)
                vd.Add((VariableDeclaration)variableDeclaration.DoResolve(rc));
            
            Variables = vd;

            var md = new List<MethodDeclaration>();
            foreach (var methodDeclaration in Methods)
            {
                ResolveContext childctx = rc.CreateAsChild(methodDeclaration);
                MethodDeclaration d = (MethodDeclaration)methodDeclaration.DoResolve(childctx);
                md.Add(d);
                rc.UpdateFather(childctx);
            }
            Methods = md;

            Block = (Statement)Block.DoResolve(rc);

            return base.DoResolve(rc);
        }
    }
}
