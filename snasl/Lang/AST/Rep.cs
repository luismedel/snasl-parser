using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class Rep
        : InstrNode
    {
        public ExprNode Expr { get; set; }
        public ExprNode Times { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitRep (this);
    }
}
