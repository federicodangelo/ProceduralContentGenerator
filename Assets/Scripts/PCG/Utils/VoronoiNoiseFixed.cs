using System;
using System.Collections;

//Original from: http://scrawkblog.com/2013/03/26/voronoi-noise-plugin-for-unity/

//Usage:
//- First set distance function (only one)
//  VoronoiNoise.SetDistanceToEuclidian();
//  VoronoiNoise.SetDistanceToManhattan();
//  VoronoiNoise.SetDistanceToChebyshev();
//- Then set combination function:
//  VoronoiNoise.SetCombinationTo_D0();
//  VoronoiNoise.SetCombinationTo_D1_D0();
//  VoronoiNoise.SetCombinationTo_D2_D0();
//- Then generate!
//  VoronoiNoise.FractalNoise2D(x,  y,  octaves, frq, amp, seed);

namespace PCG
{
    public class VoronoiNoiseFixed
    {
        //Function delegates, makes using functions pointers easier
        private delegate fint DISTANCE_FUNC2(fint p1x, fint p1y, fint p2x, fint p2y);
        //private delegate fint DISTANCE_FUNC3(fint p1x, fint p1y, fint p1z, fint p2x, fint p2y, fint p2z);
        private delegate fint COMBINE_FUNC(fint d0, fint d1);

        //Function pointer to active distance function and combination function
        private DISTANCE_FUNC2 DistanceFunc2 = EuclidianDistanceFunc2;
        //private DISTANCE_FUNC3 DistanceFunc3 = EuclidianDistanceFunc3;
        private COMBINE_FUNC CombineFunc = CombineFunc_D1_D0;

        //Set distance function
        public void SetDistanceToEuclidian() { DistanceFunc2 = EuclidianDistanceFunc2; /*DistanceFunc3 = EuclidianDistanceFunc3;*/ }
        public void SetDistanceToManhattan() { DistanceFunc2 = ManhattanDistanceFunc2; /*DistanceFunc3 = ManhattanDistanceFunc3;*/ }
        public void SetDistanceToChebyshev() { DistanceFunc2 = ChebyshevDistanceFunc2; /*DistanceFunc3 = ChebyshevDistanceFunc3;*/ }

        //Set combination function
        public void SetCombinationTo_D0() { CombineFunc = CombineFunc_D0; }
        public void SetCombinationTo_D1_D0() { CombineFunc = CombineFunc_D1_D0; }
        //public void SetCombinationTo_D2_D0() { CombineFunc = CombineFunc_D2_D0; }

        private int seed;

        public VoronoiNoiseFixed(int seed)
        {
            this.seed = seed;
        }

        //Sample 2D fractal noise
        public fint FractalNoise2D(int x, int y, int octNum, fint frq, fint amp)
        {
            fint gain = fint.one;
            fint sum = fint.zero;

            fint fx = fint.CreateFromInt(x);
            fint fy = fint.CreateFromInt(y);

            for (int i = 0; i < octNum; i++)
            {
                sum += Noise2D(fx * gain / frq, fy * gain / frq) * amp / gain;
                gain *= fint.two;
            }
            return sum;
        }

        //Sample single octave of 2D noise
        private fint Noise2D(fint inputX, fint inputY)
        {
            //Declare some values for later use
            uint lastRandom, numberFeaturePoints;
            fint randomDiffX, randomDiffY, featurePointX, featurePointY;
            int cubeX, cubeY;

            //Initialize values in distance array to large values
            fint m1 = fint.CreateFromInt(6666); //Closest point
            fint m2 = fint.CreateFromInt(6666); //2nd closest point

            //1. Determine which cube the evaluation point is in
            int evalCubeX = (int)inputX.ToInt();
            int evalCubeY = (int)inputY.ToInt();

            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    cubeX = evalCubeX + i;
                    cubeY = evalCubeY + j;

                    //2. Generate a reproducible random number generator for the cube
                    lastRandom = lcgRandom(hash((uint)(cubeX + seed), (uint)(cubeY)));

                    //3. Determine how many feature points are in the cube
                    numberFeaturePoints = probLookup(lastRandom);

                    //4. Randomly place the feature points in the cube
                    for (uint l = 0; l < numberFeaturePoints; ++l)
                    {
                        lastRandom = lcgRandom(lastRandom);
                        randomDiffX = fint.CreateRaw((int)(lastRandom & fint.decimalPartMask)); //(float)lastRandom / 0x100000000; --> random in (0..1) range

                        lastRandom = lcgRandom(lastRandom);
                        randomDiffY = fint.CreateRaw((int)(lastRandom & fint.decimalPartMask)); //(float)lastRandom / 0x100000000; --> random in (0..1) range

                        featurePointX = randomDiffX + fint.CreateFromInt(cubeX);
                        featurePointY = randomDiffY + fint.CreateFromInt(cubeY);

                        //5. Find the feature point closest to the evaluation point. 
                        //This is done by inserting the distances to the feature points into a sorted list

                        //insert(distanceArray, DistanceFunc2(inputX, inputY, featurePointX, featurePointY));
                        //insert(distanceArray, EuclidianDistanceFunc2(inputX, inputY, featurePointX, featurePointY));

                        fint val = DistanceFunc2(inputX, inputY, featurePointX, featurePointY);

                        if (val < m1)
                        {
                            m2 = m1;
                            m1 = val;
                        }
                        else if (val < m2)
                        {
                            m2 = val;
                        }
                    }

                    //6. Check the neighboring cubes to ensure their are no closer evaluation points.
                    // This is done by repeating steps 1 through 5 above for each neighboring cube
                }
            }

            //fint res = CombineFunc(distanceArray[0], distanceArray[1], distanceArray[2]);

            fint res = CombineFunc(m1, m2);

            if (res > fint.one)
                res = fint.one;
            else if (res < fint.zero)
                res = fint.zero;

            return res;
        }

        /*

        //Sample 3D fractal noise
        public fint FractalNoise3D(int x, int y, int z, int octNum, fint frq, fint amp)
        {
            fint gain = 1.0f;
            fint sum = 0.0f;

            for (int i = 0; i < octNum; i++)
            {
                sum += Noise3D(x * gain / frq, y * gain / frq, z * gain / frq) * amp / gain;
                gain *= 2.0f;
            }
            return sum;
        }
        
        //Sample single octave of 3D noise
        private fint Noise3D(fint inputX, fint inputY, fint inputZ)
        {
            //Declare some values for later use
            uint lastRandom, numberFeaturePoints;
            fint randomDiffX, randomDiffY, randomDiffZ, featurePointX, featurePointY, featurePointZ;
            int cubeX, cubeY, cubeZ;

            //Initialize values in distance array to large values
            for (int i = 0; i < distanceArray.Length; i++)
                distanceArray[i] = 6666;

            //1. Determine which cube the evaluation point is in
            int evalCubeX = (int) inputX;
            int evalCubeY = (int) inputY;
            int evalCubeZ = (int) inputZ;

            for (int i = -1; i < 2; ++i)
            {
                for (int j = -1; j < 2; ++j)
                {
                    for (int k = -1; k < 2; ++k)
                    {
                        cubeX = evalCubeX + i;
                        cubeY = evalCubeY + j;
                        cubeZ = evalCubeZ + k;

                        //2. Generate a reproducible random number generator for the cube
                        lastRandom = lcgRandom(hash((uint)(cubeX + seed), (uint)(cubeY), (uint)(cubeZ)));

                        //3. Determine how many feature points are in the cube
                        numberFeaturePoints = probLookup(lastRandom);

                        //4. Randomly place the feature points in the cube
                        for (uint l = 0; l < numberFeaturePoints; ++l)
                        {
                            lastRandom = lcgRandom(lastRandom);
                            randomDiffX = (fint)lastRandom / 0x100000000;

                            lastRandom = lcgRandom(lastRandom);
                            randomDiffY = (fint)lastRandom / 0x100000000;

                            lastRandom = lcgRandom(lastRandom);
                            randomDiffZ = (fint)lastRandom / 0x100000000;

                            featurePointX = randomDiffX + (fint)cubeX;
                            featurePointY = randomDiffY + (fint)cubeY;
                            featurePointZ = randomDiffZ + (fint)cubeZ;

                            //5. Find the feature point closest to the evaluation point. 
                            //This is done by inserting the distances to the feature points into a sorted list
                            insert(distanceArray, DistanceFunc3(inputX, inputY, inputZ, featurePointX, featurePointY, featurePointZ));
                        }
                        //6. Check the neighboring cubes to ensure their are no closer evaluation points.
                        // This is done by repeating steps 1 through 5 above for each neighboring cube
                    }
                }
            }

            fint res = CombineFunc(distanceArray[0], distanceArray[1], distanceArray[2]);

            if (res > 1.0f)
                res = 1.0f;
            else if (res < 0.0f)
                res = 0.0f;

            return res;
        }
         */

        //2D distance functions
        private static fint EuclidianDistanceFunc2(fint p1x, fint p1y, fint p2x, fint p2y)
        {
            return (p1x - p2x) * (p1x - p2x) + (p1y - p2y) * (p1y - p2y);
        }

        private static fint ManhattanDistanceFunc2(fint p1x, fint p1y, fint p2x, fint p2y)
        {
            return FMath.Abs(p1x - p2x) + FMath.Abs(p1y - p2y);
        }

        private static fint ChebyshevDistanceFunc2(fint p1x, fint p1y, fint p2x, fint p2y)
        {
            fint dx = p1x - p2x;
            fint dy = p1y - p2y;

            return FMath.Max(FMath.Abs(dx), FMath.Abs(dy));
        }

        //3D distance functions
        private static fint EuclidianDistanceFunc3(fint p1x, fint p1y, fint p1z, fint p2x, fint p2y, fint p2z)
        {
            return (p1x - p2x) * (p1x - p2x) + (p1y - p2y) * (p1y - p2y) + (p1z - p2z) * (p1z - p2z);
        }

        private static fint ManhattanDistanceFunc3(fint p1x, fint p1y, fint p1z, fint p2x, fint p2y, fint p2z)
        {
            return FMath.Abs(p1x - p2x) + FMath.Abs(p1y - p2y) + FMath.Abs(p1z - p2z);
        }

        private static fint ChebyshevDistanceFunc3(fint p1x, fint p1y, fint p1z, fint p2x, fint p2y, fint p2z)
        {
            fint dx = p1x - p2x;
            fint dy = p1y - p2y;
            fint dz = p1z - p2z;

            return FMath.Max(FMath.Max(FMath.Abs(dx), FMath.Abs(dy)), FMath.Abs(dz));
        }

        //Combination functions
        private static fint CombineFunc_D0(fint d0, fint d1) { return d0; }
        private static fint CombineFunc_D1_D0(fint d0, fint d1) { return d1 - d0; }
        //private static fint CombineFunc_D2_D0(fint d0, fint d1, fint d2) { return d2 - d0; }

        /// <summary>
        /// Given a uniformly distributed random number this function returns the number of feature points in a given cube.
        /// </summary>
        /// <param name="value">a uniformly distributed random number</param>
        /// <returns>The number of feature points in a cube.</returns>
        // Generated using mathmatica with "AccountingForm[N[Table[CDF[PoissonDistribution[4], i], {i, 1, 9}], 20]*2^32]"
        private static uint probLookup(uint value)
        {
            if (value < 393325350) return 1;
            if (value < 1022645910) return 2;
            if (value < 1861739990) return 3;
            if (value < 2700834071) return 4;
            if (value < 3372109335) return 5;
            if (value < 3819626178) return 6;
            if (value < 4075350088) return 7;
            if (value < 4203212043) return 8;
            return 9;
        }
        /// <summary>
        /// Inserts value into array using insertion sort. If the value is greater than the largest value in the array
        /// it will not be added to the array.
        /// </summary>
        /// <param name="arr">The array to insert the value into.</param>
        /// <param name="value">The value to insert into the array.</param>
        private static void insert(fint[] arr, fint value)
        {
            fint temp;
            for (int i = arr.Length - 1; i >= 0; i--)
            {
                if (value > arr[i]) break;
                temp = arr[i];
                arr[i] = value;
                if (i + 1 < arr.Length) arr[i + 1] = temp;
            }
        }
        /// <summary>
        /// LCG Random Number Generator.
        /// LCG: http://en.wikipedia.org/wiki/Linear_congruential_generator
        /// </summary>
        /// <param name="lastValue">The last value calculated by the lcg or a seed</param>
        /// <returns>A new random number</returns>
        private static uint lcgRandom(uint lastValue)
        {
            return (uint)((1103515245u * lastValue + 12345u) % 0x100000000u);
        }
        /// <summary>
        /// Constant used in FNV hash function.
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        private const uint OFFSET_BASIS = 2166136261;
        /// <summary>
        /// Constant used in FNV hash function
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        private const uint FNV_PRIME = 16777619;
        /// <summary>
        /// Hashes three integers into a single integer using FNV hash.
        /// FNV hash: http://isthe.com/chongo/tech/comp/fnv/#FNV-source
        /// </summary>
        /// <returns>hash value</returns>
        private static uint hash(uint i, uint j, uint k)
        {
            return (uint)((((((OFFSET_BASIS ^ (uint)i) * FNV_PRIME) ^ (uint)j) * FNV_PRIME) ^ (uint)k) * FNV_PRIME);
        }

        private static uint hash(uint i, uint j)
        {
            return (uint)((((OFFSET_BASIS ^ (uint)i) * FNV_PRIME) ^ (uint)j) * FNV_PRIME);
        }
    }
}
