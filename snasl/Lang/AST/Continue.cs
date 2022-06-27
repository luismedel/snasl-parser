using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class Continue
        : InstrNode
    {

        public override void Visit (IVisitor visitor) => visitor.VisitContinue (this);
    }
}
