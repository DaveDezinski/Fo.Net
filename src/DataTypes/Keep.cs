using Fonet.Fo;

namespace Fonet.DataTypes
{
    internal class Keep : ICompoundDatatype
    {
        private Property _withinLine;

        private Property _withinColumn;

        private Property _withinPage;

        public Keep()
        {
        }

        public void SetComponent(string componentName, Property componentValue,
                                 bool isDefault)
        {
            if (componentName.Equals("within-line"))
            {
                setWithinLine(componentValue, isDefault);
            }
            else if (componentName.Equals("within-column"))
            {
                setWithinColumn(componentValue, isDefault);
            }
            else if (componentName.Equals("within-page"))
            {
                setWithinPage(componentValue, isDefault);
            }
        }

        public Property GetComponent(string componentName)
        {
            if (componentName.Equals("within-line"))
            {
                return getWithinLine();
            }
            else if (componentName.Equals("within-column"))
            {
                return getWithinColumn();
            }
            else if (componentName.Equals("within-page"))
            {
                return getWithinPage();
            }
            else
            {
                return null;
            }
        }

        public void setWithinLine(Property withinLine, bool bIsDefault)
        {
            this._withinLine = withinLine;
        }

        protected void setWithinColumn(Property withinColumn,
                                       bool bIsDefault)
        {
            this._withinColumn = withinColumn;
        }

        public void setWithinPage(Property withinPage, bool bIsDefault)
        {
            this._withinPage = withinPage;
        }

        public Property getWithinLine()
        {
            return this._withinLine;
        }

        public Property getWithinColumn()
        {
            return this._withinColumn;
        }

        public Property getWithinPage()
        {
            return this._withinPage;
        }

        public override string ToString()
        {
            return "Keep";
        }
    }
}