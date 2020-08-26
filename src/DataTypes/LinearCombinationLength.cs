namespace Fonet.DataTypes
{
    using System.Collections.Generic;

    internal class LinearCombinationLength : Length
    {
        protected List<double> factors;
        protected List<Length> lengths;

        public LinearCombinationLength()
        {
            factors = new List<double>();
            lengths = new List<Length>();
        }

        public void AddTerm(double factor, Length length)
        {
            factors.Add(factor);
            lengths.Add(length);
        }

        public override void ComputeValue()
        {
            int result = 0;
            int numFactors = factors.Count;
            for (int i = 0; i < numFactors; ++i)
            {
                double d = factors[i];
                Length l = lengths[i];
                result += (int)(d * l.MValue());
            }
            SetComputedValue(result);
        }
    }
}