using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class ArrayAccess
        : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode IndexExpr { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitArrayAccess (this);
    }
}
