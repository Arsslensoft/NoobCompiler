﻿using System;
using System.Collections.Generic;
using System.Text;
using NoobCompiler.Contexts;

namespace NoobCompiler.AST.Statements
{
   public class BlockStatement : Statement
    {
        public List<Statement> Statements { get; set; }

        public override INode DoResolve(ResolveContext rc)
        {
            List <Statement> stmt = new List<Statement>();
            foreach (var statement in Statements)
                if(statement != null)
                     stmt.Add((Statement)statement.DoResolve(rc));

            Statements = stmt;
            return this;
        }

        /// <summary>
        /// Emit code
        /// </summary>
        /// <returns>Success or fail</returns>
        public override bool Emit(EmitContext ec)
        {
            foreach (var statement in Statements)
                statement.Emit(ec);
            return base.Emit(ec);
        }
    }
}
