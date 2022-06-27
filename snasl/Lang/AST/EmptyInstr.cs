using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class EmptyInstr
        : InstrNode
    {
        public override void Visit (IVisitor visitor)
        {
            visitor.VisitEmptyInstr (this);
        }
    }
}
