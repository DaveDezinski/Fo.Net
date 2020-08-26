using Fonet.Fo.Expr;

namespace Fonet.DataTypes
{
    internal class TableColLength : Length
    {
        private readonly double _tcolUnits;

        public TableColLength(double tcolUnits)
        {
            this._tcolUnits = tcolUnits;
        }

        public override double GetTableUnits()
        {
            return _tcolUnits;
        }

        public override void ResolveTableUnit(double tableUnit)
        {
            SetComputedValue((int)(_tcolUnits * tableUnit));
        }

        public override string ToString()
        {
            return (_tcolUnits.ToString() + " table-column-units");
        }

        public override Numeric AsNumeric()
        {
            return new Numeric(this);
        }
    }
}