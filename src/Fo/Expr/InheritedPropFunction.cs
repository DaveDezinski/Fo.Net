namespace Fonet.Fo.Expr
{
    internal class InheritedPropFunction : FunctionBase
    {
        public override int NumArgs
        {
            get
            {
                return 1;
            }
        }

        public override Property Eval(Property[] args, PropertyInfo propInfo)
        {
            string propName = args[0].GetString();
            if (propName == null)
            {
                throw new PropertyException("Incorrect parameter to inherited-property-value function");
            }
            return propInfo.GetPropertyList().GetInheritedProperty(propName);
        }

    }
}