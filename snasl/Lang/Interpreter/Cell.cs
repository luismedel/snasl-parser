using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace snasl.Lang.Interpreter
{
    [StructLayout(LayoutKind.Explicit)]
    struct Cell
    {
        public static readonly Cell NULL = new Cell { StringValue = null };

        [FieldOffset (0)]
        public int IntValue;

        [FieldOffset (0)]
        public string StringValue;

        [FieldOffset (0)]
        public Cell[] ArrayValue;
    }
}
