using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class ForeachInstr
        : LoopInstr
    {
        public IdentifierAccess Identifier { get; set; }
        public ExprNode Array { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitForeachInstr (this);
    }
}
