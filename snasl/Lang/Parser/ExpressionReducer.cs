using System;
using System.Collections.Generic;
using System.Text;
using snasl.Lang.AST;

namespace snasl.Lang.Parser
{
    static class ExpressionReducer
    {
        public static ExprNode Reduce (ExprNode expr)
        {
            return expr;

            // No optimizations right now
            /*
            if (expr == null || expr.IsReduced)
                return expr;

            expr.IsReduced = true;

            BinExpr bexpr = expr as BinExpr;
            if (bexpr != null)
                return ReduceBinaryExpression (bexpr);

            UnaryExpr uexpr = expr as UnaryExpr;
            if (uexpr != null)
                ReduceUnaryExpression (uexpr);

            expr.IsReduced = true;
            return expr;
            */
        }

        static ExprNode ReduceUnaryExpression (UnaryExpr uexpr)
        {
            uexpr.Expr = Reduce (uexpr.Expr);
            if (uexpr.Op.Value == "+")
                return uexpr.Expr;
/*
            if (uexpr.Op.Value == "-" && uexpr.Expr.Type == ItemType.Literal)
            {
                if (uexpr.InnerExpr is IntLiteralItem)
                    ((IntLiteralItem) uexpr.InnerExpr).Value *= -1;
                else if (uexpr.InnerExpr is FloatLiteralItem)
                    ((FloatLiteralItem) uexpr.InnerExpr).Value *= -1;
                else
                    throw new ParserException (null, "This error shouldn't happen");

                return uexpr.InnerExpr;
            }
*/
            return uexpr;
        }

        static ExprNode ReduceBinaryExpression (BinExpr bexpr)
        {
            bexpr.Left = Reduce (bexpr.Left);
            bexpr.Right = Reduce (bexpr.Right);
/*
            if (bexpr.Left is Atom && bexpr.Right is Atom)
            {
                Atom aleft = (Atom) bexpr.Left;
                Atom aright = (Atom) bexpr.Right;

                switch (bexpr.Op.Value)
                {
                    case "+":
                        if (aleft.Type == Atom.AtomType.Number && aright.Type == Atom.AtomType.Number)
                            return new Atom {  new IntLiteralItem (((IntLiteralItem) bexpr.Left).Value + ((IntLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is FloatLiteralItem && bexpr.Right is IntLiteralItem)
                            return new FloatLiteralItem (((FloatLiteralItem) bexpr.Left).Value + ((IntLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is IntLiteralItem && bexpr.Right is FloatLiteralItem)
                            return new FloatLiteralItem (((IntLiteralItem) bexpr.Left).Value + ((FloatLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is FloatLiteralItem && bexpr.Right is FloatLiteralItem)
                            return new FloatLiteralItem (((FloatLiteralItem) bexpr.Left).Value + ((FloatLiteralItem) bexpr.Right).Value);
                        else
                            throw new ParserException (null, "This error shouldn't happen");

                    case "-":
                        if (bexpr.Left is IntLiteralItem && bexpr.Right is IntLiteralItem)
                            return new IntLiteralItem (((IntLiteralItem) bexpr.Left).Value - ((IntLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is FloatLiteralItem && bexpr.Right is IntLiteralItem)
                            return new FloatLiteralItem (((FloatLiteralItem) bexpr.Left).Value - ((IntLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is IntLiteralItem && bexpr.Right is FloatLiteralItem)
                            return new FloatLiteralItem (((IntLiteralItem) bexpr.Left).Value - ((FloatLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is FloatLiteralItem && bexpr.Right is FloatLiteralItem)
                            return new FloatLiteralItem (((FloatLiteralItem) bexpr.Left).Value - ((FloatLiteralItem) bexpr.Right).Value);
                        else
                            throw new ParserException (null, "This error shouldn't happen");

                    case "*":
                        if (bexpr.Left is IntLiteralItem && bexpr.Right is IntLiteralItem)
                            return new IntLiteralItem (((IntLiteralItem) bexpr.Left).Value * ((IntLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is FloatLiteralItem && bexpr.Right is IntLiteralItem)
                            return new FloatLiteralItem (((FloatLiteralItem) bexpr.Left).Value * ((IntLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is IntLiteralItem && bexpr.Right is FloatLiteralItem)
                            return new FloatLiteralItem (((IntLiteralItem) bexpr.Left).Value * ((FloatLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is FloatLiteralItem && bexpr.Right is FloatLiteralItem)
                            return new FloatLiteralItem (((FloatLiteralItem) bexpr.Left).Value * ((FloatLiteralItem) bexpr.Right).Value);
                        else
                            throw new ParserException (null, "This error shouldn't happen");

                    case "/":
                        if (bexpr.Left is IntLiteralItem && bexpr.Right is IntLiteralItem)
                            return new IntLiteralItem (((IntLiteralItem) bexpr.Left).Value * ((IntLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is FloatLiteralItem && bexpr.Right is IntLiteralItem)
                            return new FloatLiteralItem (((FloatLiteralItem) bexpr.Left).Value * ((IntLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is IntLiteralItem && bexpr.Right is FloatLiteralItem)
                            return new FloatLiteralItem (((IntLiteralItem) bexpr.Left).Value * ((FloatLiteralItem) bexpr.Right).Value);
                        else if (bexpr.Left is FloatLiteralItem && bexpr.Right is FloatLiteralItem)
                            return new FloatLiteralItem (((FloatLiteralItem) bexpr.Left).Value * ((FloatLiteralItem) bexpr.Right).Value);
                        else
                            throw new ParserException (null, "This error shouldn't happen");

                    default:
                        throw new ParserException (null, "This error shouldn't happen");
                }
            }
*/

            return bexpr;
        }
    }
}
