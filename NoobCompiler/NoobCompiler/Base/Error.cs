using System;
using System.Collections.Generic;
using System.Text;

namespace NoobCompiler.Base
{
    public enum ErrorType
    {
        Error,
        Warning
    };

    /// <summary>
    /// Simple Errors Store
    /// </summary>
    public class Error
    {
        public int ID;
        public string Description;
        public int Line;
        public ErrorType Type;
        public string File;
        public int Col;
        public Error() { }
        public Error(int errid, string desc, int line, int col, string file, ErrorType errtype)
        {
            File = file;
            Col = col;
            ID = errid;
            Description = desc;
            Line = line;
            Type = errtype;
        }
    }



}
