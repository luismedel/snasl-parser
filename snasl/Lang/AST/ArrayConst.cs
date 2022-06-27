using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class ArrayConst
        : ExprNode
    {
        public ExprNode[] ExprItems { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitArrayConst (this);
    }
}
