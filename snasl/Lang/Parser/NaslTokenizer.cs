using System;
using System.Collections.Generic;
using System.Text;

namespace snasl.Lang.Parser
{
    class NaslTokenizer
    {
        public NaslTokenizer ()
        {
        }

        public IEnumerable<Token> Tokenize (string input)
        {
            var skip = new TokenType[] {
                TokenType.Comment,
                TokenType.Blank,
                TokenType.EOL,
            };

            return _tokernizer.Tokenize (input, skip);
        }

        static readonly Tokenizer _tokernizer = new Tokenizer (new List<IMatcher> {
            new RegexMatcher (@"#[^\n]*", TokenType.Comment),
            new RegexMatcher (@"\/\/[^\n]*", TokenType.Comment),
            new RegexMatcher (@"\/\*.+\*\/", TokenType.Comment),
            new RegexMatcher (@"\n", TokenType.EOL),
            new RegexMatcher (@"[\s\t\r]+", TokenType.Blank),

            new LiteralMatcher (".", TokenType.Dot),
            new LiteralMatcher (":", TokenType.Colon),
            new LiteralMatcher (";", TokenType.Semicolon),
            new LiteralMatcher (",", TokenType.Comma),
            new LiteralMatcher ("(", TokenType.LParen),
            new LiteralMatcher (")", TokenType.RParen),
            new LiteralMatcher ("{", TokenType.LCurly),
            new LiteralMatcher ("}", TokenType.RCurly),
            new LiteralMatcher ("[", TokenType.LSquare),
            new LiteralMatcher ("]", TokenType.RSquare),

            new LiteralMatcher ("=", TokenType.OpAssign),
            new LiteralMatcher ("+=", TokenType.OpAssignPlus),
            new LiteralMatcher ("-=", TokenType.OpAssignMinus),
            new LiteralMatcher ("*=", TokenType.OpAssignMul),
            new LiteralMatcher ("/=", TokenType.OpAssignDiv),
            new LiteralMatcher ("%=", TokenType.OpAssignMod),
            new LiteralMatcher (">>=", TokenType.OpAssignRShift),
            new LiteralMatcher ("<<=", TokenType.OpAssignLShift),
            new LiteralMatcher (">>>=", TokenType.OpAssignRShiftSign),

            new LiteralMatcher ("+", TokenType.OpPlus),
            new LiteralMatcher ("-", TokenType.OpMinus),
            new LiteralMatcher ("*", TokenType.OpMul),
            new LiteralMatcher ("/", TokenType.OpDiv),
            new LiteralMatcher ("%", TokenType.OpMod),
            new LiteralMatcher ("**", TokenType.OpPow),
            new LiteralMatcher ("++", TokenType.OpInc),
            new LiteralMatcher ("--", TokenType.OpDec),

            new LiteralMatcher ("and", TokenType.OpLogAnd),
            new LiteralMatcher ("!", TokenType.OpLogNot),
            new LiteralMatcher ("or", TokenType.OpLogOr),
            new LiteralMatcher ("&&", TokenType.OpLogAnd),
            new LiteralMatcher ("||", TokenType.OpLogOr),

            new LiteralMatcher ("~", TokenType.OpBitNeg),
            new LiteralMatcher ("&", TokenType.OpBitAnd),
            new LiteralMatcher ("^", TokenType.OpBitXor),
            new LiteralMatcher ("|", TokenType.OpBitOr),
            new LiteralMatcher (">>", TokenType.OpRShift),
            new LiteralMatcher ("<<", TokenType.OpLShift),
            new LiteralMatcher (">>>", TokenType.OpRShiftSign),

            new LiteralMatcher ("><", TokenType.OpInstr),
            new LiteralMatcher (">!<", TokenType.OpNotInstr),
            new LiteralMatcher ("=~", TokenType.OpRegexMatch),
            new LiteralMatcher ("!~", TokenType.OpNotRegexMatch),
            new LiteralMatcher ("<", TokenType.OpLT),
            new LiteralMatcher (">", TokenType.OpGT),
            new LiteralMatcher ("==", TokenType.OpEq),
            new LiteralMatcher ("!=", TokenType.OpNotEq),
            new LiteralMatcher ("<=", TokenType.OpLET),
            new LiteralMatcher (">=", TokenType.OpGET),

            new LiteralMatcher ("=>", TokenType.OpArrow),

            new RegexMatcher (@"[0-9]*", TokenType.Number),
            new RegexMatcher (@"0[xX]([0-9A-Fa-f]+)", TokenType.HexNumer, valueGroup:1),
            new RegexMatcher (@"0[bB]([01]+)", TokenType.BinNumber, valueGroup:1),
            new RegexMatcher (@"0([0-7]+)", TokenType.OctNumber, valueGroup:1),

            new RegexMatcher (@"[0-9]+\.[0-9]+\.[0-9]+\.[0-9]+", TokenType.IPAddress),

            new RegexMatcher (@"if\b", TokenType.KeywordIf),
            new RegexMatcher (@"else\b", TokenType.KeywordElse),
            new RegexMatcher (@"for\b", TokenType.KeywordFor),
            new RegexMatcher (@"while\b", TokenType.KeywordWhile),
            new RegexMatcher (@"repeat\b", TokenType.KeywordRepeat),
            new RegexMatcher (@"until\b", TokenType.KeywordUntil),
            new RegexMatcher (@"foreach\b", TokenType.KeywordForeach),
            new RegexMatcher (@"function\b", TokenType.KeywordFunction),
            new RegexMatcher (@"return\b", TokenType.KeywordReturn),
            new RegexMatcher (@"switch\b", TokenType.KeywordSwitch),
            new RegexMatcher (@"case\b", TokenType.KeywordCase),
            new RegexMatcher (@"break\b", TokenType.KeywordBreak),
            new RegexMatcher (@"local_var\b", TokenType.KeywordLocalVar),
            new RegexMatcher (@"global_var\b", TokenType.KeywordGlobalVar),
            new RegexMatcher (@"continue\b", TokenType.KeywordContinue),
            new RegexMatcher (@"include\b", TokenType.KeywordInclude),

            new RegexMatcher (@"[a-zA-Z_][a-zA-Z0-9_]*", TokenType.Identifier),

            new StringMatcher ('"', TokenType.String, escapeChars:false),
            new StringMatcher ('\'', TokenType.PureString, escapeChars:true),
        });

    }
}
