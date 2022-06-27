using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class Break
        : InstrNode
    {

        public override void Visit (IVisitor visitor) => visitor.VisitBreak (this);
    }
}
