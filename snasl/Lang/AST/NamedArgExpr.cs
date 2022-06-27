using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class NamedArgExpr
        : ExprNode
    {
        public string Identifier { get;  set; }
        public ExprNode Expr { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitNamedArgExpr (this);
    }
}
