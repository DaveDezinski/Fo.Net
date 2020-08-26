using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class Space : LengthRange
    {
        public override void SetComponent(string componentName, Property componentValue, bool isDefault)
        {
            if (componentName.Equals("precedence"))
            {
                Precedence = componentValue;
            }
            else if (componentName.Equals("conditionality"))
            {
                Conditionality = componentValue;
            }
            else
            {
                base.SetComponent(componentName, componentValue, isDefault);
            }
        }

        public override Property GetComponent(string componentName)
        {
            if (componentName.Equals("precedence"))
            {
                return Precedence;
            }
            else if (componentName.Equals("conditionality"))
            {
                return Conditionality;
            }
            else
            {
                return base.GetComponent(componentName);
            }
        }

        public Property Conditionality { get; set; }

        public Property Precedence { get; set; }

    }
}