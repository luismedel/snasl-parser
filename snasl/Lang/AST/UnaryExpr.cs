using System;
using System.Collections.Generic;
using System.Text;
using snasl.Lang.Parser;

namespace snasl.Lang.AST
{
    class UnaryExpr
        : ExprNode
    {
        public bool IsPostOperator { get; set; } = false;
        public Token Op { get; set; }
        public ExprNode Expr { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitUnaryExpr (this);
    }
}
