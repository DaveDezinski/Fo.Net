namespace Fonet.DataTypes
{
    using Fonet.Fo;

    internal class LengthRange : ICompoundDatatype
    {
        private Property _minimum;
        private Property _optimum;
        private Property _maximum;
        private const int _MINSET = 1;
        private const int _OPTSET = 2;
        private const int _MAXSET = 4;
        private int _bfSet = 0;
        private bool _bChecked = false;

        public virtual void SetComponent(string componentName, Property componentValue,
                                         bool isDefault)
        {
            if (componentName.Equals("minimum"))
            {
                SetMinimum(componentValue, isDefault);
            }
            else if (componentName.Equals("optimum"))
            {
                SetOptimum(componentValue, isDefault);
            }
            else if (componentName.Equals("maximum"))
            {
                SetMaximum(componentValue, isDefault);
            }
        }

        public virtual Property GetComponent(string componentName)
        {
            if (componentName.Equals("minimum"))
            {
                return GetMinimum();
            }
            else if (componentName.Equals("optimum"))
            {
                return GetOptimum();
            }
            else if (componentName.Equals("maximum"))
            {
                return GetMaximum();
            }
            else
            {
                return null;
            }
        }

        protected void SetMinimum(Property minimum, bool bIsDefault)
        {
            this._minimum = minimum;
            if (!bIsDefault)
            {
                _bfSet |= _MINSET;
            }
        }

        protected void SetMaximum(Property max, bool bIsDefault)
        {
            _maximum = max;
            if (!bIsDefault)
            {
                _bfSet |= _MAXSET;
            }
        }

        protected void SetOptimum(Property opt, bool bIsDefault)
        {
            _optimum = opt;
            if (!bIsDefault)
            {
                _bfSet |= _OPTSET;
            }
        }

        private void CheckConsistency()
        {
            if (_bChecked)
            {
                return;
            }
            _bChecked = true;
        }

        public Property GetMinimum()
        {
            CheckConsistency();
            return this._minimum;
        }

        public Property GetMaximum()
        {
            CheckConsistency();
            return this._maximum;
        }

        public Property GetOptimum()
        {
            CheckConsistency();
            return this._optimum;
        }
    }
}