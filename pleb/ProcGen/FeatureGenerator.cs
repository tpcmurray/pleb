using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace pleb.ProcGen
{
    public static class FeatureGenerator
    {
        private static SHA1 sha = new SHA1Managed();
        private static Crc32 crc = new Crc32();
        private static PercRangeUshort uShortRange = new PercRangeUshort(0, 65535);

        public static ushort Hash(int x, int y)
        {
            return Hash(x.ToString() + y.ToString());
        }

        public static ushort Hash(string input)
        {
            //return BitConverter.ToUInt16(sha.ComputeHash(Encoding.UTF8.GetBytes(input)));
            return BitConverter.ToUInt16(crc.ComputeHash(Encoding.UTF8.GetBytes(input)));
        }

        public static bool IsHit(int x, int y, float perc)
        {
            x += 1;
            ushort hash = Hash(x.ToString() + y.ToString());
            return IsHit(hash, perc);
        }

        public static bool IsHit(ushort hash, float perc)
        {
            return uShortRange.IsLocationWithinPercent(hash, perc);
        }
    }
}
