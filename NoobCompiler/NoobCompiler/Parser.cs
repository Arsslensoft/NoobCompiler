using System;
using System.Collections.Generic;
using NoobCompiler.AST;
using NoobCompiler.AST.Declarations;
using NoobCompiler.AST.Definitions;
using NoobCompiler.AST.Expressions;
using NoobCompiler.AST.Statements;
using NoobCompiler.Base;





using System;



public class Parser
{
    public const int _EOF = 0;
    public const int _ident = 1;
    public const int _nb = 2;
    public const int _prog = 3;
    public const int _var = 4;
    public const int _int = 5;
    public const int _func = 6;
    public const int _proc = 7;
    public const int _if = 8;
    public const int _then = 9;
    public const int _else = 10;
    public const int _while = 11;
    public const int _do = 12;
    public const int _and = 13;
    public const int _mod = 14;
    public const int _div = 15;
    public const int _or = 16;
    public const int _not = 17;
    public const int _comma = 18;
    public const int _semicolon = 19;
    public const int _colon = 20;
    public const int _star = 21;
    public const int _lpar = 22;
    public const int _rpar = 23;
    public const int _lbrack = 24;
    public const int _rbrace = 25;
    public const int _dot = 26;
    public const int _plus = 27;
    public const int _minus = 28;
    public const int _lbrace = 29;
    public const int _rbrack = 30;
    public const int maxT = 39;

    const bool _T = true;
    const bool _x = false;
    const int minErrDist = 2;

    public Scanner scanner;
    public Errors errors;

    public Token t;    // last recognized token
    public Token la;   // lookahead token
    int errDist = minErrDist;

    CompilationUnit Unit;




    public Parser(Scanner scanner)
    {
        this.scanner = scanner;
        errors = new Errors();
    }

    void SynErr(int n)
    {
        if (errDist >= minErrDist) errors.SynErr(la.line, la.col, n);
        errDist = 0;
    }

    public void SemErr(string msg)
    {
        if (errDist >= minErrDist) errors.SemErr(t.line, t.col, msg);
        errDist = 0;
    }

    void Get()
    {
        for (; ; )
        {
            t = la;
            la = scanner.Scan();
            if (la.kind <= maxT) { ++errDist; break; }

            la = t;
        }
    }

    void Expect(int n)
    {
        if (la.kind == n) Get(); else { SynErr(n); }
    }

    bool StartOf(int s)
    {
        return set[s, la.kind];
    }

    void ExpectWeak(int n, int follow)
    {
        if (la.kind == n) Get();
        else
        {
            SynErr(n);
            while (!StartOf(follow)) Get();
        }
    }


    bool WeakSeparator(int n, int syFol, int repFol)
    {
        int kind = la.kind;
        if (kind == n) { Get(); return true; }
        else if (StartOf(repFol)) { return false; }
        else
        {
            SynErr(n);
            while (!(set[syFol, kind] || set[repFol, kind] || set[0, kind]))
            {
                Get();
                kind = la.kind;
            }
            return StartOf(syFol);
        }
    }


    void programmes()
    {
        Unit = new CompilationUnit(); ProgramDeclaration p = null;
        program(ref p);
        Unit.Program = p;
        Expect(26);
    }

    void program(ref ProgramDeclaration p)
    {
        List<VariableDeclaration> vl = new List<VariableDeclaration>(); List<MethodDeclaration> ml = new List<MethodDeclaration>(); Statement s = null;
        Expect(3);
        p = new ProgramDeclaration { Location = new Location(t.line, t.col, t.charPos) };
        Expect(1);
        p.Name = t.val;
        Expect(19);
        if (la.kind == 4)
        {
            variable_declarations(ref vl);
        }
        else if (la.kind == 6 || la.kind == 7)
        {
        }
        else SynErr(40);
        p.Variables = vl;
        method_declarations(ref ml);
        p.Methods = ml;
        block_stmt(ref s);
        p.Block = s;
    }

    void variable_declarations(ref List<VariableDeclaration> v)
    {
        variable_declaration(ref v);
        if (la.kind == 6 || la.kind == 7 || la.kind == 29)
        {
        }
        else if (la.kind == 4)
        {
            variable_declarations(ref v);
        }
        else SynErr(41);
    }

    void method_declarations(ref List<MethodDeclaration> ml)
    {
        MethodDeclaration m = null;
        method_declaration(ref m);
        Expect(19);
        if (la.kind == 29)
        {
            ml = new List<MethodDeclaration> { m };
        }
        else if (la.kind == 6 || la.kind == 7)
        {
            method_declarations(ref ml);
            ml.Insert(0, m);
        }
        else SynErr(42);
    }

    void block_stmt(ref Statement stmt)
    {
        Expect(29);
        List<Statement> stmts = null; stmt = new BlockStatement { Location = new Location(t.line, t.col, t.charPos) };
        if (la.kind == 1 || la.kind == 29)
        {
            stmt_list(ref stmts);
            (stmt as BlockStatement).Statements = stmts;
        }
        else if (la.kind == 25)
        {
            (stmt as BlockStatement).Statements = new List<Statement>();
        }
        else SynErr(43);
        Expect(25);
    }

    void variable_declaration(ref List<VariableDeclaration> v)
    {
        Expect(4);
        var ptoken = t; List<VariableDeclaration> vtemp = new List<VariableDeclaration>(); List<string> il = null;
        ident_list(ref il);
        foreach (var id in il)
            vtemp.Add(new VariableDeclaration { Name = id, Location = new Location(ptoken.line, ptoken.col, ptoken.charPos) });
        if (v != null) // if it is passed by another var declaration
            vtemp.AddRange(v);
        v = vtemp;

        Expect(20);
        Expect(5);
        Expect(19);
    }

    void ident_list(ref List<string> il)
    {
        Expect(1);
        if (la.kind == 20)
        {
            il = new List<string> { t.val };
        }
        else if (la.kind == 18)
        {
            var i = t.val;
            Get();
            ident_list(ref il);
            il.Insert(0, i);
        }
        else SynErr(44);
    }

    void method_declaration(ref MethodDeclaration m)
    {
        method_header(ref m);
        Statement s = null; List<VariableDeclaration> vl = null;
        if (la.kind == 4)
        {
            variable_declarations(ref vl);
        }
        else if (la.kind == 29)
        {
            vl = new List<VariableDeclaration>();
        }
        else SynErr(45);
        m.LocalVariables = vl;
        block_stmt(ref s);
        m.Block = s;
    }

    void method_header(ref MethodDeclaration m)
    {
        if (la.kind == 6)
        {
            Get();
            m = new MethodDeclaration { Location = new Location(t.line, t.col, t.charPos), IsFunction = true };
            Expect(1);
            m.Name = t.val;
            if (la.kind == 22)
            {
                List<Parameter> pl = null;
                Get();
                parameter_list(ref pl);
                m.Parameters = pl;
                Expect(23);
            }
            else if (la.kind == 20)
            {
                m.Parameters = new List<Parameter>();
            }
            else SynErr(46);
            Expect(20);
            Expect(5);
            Expect(19);
        }
        else if (la.kind == 7)
        {
            Get();
            m = new MethodDeclaration { Location = new Location(t.line, t.col, t.charPos), IsFunction = false };
            Expect(1);
            m.Name = t.val;
            if (la.kind == 22)
            {
                List<Parameter> pl = null;
                Get();
                parameter_list(ref pl);
                m.Parameters = pl;
                Expect(23);
            }
            else if (la.kind == 19)
            {
                m.Parameters = new List<Parameter>();
            }
            else SynErr(47);
            Expect(19);
        }
        else SynErr(48);
    }

    void parameter_list(ref List<Parameter> pl)
    {
        Parameter p = null;
        parameter(ref p);
        if (la.kind == 23)
        {
            pl = new List<Parameter> { p };
        }
        else if (la.kind == 19)
        {
            Get();
            parameter_list(ref pl);
            pl.Insert(0, p);
        }
        else SynErr(49);
    }

    void parameter(ref Parameter p)
    {
        if (la.kind == 1)
        {
            Get();
            p = new Parameter { Name = t.val, Location = new Location(t.line, t.col, t.charPos) };
            Expect(20);
            Expect(5);
        }
        else if (la.kind == 4)
        {
            Get();
            Expect(1);
            p = new Parameter { Name = t.val, Location = new Location(t.line, t.col, t.charPos), IsVariable = true };
            Expect(20);
            Expect(5);
        }
        else SynErr(50);
    }

    void stmt_list(ref List<Statement> sl)
    {
        Statement s = null;
        stmt(ref s);
        if (la.kind == 25)
        {
            sl = new List<Statement> { s };
        }
        else if (la.kind == 19)
        {
            Get();
            stmt_list(ref sl);
            sl.Insert(0, s);
        }
        else SynErr(51);
    }

    void stmt(ref Statement stmt)
    {
        if (la.kind == 1)
        {
            Get();
            if (la.kind == 19 || la.kind == 25)
            {
                stmt = new MethodInvocationStatement { Name = t.val, Location = new Location(t.line, t.col, t.charPos) };
            }
            else if (la.kind == 31)
            {
                Expression e = null; stmt = new AssignmentStatement { Target = t.val, Location = new Location(t.line, t.col, t.charPos) };
                Get();
                expr(ref e);
                (stmt as AssignmentStatement).Expression = e;
            }
            else if (la.kind == 22)
            {
                List<Expression> args = null; stmt = new MethodInvocationStatement { Name = t.val, Location = new Location(t.line, t.col, t.charPos) };
                Get();
                expr_list(ref args);
                (stmt as MethodInvocationStatement).Arguments = args;
                Expect(23);
            }
            else SynErr(52);
        }
        else if (la.kind == 29)
        {
            block_stmt(ref stmt);
        }
        else SynErr(53);
    }

    void expr(ref Expression expr)
    {
        simple_expr(ref expr);
        if (StartOf(1))
        {
        }
        else if (StartOf(2))
        {
            Expression e = null; Operators op = Operators.Add;
            oprel(ref op);
            simple_expr(ref e);
            expr = new BinaryOperationExpression { Left = expr, Right = e, Operator = op, Location = expr.Location };
        }
        else SynErr(54);
    }

    void expr_list(ref List<Expression> e)
    {
        Expression ex = null;
        expr(ref ex);
        if (la.kind == 23)
        {
            e = new List<Expression> { ex };
        }
        else if (la.kind == 18)
        {
            Get();
            expr_list(ref e);
            e.Insert(0, ex);
        }
        else SynErr(55);
    }

    void simple_expr(ref Expression expr)
    {
        if (la.kind == 27 || la.kind == 28)
        {
            Expression e = null; Operators op = Operators.Add;
            signe(ref op);
            terme(ref e);
            expr = new UnaryOperationExpression { Operator = op, Location = e.Location, Expression = e };
        }
        else if (StartOf(3))
        {
            Expression e = null; Operators op = Operators.Add;
            terme(ref expr);
            if (StartOf(4))
            {
            }
            else if (la.kind == 27 || la.kind == 28)
            {
                signe(ref op);
                simple_expr(ref e);
                expr = new BinaryOperationExpression { Left = expr, Right = e, Operator = op, Location = expr.Location };
            }
            else if (la.kind == 16)
            {
                Get();
                simple_expr(ref e);
                expr = new BinaryOperationExpression { Left = expr, Right = e, Operator = Operators.Or, Location = expr.Location };
            }
            else SynErr(56);
        }
        else SynErr(57);
    }

    void oprel(ref Operators op)
    {
        switch (la.kind)
        {
            case 33:
                {
                    Get();
                    op = Operators.Equal;
                    break;
                }
            case 34:
                {
                    Get();
                    op = Operators.NotEqual;
                    break;
                }
            case 35:
                {
                    Get();
                    op = Operators.GT;
                    break;
                }
            case 36:
                {
                    Get();
                    op = Operators.LT;
                    break;
                }
            case 37:
                {
                    Get();
                    op = Operators.GTE;
                    break;
                }
            case 38:
                {
                    Get();
                    op = Operators.LTE;
                    break;
                }
            default: SynErr(58); break;
        }
    }

    void signe(ref Operators op)
    {
        if (la.kind == 27)
        {
            Get();
            op = Operators.Add;
        }
        else if (la.kind == 28)
        {
            Get();
            op = Operators.Sub;
        }
        else SynErr(59);
    }

    void terme(ref Expression expr)
    {
        Expression e = null;
        facteur(ref e);
        if (StartOf(5))
        {
            Expression right = null; Operators op = Operators.Add;
            opmul(ref op);
            terme(ref right);
            expr = new BinaryOperationExpression { Left = e, Right = right, Operator = op, Location = e.Location };
        }
        else if (StartOf(6))
        {
            expr = e;
        }
        else SynErr(60);
    }

    void facteur(ref Expression ex)
    {
        if (la.kind == 1)
        {
            Get();
            if (StartOf(7))
            {
                ex = new VariableExpression { Name = t.val, Location = new Location(t.line, t.col, t.charPos) };
            }
            else if (la.kind == 22)
            {
                ex = new MethodInvocationExpression { Name = t.val, Location = new Location(t.line, t.col, t.charPos) }; List<Expression> el = null;
                Get();
                expr_list(ref el);
                Expect(23);
            }
            else SynErr(61);
        }
        else if (la.kind == 2)
        {
            Get();
            ex = new IntegralExpression { Value = int.Parse(t.val), Location = new Location(t.line, t.col, t.charPos) };
        }
        else if (la.kind == 22)
        {
            Expression target = null;
            Get();
            ex = new ParenthesisExpression { Location = new Location(t.line, t.col, t.charPos) };
            expr(ref target);
            (ex as ParenthesisExpression).Target = target;
            Expect(23);
        }
        else if (la.kind == 17)
        {
            Expression e = null;
            Get();
            ex = new UnaryOperationExpression { Location = new Location(t.line, t.col, t.charPos), Operator = Operators.LogicalNot };
            facteur(ref e);
            (ex as UnaryOperationExpression).Expression = e;
        }
        else SynErr(62);
    }

    void opmul(ref Operators op)
    {
        if (la.kind == 32)
        {
            Get();
            op = Operators.Div;
        }
        else if (la.kind == 21)
        {
            Get();
            op = Operators.Mul;
        }
        else if (la.kind == 15)
        {
            Get();
            op = Operators.Div;
        }
        else if (la.kind == 14)
        {
            Get();
            op = Operators.Mod;
        }
        else if (la.kind == 13)
        {
            Get();
            op = Operators.And;
        }
        else SynErr(63);
    }



    public void Parse()
    {
        la = new Token();
        la.val = "";
        Get();
        programmes();
        Expect(0);

    }

    static readonly bool[,] set = {
        {_T,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _x,_x,_x,_T, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_x, _x},
        {_x,_T,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_T,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_T,_T, _x,_x,_x,_T, _x,_T,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_x, _x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _x,_x,_x,_x, _x,_T,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_x,_x, _x,_x,_x,_x, _x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _T,_x,_T,_T, _x,_x,_x,_T, _x,_T,_x,_T, _T,_x,_x,_x, _x,_T,_T,_T, _T,_T,_T,_x, _x},
        {_x,_x,_x,_x, _x,_x,_x,_x, _x,_x,_x,_x, _x,_T,_T,_T, _T,_x,_T,_T, _x,_T,_x,_T, _x,_T,_x,_T, _T,_x,_x,_x, _T,_T,_T,_T, _T,_T,_T,_x, _x}

    };
} // end Parser


public class Errors
{
    public int count = 0;                                    // number of errors detected
    public System.IO.TextWriter errorStream = Console.Out;   // error messages go to this stream
    public string errMsgFormat = "-- line {0} col {1}: {2}"; // 0=line, 1=column, 2=text

    public virtual void SynErr(int line, int col, int n)
    {
        string s;
        switch (n)
        {
            case 0: s = "EOF expected"; break;
            case 1: s = "ident expected"; break;
            case 2: s = "nb expected"; break;
            case 3: s = "prog expected"; break;
            case 4: s = "var expected"; break;
            case 5: s = "int expected"; break;
            case 6: s = "func expected"; break;
            case 7: s = "proc expected"; break;
            case 8: s = "if expected"; break;
            case 9: s = "then expected"; break;
            case 10: s = "else expected"; break;
            case 11: s = "while expected"; break;
            case 12: s = "do expected"; break;
            case 13: s = "and expected"; break;
            case 14: s = "mod expected"; break;
            case 15: s = "div expected"; break;
            case 16: s = "or expected"; break;
            case 17: s = "not expected"; break;
            case 18: s = "comma expected"; break;
            case 19: s = "semicolon expected"; break;
            case 20: s = "colon expected"; break;
            case 21: s = "star expected"; break;
            case 22: s = "lpar expected"; break;
            case 23: s = "rpar expected"; break;
            case 24: s = "lbrack expected"; break;
            case 25: s = "rbrace expected"; break;
            case 26: s = "dot expected"; break;
            case 27: s = "plus expected"; break;
            case 28: s = "minus expected"; break;
            case 29: s = "lbrace expected"; break;
            case 30: s = "rbrack expected"; break;
            case 31: s = "\"=\" expected"; break;
            case 32: s = "\"/\" expected"; break;
            case 33: s = "\"==\" expected"; break;
            case 34: s = "\"<>\" expected"; break;
            case 35: s = "\">\" expected"; break;
            case 36: s = "\"<\" expected"; break;
            case 37: s = "\">=\" expected"; break;
            case 38: s = "\"<=\" expected"; break;
            case 39: s = "??? expected"; break;
            case 40: s = "invalid program"; break;
            case 41: s = "invalid variable_declarations"; break;
            case 42: s = "invalid method_declarations"; break;
            case 43: s = "invalid block_stmt"; break;
            case 44: s = "invalid ident_list"; break;
            case 45: s = "invalid method_declaration"; break;
            case 46: s = "invalid method_header"; break;
            case 47: s = "invalid method_header"; break;
            case 48: s = "invalid method_header"; break;
            case 49: s = "invalid parameter_list"; break;
            case 50: s = "invalid parameter"; break;
            case 51: s = "invalid stmt_list"; break;
            case 52: s = "invalid stmt"; break;
            case 53: s = "invalid stmt"; break;
            case 54: s = "invalid expr"; break;
            case 55: s = "invalid expr_list"; break;
            case 56: s = "invalid simple_expr"; break;
            case 57: s = "invalid simple_expr"; break;
            case 58: s = "invalid oprel"; break;
            case 59: s = "invalid signe"; break;
            case 60: s = "invalid terme"; break;
            case 61: s = "invalid facteur"; break;
            case 62: s = "invalid facteur"; break;
            case 63: s = "invalid opmul"; break;

            default: s = "error " + n; break;
        }
        errorStream.WriteLine(errMsgFormat, line, col, s);
        count++;
    }

    public virtual void SemErr(int line, int col, string s)
    {
        errorStream.WriteLine(errMsgFormat, line, col, s);
        count++;
    }

    public virtual void SemErr(string s)
    {
        errorStream.WriteLine(s);
        count++;
    }

    public virtual void Warning(int line, int col, string s)
    {
        errorStream.WriteLine(errMsgFormat, line, col, s);
    }

    public virtual void Warning(string s)
    {
        errorStream.WriteLine(s);
    }
} // Errors


public class FatalError : Exception
{
    public FatalError(string m) : base(m) { }
}
