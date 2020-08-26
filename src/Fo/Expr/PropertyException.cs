using System;

namespace Fonet.Fo.Expr
{
     public class PropertyException : Exception
    {
        public PropertyException(string detail) : base(detail)
        {
        }
    }
}