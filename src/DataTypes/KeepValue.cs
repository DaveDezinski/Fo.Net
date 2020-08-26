namespace Fonet.DataTypes
{
    internal class KeepValue
    {
        public const string KEEP_WITH_ALWAYS = "KEEP_WITH_ALWAYS";
        public const string KEEP_WITH_AUTO = "KEEP_WITH_AUTO";
        public const string KEEP_WITH_VALUE = "KEEP_WITH_VALUE";

        private readonly string _type;
        private readonly int _value = 0;

        public KeepValue(string type, int value)
        {
            this._type = type;
            this._value = value;
        }

        public int GetValue()
        {
            return _value;
        }

        public string GetKeepType()
        {
            return _type;
        }

        public override string ToString()
        {
            return _type;
        }
    }
}