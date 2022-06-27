using System;
using System.Collections.Generic;
using System.Text;
using snasl.Lang.Parser;

namespace snasl.Lang.AST
{
    class Assignment
        : BinExpr
    {
        public override void Visit (IVisitor visitor) => visitor.VisitAssignment (this);
    }
}
