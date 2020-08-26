using System;
using System.Collections;
using System.Globalization;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class PropertyParser : PropertyTokenizer
    {
        private readonly PropertyInfo _propInfo;
        private const string _rELUNIT = "em";
        private static readonly Numeric _negOne = new Numeric((decimal)-1.0);
        private static readonly Hashtable _functionTable = new Hashtable();

        static PropertyParser()
        {
            _functionTable.Add("ceiling", new CeilingFunction());
            _functionTable.Add("floor", new FloorFunction());
            _functionTable.Add("round", new RoundFunction());
            _functionTable.Add("min", new MinFunction());
            _functionTable.Add("max", new MaxFunction());
            _functionTable.Add("abs", new AbsFunction());
            _functionTable.Add("rgb", new RgbColorFunction());
            _functionTable.Add("from-table-column", new FromTableColumnFunction());
            _functionTable.Add("inherited-property-value",
                              new InheritedPropFunction());
            _functionTable.Add("from-parent", new FromParentFunction());
            _functionTable.Add("from-nearest-specified-value",
                              new NearestSpecPropFunction());
            _functionTable.Add("proportional-column-width",
                              new PPColWidthFunction());
            _functionTable.Add("label-end", new LabelEndFunction());
            _functionTable.Add("body-start", new BodyStartFunction());
            _functionTable.Add("_fop-property-value", new FonetPropValFunction());
        }

        public static Property Parse(string expr, PropertyInfo propInfo)
        {
            return new PropertyParser(expr, propInfo).ParseProperty();
        }

        private PropertyParser(string propExpr, PropertyInfo pInfo)
            : base(propExpr)
        {
            this._propInfo = pInfo;
        }

        private Property ParseProperty()
        {
            Next();
            if (currentToken == TOK_EOF)
            {
                return new StringProperty("");
            }
            ListProperty propList = null;
            while (true)
            {
                Property prop = ParseAdditiveExpr();
                if (currentToken == TOK_EOF)
                {
                    if (propList != null)
                    {
                        propList.addProperty(prop);
                        return propList;
                    }
                    else
                    {
                        return prop;
                    }
                }
                else
                {
                    if (propList == null)
                    {
                        propList = new ListProperty(prop);
                    }
                    else
                    {
                        propList.addProperty(prop);
                    }
                }
            }
        }

        private Property ParseAdditiveExpr()
        {
            Property prop = ParseMultiplicativeExpr();
            bool cont = true;
            while (cont)
            {
                switch (currentToken)
                {
                    case TOK_PLUS:
                        Next();
                        prop = EvalAddition(prop.GetNumeric(),
                                            ParseMultiplicativeExpr().GetNumeric());
                        break;
                    case TOK_MINUS:
                        Next();
                        prop =
                            EvalSubtraction(prop.GetNumeric(),
                                            ParseMultiplicativeExpr().GetNumeric());
                        break;
                    default:
                        cont = false;
                        break;
                }
            }
            return prop;
        }

        private Property ParseMultiplicativeExpr()
        {
            Property prop = ParseUnaryExpr();
            bool cont = true;
            while (cont)
            {
                switch (currentToken)
                {
                    case TOK_DIV:
                        Next();
                        prop = EvalDivide(prop.GetNumeric(),
                                          ParseUnaryExpr().GetNumeric());
                        break;
                    case TOK_MOD:
                        Next();
                        prop = EvalModulo(prop.GetNumber(),
                                          ParseUnaryExpr().GetNumber());
                        break;
                    case TOK_MULTIPLY:
                        Next();
                        prop = EvalMultiply(prop.GetNumeric(),
                                            ParseUnaryExpr().GetNumeric());
                        break;
                    default:
                        cont = false;
                        break;
                }
            }
            return prop;
        }

        private Property ParseUnaryExpr()
        {
            if (currentToken == TOK_MINUS)
            {
                Next();
                return EvalNegate(ParseUnaryExpr().GetNumeric());
            }
            return ParsePrimaryExpr();
        }

        private void ExpectRpar()
        {
            if (currentToken != TOK_RPAR)
            {
                throw new PropertyException("expected )");
            }
            Next();
        }

        private Property ParsePrimaryExpr()
        {
            Property prop;
            switch (currentToken)
            {
                case TOK_LPAR:
                    Next();
                    prop = ParseAdditiveExpr();
                    ExpectRpar();
                    return prop;

                case TOK_LITERAL:
                    prop = new StringProperty(currentTokenValue);
                    break;

                case TOK_NCNAME:
                    prop = new NCnameProperty(currentTokenValue);
                    break;

                case TOK_FLOAT:
                    prop = new NumberProperty(ParseDouble(currentTokenValue));
                    break;

                case TOK_INTEGER:
                    prop = new NumberProperty(Int32.Parse(currentTokenValue));
                    break;

                case TOK_PERCENT:
                    double pcval = ParseDouble(
                        currentTokenValue.Substring(0, currentTokenValue.Length - 1)) / 100.0;
                    IPercentBase pcBase = this._propInfo.GetPercentBase();
                    if (pcBase != null)
                    {
                        if (pcBase.GetDimension() == 0)
                        {
                            prop = new NumberProperty(pcval * pcBase.GetBaseValue());
                        }
                        else if (pcBase.GetDimension() == 1)
                        {
                            prop = new LengthProperty(new PercentLength(pcval,
                                                                        pcBase));
                        }
                        else
                        {
                            throw new PropertyException("Illegal percent dimension value");
                        }
                    }
                    else
                    {
                        prop = new NumberProperty(pcval);
                    }
                    break;

                case TOK_NUMERIC:
                    int numLen = currentTokenValue.Length - currentUnitLength;
                    string unitPart = currentTokenValue.Substring(numLen);
                    double numPart = ParseDouble(currentTokenValue.Substring(0, numLen));
                    Length length;
                    if (unitPart.Equals(_rELUNIT))
                    {
                        length = new FixedLength(numPart, _propInfo.CurrentFontSize());
                    }
                    else
                    {
                        length = new FixedLength(numPart, unitPart);
                    }
                    
                    prop = new LengthProperty(length);
                    
                    break;

                case TOK_COLORSPEC:
                    prop = new ColorTypeProperty(new ColorType(currentTokenValue));
                    break;

                case TOK_FUNCTION_LPAR:
                    {
                        IFunction function =
                            (IFunction)_functionTable[currentTokenValue];
                        if (function == null)
                        {
                            throw new PropertyException("no such function: "
                                + currentTokenValue);
                        }
                        Next();
                        _propInfo.PushFunction(function);
                        prop = function.Eval(ParseArgs(function.NumArgs), _propInfo);
                        _propInfo.PopFunction();
                        return prop;
                    }
                default:
                    throw new PropertyException("syntax error");
            }
            Next();
            return prop;
        }

        private Property[] ParseArgs(int nbArgs)
        {
            Property[] args = new Property[nbArgs];
            Property prop;
            int i = 0;
            if (currentToken == TOK_RPAR)
            {
                Next();
            }
            else
            {
                while (true)
                {
                    prop = ParseAdditiveExpr();
                    if (i < nbArgs)
                    {
                        args[i++] = prop;
                    }
                    if (currentToken != TOK_COMMA)
                    {
                        break;
                    }
                    Next();
                }
                ExpectRpar();
            }
            if (nbArgs != i)
            {
                throw new PropertyException("Wrong number of args for function");
            }
            return args;
        }

        private Property EvalAddition(Numeric op1, Numeric op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non numeric operand in addition");
            }
            return new NumericProperty(op1.Add(op2));
        }

        private Property EvalSubtraction(Numeric op1, Numeric op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non numeric operand in subtraction");
            }
            return new NumericProperty(op1.Subtract(op2));
        }

        private Property EvalNegate(Numeric op)
        {
            if (op == null)
            {
                throw new PropertyException("Non numeric operand to unary minus");
            }
            return new NumericProperty(op.Multiply(_negOne));
        }

        private Property EvalMultiply(Numeric op1, Numeric op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non numeric operand in multiplication");
            }
            return new NumericProperty(op1.Multiply(op2));
        }

        private Property EvalDivide(Numeric op1, Numeric op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non numeric operand in division");
            }
            return new NumericProperty(op1.Divide(op2));
        }

        private Property EvalModulo(Number op1, Number op2)
        {
            if (op1 == null || op2 == null)
            {
                throw new PropertyException("Non number operand to modulo");
            }
            return new NumberProperty(op1.DoubleValue() % op2.DoubleValue());
        }

        private double ParseDouble(string s)
        {
            return Double.Parse(s, CultureInfo.InvariantCulture.NumberFormat);
        }
    }
}