using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class Module
        : IASTNode
    {
        public InstrNode[] Instructions { get; set; }

        public void Visit (IVisitor visitor) => visitor.VisitModule (this);
    }
}
