using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NCAsm.Optimizer
{
    public interface IOptimizer
    {
        int Priority { get; set; }
        int Level { get; set; }
        bool Optimize(ref List<Instruction> src);
        bool Match(Instruction ins, int idx);
    }
}
