using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class IdentifierAccess
        : ExprNode
    {
        public string Identifier { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitIdentifierAccess (this);
    }
}
