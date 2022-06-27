using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.AST
{
    interface IASTNode
    {
        void Visit (IVisitor visitor);
    }
}
