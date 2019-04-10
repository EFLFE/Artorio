using System;

namespace Artorio.EFPng
{
    internal static class Extra
    {
        public static int GetSquareForTiles(int tilesCount)
        {
            if (tilesCount < 2)
                return 1;

            // from 2 (2x2)
            int square = 2;

            // if pow(square) >= tiles
            while (square * square < tilesCount)
            {
                square++;
            }

            return square;
        }

        public static string RemoveDoubleQuotes(this string a)
        {
            return a.Replace('"', ' ').Trim();
        }

        public static int Max(params int[] args)
        {
            int max = int.MinValue;
            foreach (var item in args)
            {
                if (item > max)
                    max = item;
            }
            return max;
        }

        public static byte Max(params byte[] args)
        {
            byte max = byte.MinValue;
            foreach (var item in args)
            {
                if (item > max)
                    max = item;
            }
            return max;
        }

        public static byte Min(params byte[] args)
        {
            byte min = byte.MaxValue;
            foreach (var item in args)
            {
                if (item < min)
                    min = item;
            }
            return min;
        }
    }
}
