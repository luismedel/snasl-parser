using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    class RepeatInstr
        : LoopInstr
    {
        public ExprNode Test { get; set; }

        public override void Visit (IVisitor visitor) => visitor.VisitRepeatInstr (this);
    }
}
