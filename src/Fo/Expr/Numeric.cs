using System;
using System.Collections.Generic;
using Fonet.DataTypes;

namespace Fonet.Fo.Expr
{
    internal class Numeric
    {
        public const int ABS_LENGTH = 1;
        public const int PC_LENGTH = 2;
        public const int TCOL_LENGTH = 4;

        private readonly int _valType;
        private readonly double _absValue;
        private readonly double _pcValue;
        private readonly IPercentBase _pcBase = null;
        private readonly double _tcolValue;
        private readonly int _dim;

        protected Numeric(int valType, double absValue, double pcValue,
                          double tcolValue, int dim, IPercentBase pcBase)
        {
            this._valType = valType;
            this._absValue = absValue;
            this._pcValue = pcValue;
            this._tcolValue = tcolValue;
            this._dim = dim;
            this._pcBase = pcBase;
        }

        public Numeric(decimal num) :
            this(ABS_LENGTH, (double)num, 0.0, 0.0, 0, null)
        {
        }

        public Numeric(FixedLength l) :
            this(ABS_LENGTH, (double)l.MValue(), 0.0, 0.0, 1, null)
        {
        }

        public Numeric(PercentLength pclen) :
            this(PC_LENGTH, 0.0, pclen.Value(), 0.0, 1, pclen.BaseLength)
        {
        }

        public Numeric(TableColLength tclen) :
            this(TCOL_LENGTH, 0.0, 0.0, tclen.GetTableUnits(), 1, null)
        {
        }

        public Length AsLength()
        {
            if (_dim == 1)
            {
                List<Length> len = new List<Length>(3);
                if ((_valType & ABS_LENGTH) != 0)
                {
                    len.Add(new FixedLength((int)_absValue));
                }
                if ((_valType & PC_LENGTH) != 0)
                {
                    len.Add(new PercentLength(_pcValue, _pcBase));
                }
                if ((_valType & TCOL_LENGTH) != 0)
                {
                    len.Add(new TableColLength(_tcolValue));
                }
                if (len.Count == 1)
                {
                    return len[0];
                }
                else
                {
                    return new MixedLength(len);
                }
            }
            else
            {
                return null;
            }
        }

        public Number AsNumber()
        {
            return new Number(AsDouble());
        }

        public Double AsDouble()
        {
            if (_dim == 0 && _valType == ABS_LENGTH)
            {
                return _absValue;
            }
            else
            {
                throw new InvalidOperationException("cannot make number if dimension != 0");
            }
        }

        private bool IsMixedType()
        {
            int ntype = 0;
            for (int t = _valType; t != 0; t >>= 1)
            {
                if ((t & 1) != 0)
                {
                    ++ntype;
                }
            }
            return ntype > 1;
        }

        public Numeric Subtract(Numeric op)
        {
            if (_dim == op._dim)
            {
                IPercentBase npcBase = ((_valType & PC_LENGTH) != 0) ? _pcBase
                    : op._pcBase;
                return new Numeric(_valType | op._valType, _absValue - op._absValue,
                                   _pcValue - op._pcValue,
                                   _tcolValue - op._tcolValue, _dim, npcBase);
            }
            else
            {
                throw new PropertyException("Can't add Numerics of different dimensions");
            }
        }

        public Numeric Add(Numeric op)
        {
            if (_dim == op._dim)
            {
                IPercentBase npcBase = ((_valType & PC_LENGTH) != 0) ? _pcBase
                    : op._pcBase;
                return new Numeric(_valType | op._valType, _absValue + op._absValue,
                                   _pcValue + op._pcValue,
                                   _tcolValue + op._tcolValue, _dim, npcBase);
            }
            else
            {
                throw new PropertyException("Can't add Numerics of different dimensions");
            }
        }

        public Numeric Multiply(Numeric op)
        {
            if (_dim == 0)
            {
                return new Numeric(op._valType, _absValue * op._absValue,
                                   _absValue * op._pcValue,
                                   _absValue * op._tcolValue, op._dim, op._pcBase);
            }
            else if (op._dim == 0)
            {
                double opval = op._absValue;
                return new Numeric(_valType, opval * _absValue, opval * _pcValue,
                                   opval * _tcolValue, _dim, _pcBase);
            }
            else if (_valType == op._valType && !IsMixedType())
            {
                IPercentBase npcBase = ((_valType & PC_LENGTH) != 0) ? _pcBase
                    : op._pcBase;
                return new Numeric(_valType, _absValue * op._absValue,
                                   _pcValue * op._pcValue,
                                   _tcolValue * op._tcolValue, _dim + op._dim,
                                   npcBase);
            }
            else
            {
                throw new PropertyException("Can't multiply mixed Numerics");
            }
        }

        public Numeric Divide(Numeric op)
        {
            if (_dim == 0)
            {
                return new Numeric(op._valType, _absValue / op._absValue,
                                   _absValue / op._pcValue,
                                   _absValue / op._tcolValue, -op._dim, op._pcBase);
            }
            else if (op._dim == 0)
            {
                double opval = op._absValue;
                return new Numeric(_valType, _absValue / opval, _pcValue / opval,
                                   _tcolValue / opval, _dim, _pcBase);
            }
            else if (_valType == op._valType && !IsMixedType())
            {
                IPercentBase npcBase = ((_valType & PC_LENGTH) != 0) ? _pcBase
                    : op._pcBase;
                return new Numeric(_valType,
                                   (_valType == ABS_LENGTH ? _absValue / op._absValue : 0.0),
                                   (_valType == PC_LENGTH ? _pcValue / op._pcValue : 0.0),
                                   (_valType == TCOL_LENGTH ? _tcolValue / op._tcolValue : 0.0),
                                   _dim - op._dim, npcBase);
            }
            else
            {
                throw new PropertyException("Can't divide mixed Numerics.");
            }
        }

        public Numeric Abs()
        {
            return new Numeric(_valType, Math.Abs(_absValue), Math.Abs(_pcValue),
                               Math.Abs(_tcolValue), _dim, _pcBase);
        }

        public Numeric Max(Numeric op)
        {
            double rslt = 0.0;
            if (_dim == op._dim && _valType == op._valType && !IsMixedType())
            {
                if (_valType == ABS_LENGTH)
                {
                    rslt = _absValue - op._absValue;
                }
                else if (_valType == PC_LENGTH)
                {
                    rslt = _pcValue - op._pcValue;
                }
                else if (_valType == TCOL_LENGTH)
                {
                    rslt = _tcolValue - op._tcolValue;
                }
                if (rslt > 0.0)
                {
                    return this;
                }
                else
                {
                    return op;
                }
            }
            throw new PropertyException("Arguments to max() must have same dimension and value type.");
        }

        public Numeric Min(Numeric op)
        {
            double rslt = 0.0;
            if (_dim == op._dim && _valType == op._valType && !IsMixedType())
            {
                if (_valType == ABS_LENGTH)
                {
                    rslt = _absValue - op._absValue;
                }
                else if (_valType == PC_LENGTH)
                {
                    rslt = _pcValue - op._pcValue;
                }
                else if (_valType == TCOL_LENGTH)
                {
                    rslt = _tcolValue - op._tcolValue;
                }
                if (rslt > 0.0)
                {
                    return op;
                }
                else
                {
                    return this;
                }
            }
            throw new PropertyException("Arguments to min() must have same dimension and value type.");
        }
    }
}