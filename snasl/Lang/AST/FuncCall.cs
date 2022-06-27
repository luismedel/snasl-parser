using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class FuncCall
        : ExprNode
    {
        public ExprNode Left { get; set; }
        public ExprNode[] Args { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitFuncCall (this);
    }
}
