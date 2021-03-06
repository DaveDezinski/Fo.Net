using Fonet.Fo.Expr;

namespace Fonet.DataTypes
{

    internal class PercentLength : Length
    {

        private readonly double _factor;

        public PercentLength(double factor) : this(factor, null)
        {
        }

        public PercentLength(double factor, IPercentBase lbase)
        {
            this._factor = factor;
            this.BaseLength = lbase;
        }

        public IPercentBase BaseLength { get; set; } = null;

        public override void ComputeValue()
        {
            SetComputedValue((int)(_factor * (double)BaseLength.GetBaseLength()));
        }

        public double Value()
        {
            return _factor;
        }

        public override string ToString()
        {
            return (_factor * 100.0).ToString() + "%";
        }

        public override Numeric AsNumeric()
        {
            return new Numeric(this);
        }
    }
}