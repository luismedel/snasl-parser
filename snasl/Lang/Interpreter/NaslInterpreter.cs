using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using snasl.Lang.Parser;

namespace snasl.Lang.Interpreter
{
    class NaslInterpreter
    {
        void LoadModule (string path)
        {
        }

        readonly Tokenizer t;
        readonly Dictionary<string, Module> _modules = new Dictionary<string, Module> ();
    }
}
