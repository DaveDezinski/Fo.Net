namespace Fonet.DataTypes
{
    using Fonet.Fo;
    using Fonet.Fo.Properties;

    internal class CondLength : ICompoundDatatype
    {
        private Property length;

        private Property conditionality;

        public void SetComponent(string componentName, Property componentValue, bool isDefault)
        {
            if (componentName.Equals("length"))
            {
                length = componentValue;
            }
            else if (componentName.Equals("conditionality"))
            {
                conditionality = componentValue;
            }
        }

        public Property GetComponent(string componentName)
        {
            if (componentName.Equals("length"))
            {
                return length;
            }
            else if (componentName.Equals("conditionality"))
            {
                return conditionality;
            }
            else
            {
                return null;
            }
        }

        public Property GetConditionality()
        {
            return conditionality;
        }

        public Property GetLength()
        {
            return length;
        }

        public bool IsDiscard()
        {
            return conditionality.GetEnum() == Constants.DISCARD;
        }

        public int MValue()
        {
            return length.GetLength().MValue();
        }
    }
}