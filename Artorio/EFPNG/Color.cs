using System;

namespace Artorio.EFPng
{
    public struct Color
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public int RGB => R + G + B;

        public int RGBA => R + G + B + A;

        public Color(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
            A = byte.MaxValue;
        }

        public bool Equals(ref Color clr)
        {
            return R == clr.R
                && G == clr.G
                && clr.B == B;
        }

        public bool EqualsAlpha(ref Color clr)
        {
            return A == clr.A
                && R == clr.R
                && G == clr.G
                && clr.B == B;
        }

        public override string ToString()
        {
            return $"Color: [{R.ToString()}, {G.ToString()}, {B.ToString()}, {A.ToString()}]";
        }
    }
}
