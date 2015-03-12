/*  Written in 2014 by Sebastiano Vigna (vigna@acm.org)

To the extent possible under law, the author has dedicated all copyright
and related and neighboring rights to this software to the public domain
worldwide. This software is distributed without any warranty.

See <http://creativecommons.org/publicdomain/zero/1.0/>. 

MurmurAvalanche taken from MurmurHash3: https://code.google.com/p/smhasher/
MurmurHash3 was written by Austin Appleby, and is placed in the public
domain. Austin Appleby disclaims copyright to this source code.
*/

using System;

namespace DungeonRising
{
    ///This is the fastest generator passing BigCrush without systematic
    ///errors, but due to the relatively short period it is acceptable only
    ///for applications with a very mild amount of parallelism; otherwise, use
    ///a xorshift1024* generator. */

    public class XSRandom : System.Random
    {
        private ulong[] s = new ulong[2];

        public XSRandom()
        {
            s[0] = MurmurAvalanche(System.DateTime.UtcNow.Ticks);
            s[0] = MurmurAvalanche(s[0]);
            s[1] = MurmurAvalanche(s[0]);
            s[1] = MurmurAvalanche(s[1]);
        }
        public XSRandom(long seed)
        {
            s[0] = MurmurAvalanche(seed);
            s[0] = MurmurAvalanche(s[0]);
            s[1] = MurmurAvalanche(s[0]);
            s[1] = MurmurAvalanche(s[1]);
        }
        private static ulong MurmurAvalanche(ulong k)
        {
            k ^= k >> 33;
            k *= 0xff51afd7ed558ccd;
            k ^= k >> 33;
            k *= 0xc4ceb9fe1a85ec53;
            k ^= k >> 33;

            return k;
        }
        private static ulong MurmurAvalanche(long k)
        {
            ulong k2 = (ulong)k;
            k2 ^= k2 >> 33;
            k2 *= 0xff51afd7ed558ccd;
            k2 ^= k2 >> 33;
            k2 *= 0xc4ceb9fe1a85ec53;
            k2 ^= k2 >> 33;

            return k2;
        }
        public ulong NextULong()
        {
            ulong s1 = s[0];
            ulong s0 = s[1];
            s[0] = s0;
            s1 ^= s1 << 23; // a
            return (s[1] = (s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26))) + s0; // b, c
        }
        public long NextLong()
        {
            ulong s1 = s[0];
            ulong s0 = s[1];
            s[0] = s0;
            s1 ^= s1 << 23; // a
            return (long)((s[1] = (s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26))) + s0); // b, c
        }
        public uint NextUInt()
        {
            ulong s1 = s[0];
            ulong s0 = s[1];
            s[0] = s0;
            s1 ^= s1 << 23; // a
            return (uint)((s[1] = (s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26))) + s0); // b, c
        }
        public int NextInt()
        {
            ulong s1 = s[0];
            ulong s0 = s[1];
            s[0] = s0;
            s1 ^= s1 << 23; // a
            return (int)((s[1] = (s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26))) + s0); // b, c
        }
        public override int Next(int upperBound)
        {
            ulong s1 = s[0];
            ulong s0 = s[1];
            s[0] = s0;
            s1 ^= s1 << 23; // a
            s0 = ((s[1] = (s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26))) + s0); // b, c
            return (int)((uint)(s0) % upperBound);
        }
        public override int Next(int lowerBound, int upperBound)
        {
            if (lowerBound >= upperBound)
                return upperBound;
            ulong s1 = s[0];
            ulong s0 = s[1];
            s[0] = s0;
            s1 ^= s1 << 23; // a
            s0 = ((s[1] = (s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26))) + s0); // b, c
            return (int)((uint)(s0) % (upperBound - lowerBound)) + lowerBound;
        }
        public override double NextDouble()
        {
            ulong s1 = s[0];
            ulong s0 = s[1];
            s[0] = s0;
            s1 ^= s1 << 23; // a
            return (double)((s[1] = (s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26))) + s0) / 0xffffffffffffffffU; // b, c
        }
        public double NextDouble(double outerBound)
        {
            ulong s1 = s[0];
            ulong s0 = s[1];
            s[0] = s0;
            s1 ^= s1 << 23; // a
            return (double)((s[1] = (s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26))) + s0) / 0xffffffffffffffffU * outerBound; // b, c
        }
        public double NextDouble(double innerBound, double outerBound)
        {
            ulong s1 = s[0];
            ulong s0 = s[1];
            s[0] = s0;
            s1 ^= s1 << 23; // a
            return (double)((s[1] = (s1 ^ s0 ^ (s1 >> 17) ^ (s0 >> 26))) + s0) / 0xffffffffffffffffU * (outerBound - innerBound) + innerBound; // b, c
        }
    }

    public static class XSSR
    {
        private static ulong[] ss = new ulong[2];
        public static XSRandom xsr = new XSRandom();
        public static void Seed(long seed)
        {
            xsr = new XSRandom(seed);
        }
        public static ulong NextULong()
        {
            return xsr.NextULong();
        }
        public static long NextLong()
        {
            return xsr.NextLong();
        }
        public static uint NextUInt()
        {
            return xsr.NextUInt();
        }
        public static int NextInt()
        {
            return xsr.NextInt();
        }
        public static int Next(int upperBound)
        {
            return xsr.Next(upperBound);
        }
        public static int Next(int lowerBound, int upperBound)
        {
            return xsr.Next(lowerBound, upperBound);
        }
        public static double NextDouble()
        {
            return xsr.NextDouble();
        }
        public static double NextDouble(double outerBound)
        {
            return xsr.NextDouble(outerBound);
        }
        public static double NextDouble(double innerBound, double outerBound)
        {
            return xsr.NextDouble(innerBound, outerBound);
        }
    }
}
