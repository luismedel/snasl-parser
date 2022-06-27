using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    abstract class LoopInstr
        : InstrNode
    {
        public InstrList Body { get; set; }
    }
}
