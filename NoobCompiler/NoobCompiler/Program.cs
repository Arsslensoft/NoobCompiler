using System;
using System.Collections.Generic;
using NCAsm;
using NoobCompiler.AST.Declarations;
using NoobCompiler.Contexts;

namespace NoobCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
           TestResolveAndEmit();
         
        }
        public static void TestResolveAndEmit()
        {
            Scanner scanner = new Scanner(@"C:\Users\Arsslen\Desktop\NoobCompiler\NoobCompiler\NoobCompiler\test-correct.txt");
            Parser parser = new Parser(scanner);
            parser.Parse();

            ResolveContext rc = new ResolveContext();
            var n = parser.Unit.DoResolve(rc);
            var ac = CreateAsmContext(@"C:\Users\Arsslen\Desktop\NoobCompiler\NoobCompiler\NoobCompiler\test.s");
            EmitContext ec = CreateEmit(ac);

            ec.SetCurrentResolve(rc);
          //  ec.ag.IsFlat = true;
            ec.ag.OLevel = 3;
            ((ProgramDeclaration)n).Emit(ec);

            ec.Emit();
            
            ac.AssemblerWriter.Flush();
            ac.AssemblerWriter.Close();
        }
        public static void TestResolve()
        {
            Scanner scanner = new Scanner(@"C:\Users\Arsslen\Desktop\NoobCompiler\NoobCompiler\NoobCompiler\test.txt");
            Parser parser = new Parser(scanner);
            parser.Parse();
            if (parser.errors.count == 0)
            {
                ResolveContext rc = new ResolveContext();
                parser.Unit.DoResolve(rc);
            }
        }
        public static AsmContext CreateAsmContext(string output)
        {
            return new AsmContext(new AssemblyWriter(output));
        }
        public static EmitContext CreateEmit(AsmContext actx)
        {
            return new EmitContext(actx);
        }
    }
}
