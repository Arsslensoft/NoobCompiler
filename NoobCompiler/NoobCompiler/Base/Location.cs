using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NoobCompiler.Base
{
    /// <summary>
    ///   Keeps track of the location in the program
    /// </summary>
    public class Location : IEquatable<Location>
    {
        public long Pos { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public string FileName { get { return Path.GetFileName(FullPath); } }
        public string FullPath { get; set; }
        public Location(string file, int row, int col, long pos)
        {
            FullPath = file;
            Row = row;
            Column = col;
            Pos = pos;
        }
        public Location(int row, int col, long pos)
        {
            FullPath = "default";
            Row = row;
            Column = col;
            Pos = pos;
        }
        public bool IsNull { get { return Column == -1 || Row == -1; } }

        public static Location Null = new Location(-1, -1, -1);
        #region IEquatable<Location> Members

        public bool Equals(Location other)
        {
            return this.Row == other.Row && this.Column == other.Column && FullPath == other.FullPath;
        }

        #endregion
        public override string ToString()
        {
            return string.Format("({0},{1},{2})", Row, Column, Pos);
        }
    }


}
