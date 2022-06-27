using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    abstract class ExprNode
        : InstrNode
    {
        public bool IsReduced { get; set; }
    }
}
