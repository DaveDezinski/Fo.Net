using Fonet.DataTypes;
using Fonet.Fo.Flow;

namespace Fonet.Fo.Expr
{
    internal class LabelEndFunction : FunctionBase
    {
        public override int NumArgs
        {
            get
            {
                return 0;
            }
        }

        public override Property Eval(Property[] args, PropertyInfo propInfo)
        {
            Length distance =
                propInfo.GetPropertyList().GetProperty("provisional-distance-between-starts").GetLength();
            Length separation =
                propInfo.GetPropertyList().GetNearestSpecifiedProperty("provisional-label-separation").GetLength();

            FObj item = propInfo.GetFO();
            while (item != null && !(item is ListItem))
            {
                item = item.getParent();
            }
            if (item == null)
            {
                throw new PropertyException("label-end() called from outside an fo:list-item");
            }
            Length startIndent = item.properties.GetProperty("start-indent").GetLength();

            LinearCombinationLength labelEnd = new LinearCombinationLength();

            LengthBase bse = new LengthBase(item, propInfo.GetPropertyList(),
                                            LengthBase.CONTAINING_BOX);
            PercentLength refWidth = new PercentLength(1.0, bse);

            labelEnd.AddTerm(1.0, refWidth);
            labelEnd.AddTerm(-1.0, distance);
            labelEnd.AddTerm(-1.0, startIndent);
            labelEnd.AddTerm(1.0, separation);

            labelEnd.ComputeValue();

            return new LengthProperty(labelEnd);
        }

    }
}