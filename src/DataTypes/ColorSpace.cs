namespace Fonet.DataTypes
{
    internal class ColorSpace
    {
        public const int DeviceUnknown = -1;
        public const int DeviceGray = 1;
        public const int DeviceRgb = 2;
        public const int DeviceCmyk = 3;

        protected int currentColorSpace;

        private bool _hasICCProfile;
        private byte[] _iccProfile;
        private int _numComponents;

        public ColorSpace(int theColorSpace)
        {
            currentColorSpace = theColorSpace;
            _hasICCProfile = false;
            _numComponents = this.CalculateNumComponents();
        }

        public void SetColorSpace(int theColorSpace)
        {
            currentColorSpace = theColorSpace;
            _numComponents = this.CalculateNumComponents();
        }

        public bool HasICCProfile()
        {
            return this._hasICCProfile;
        }

        public byte[] GetICCProfile()
        {
            if (_hasICCProfile)
            {
                return _iccProfile;
            }
            else
            {
                return new byte[0];
            }
        }

        public void SetICCProfile(byte[] iccProfile)
        {
            _iccProfile = iccProfile;
            _hasICCProfile = true;
        }

        public int GetColorSpace()
        {
            return currentColorSpace;
        }

        public int GetNumComponents()
        {
            return _numComponents;
        }

        public string GetColorSpacePDFString()
        {
            if (currentColorSpace == DeviceRgb)
            {
                return "DeviceRGB";
            }
            else if (currentColorSpace == DeviceCmyk)
            {
                return "DeviceCMYK";
            }
            else if (currentColorSpace == DeviceGray)
            {
                return "DeviceGray";
            }
            else
            {
                return "DeviceRGB";
            }
        }

        private int CalculateNumComponents()
        {
            if (currentColorSpace == DeviceGray)
            {
                return 1;
            }
            else if (currentColorSpace == DeviceRgb)
            {
                return 3;
            }
            else if (currentColorSpace == DeviceCmyk)
            {
                return 4;
            }
            else
            {
                return 0;
            }
        }
    }
}