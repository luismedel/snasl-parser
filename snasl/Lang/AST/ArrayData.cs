using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class ArrayData
        : ExprNode
    {
        public string Key { get; set; }
        public Atom Inner { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitArrayData (this);
    }
}
