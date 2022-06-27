using System;
using System.Collections.Generic;
using System.Text;
using snasl.Lang.Parser;

namespace snasl.Lang.AST
{
    class BinExpr
        : ExprNode
    {
        public ExprNode Left { get; set; }
        public Token Op { get; set; }
        public ExprNode Right { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitBinExpr (this);
    }
}
