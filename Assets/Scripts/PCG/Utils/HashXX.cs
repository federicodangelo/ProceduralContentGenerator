/*
C# implementation of xxHash optimized for producing random numbers from one or more input integers.
Copyright (C) 2015, Rune Skovbo Johansen. (https://bitbucket.org/runevision/random-numbers-testing/)

Based on C# implementation Copyright (C) 2014, Seok-Ju, Yun. (https://github.com/noricube/xxHashSharp)

Original C Implementation Copyright (C) 2012-2014, Yann Collet. (https://code.google.com/p/xxhash/)
BSD 2-Clause License (http://www.opensource.org/licenses/bsd-license.php)

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are
met:

    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above
      copyright notice, this list of conditions and the following
      disclaimer in the documentation and/or other materials provided
      with the distribution.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

//Reference / original source code:
//http://www.gamasutra.com/blogs/RuneSkovboJohansen/20150105/233505/A_Primer_on_Repeatable_Random_Numbers.php
//https://bitbucket.org/runevision/random-numbers-testing/src/dfebbfd28597ad33e8e7c7c7cedc4ae7aa49951e/Assets/Implementations/HashFunctions/?at=default

namespace PCG
{
    public class HashXX
    {
        const uint PRIME32_1 = 2654435761U;
        const uint PRIME32_2 = 2246822519U;
        const uint PRIME32_3 = 3266489917U;
        const uint PRIME32_4 = 668265263U;
        const uint PRIME32_5 = 374761393U;
        
        static public int Range (uint seed, int min, int max, int x, int y) 
        {
            return min + (int)(GetRandom (seed, x, y) % (max - min));
        }

        static public int Range (uint seed, int min, int max, int x) 
        {
            return min + (int)(GetRandom (seed, x) % (max - min));
        }

        static public uint GetRandom(uint seed, int buf1, int buf2)
        {
            //Simplified from GetHash() for only 2 parameters
            uint h32 = (uint)seed + PRIME32_5;
            h32 += 8U; //(uint)len * 4;
            
            h32 += (uint)buf1 * PRIME32_3;
            h32 = RotateLeft(h32, 17) * PRIME32_4;

            h32 += (uint)buf2 * PRIME32_3;
            h32 = RotateLeft(h32, 17) * PRIME32_4;

            h32 ^= h32 >> 15;
            h32 *= PRIME32_2;
            h32 ^= h32 >> 13;
            h32 *= PRIME32_3;
            h32 ^= h32 >> 16;
            
            return h32;
        }

        static public uint GetRandom(uint seed, int buf)
        {
            uint h32 = (uint)seed + PRIME32_5;
            h32 += 4U;
            h32 += (uint)buf * PRIME32_3;
            h32 = RotateLeft(h32, 17) * PRIME32_4;
            h32 ^= h32 >> 15;
            h32 *= PRIME32_2;
            h32 ^= h32 >> 13;
            h32 *= PRIME32_3;
            h32 ^= h32 >> 16;
            return h32;
        }
        
        private static uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }
    }
}