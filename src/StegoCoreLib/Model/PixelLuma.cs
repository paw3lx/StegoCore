using System;
using System.Collections.Generic;
using System.Text;

namespace StegoCore.Model
{
    internal class PixelLuma
    {
        public PixelLuma(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
            Calculate();
        }

        public PixelLuma(float y, float cb, float cr)
        {
            _y = y;
            _cb = cb;
            _cr = cr;
            Decalculate();
        }

        public float Y => _y;

        public float Cb => _cb;

        public float Cr => _cr;

        public byte R => _r;
        
        public byte G => _g;

        public byte B => _b;

        private byte _r, _g, _b;
        private float _y, _cb, _cr;

        private void Calculate()
        {
            _y = (float)(0.299 * _r + 0.587 * _g + 0.114 * _b);
            _cb = (float)(-0.168736 * _r - 0.331264 * _g + 0.5 * _b + 128);
            _cr = (float)(0.5 * _r - 0.418688 * _g - 0.081312 * _b + 128);
        }

        private void Decalculate()
        {
            _r = (byte)(_y + 1.402 * (_cr - 128));
            _g = (byte)(_y - 0.34414 * (_cb - 128) - 0.71414 * (_cr - 128));
            _b = (byte)(_y + 1.772 * (_cb - 128));
        }


    }
}
