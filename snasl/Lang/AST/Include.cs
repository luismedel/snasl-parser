using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class Include
        : InstrNode
    {
        public Atom Path { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitInclude (this);
    }
}
