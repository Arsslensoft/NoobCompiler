using System;
using NoobCompiler.AST.Declarations;

namespace NoobCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            Scanner scanner = new Scanner(@"C:\Users\Arsslen\Desktop\NoobCompiler\NoobCompiler\NoobCompiler\test.txt");
            Parser parser = new Parser(scanner);
            parser.Parse();
            Console.WriteLine("Hello World!");


            Console.Read();
        }
    }
}
