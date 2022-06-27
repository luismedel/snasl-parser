using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class Return
        : InstrNode
    {
        public ExprNode Expr { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitReturn (this);
    }
}
