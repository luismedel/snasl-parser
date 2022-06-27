using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class IfInstr
        : InstrNode
    {
        public ExprNode TestExpr { get; set; }
        public InstrList Body { get; set; }
        public InstrList ElseBody { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitIfInstr (this);
    }
}
