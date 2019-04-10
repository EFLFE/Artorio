using System;

namespace Artorio.EFPng
{
    internal struct CropData
    {
        public int Top;
        public int Left;
        public int Right;
        public int Bottom;

        public CropData(int top, int left, int right, int bottom)
        {
            Top = top;
            Left = left;
            Right = right;
            Bottom = bottom;
        }

        public bool IsEmpty => Top == 0 && Left == 0 && Right == 0 && Bottom == 0;

        public override string ToString()
        {
            return $"[Top: {Top.ToString()}, Left: {Left.ToString()}, Right: {Right.ToString()}, Bottom: {Bottom.ToString()}]";
        }
    }
}
