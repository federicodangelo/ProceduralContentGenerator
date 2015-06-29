//#define USE_FIXED

using System;

namespace PCG
{
    public class VoronoiNoiseMatrix : FunctionMatrix
    {
        public const string NAME = "VoronoiNoiseMatrix";

        public enum CombinatorFunction
        {
            D0 = 0,
            D1_D0 = 1
        }

        public enum DistanceFunction
        {
            Euclidian = 0,
            Manhattan = 1,
            Chebyshev = 2
        }
       
        private int seed;
        private int octNum;
        private float frq;
        private float amp;
        private CombinatorFunction combinatorFunction;
        private DistanceFunction distanceFunction;

        
#if USE_FIXED
        private VoronoiNoiseFixed generator;
#else
        private VoronoiNoise generator;
#endif
        
        public int Seed
        {
            get { return seed; }
            set { seed = value; }
        }
        
        public int OctNum
        {
            get { return octNum; }
            set { octNum = value; }
        }
        
        public float Frq
        {
            get { return frq; }
            set { frq = value; }
        }
        
        public float Amp
        {
            get { return amp; }
            set { amp = value; }
        }
        
        public CombinatorFunction CombinatorFunction1
        {
            get { return combinatorFunction; }
            set { combinatorFunction = value; }
        }

        public DistanceFunction DistanceFunction1
        {
            get { return distanceFunction; }
            set { distanceFunction = value; }
        }
        
        public VoronoiNoiseMatrix()
            : this(256, 0, 1, 128, 1, CombinatorFunction.D1_D0, DistanceFunction.Euclidian)
        {
        }

        public VoronoiNoiseMatrix(int size, int seed, int octNum, float frq, float amp, CombinatorFunction combinatorFunction, DistanceFunction distanceFunction) :
            base(
                NAME,
                //Input
                new ParameterDefinition[] { },
                //Output
                "x", size, 0, 255
            )
        {
            this.seed = seed;
            this.octNum = octNum;
            this.frq = frq;
            this.amp = amp;
            this.combinatorFunction = combinatorFunction;
            this.distanceFunction = distanceFunction;
        }

        protected override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            if (generator == null)
            {
#if USE_FIXED
                generator = new VoronoiNoiseFixed(seed);
#else
                generator = new VoronoiNoise(seed);
#endif

                switch(combinatorFunction)
                {
                    case CombinatorFunction.D0:
                        generator.SetCombinationTo_D0();
                        break;
                    case CombinatorFunction.D1_D0:
                        generator.SetCombinationTo_D1_D0();
                        break;
                }

                switch(distanceFunction)
                {
                    case DistanceFunction.Euclidian:
                        generator.SetDistanceToEuclidian();
                        break;

                    case DistanceFunction.Chebyshev:
                        generator.SetDistanceToChebyshev();
                        break;

                    case DistanceFunction.Manhattan:
                        generator.SetDistanceToManhattan();
                        break;
                }
            }

#if USE_FIXED
            fint noise = generator.FractalNoise2D(x, y, octNum, fint.CreateFromFloat(frq), fint.CreateFromFloat(amp));

            int intNoise = (noise * fint.CreateFromInt(256)).ToInt();

            return intNoise;
#else
            float noise = generator.FractalNoise2D(x, y, octNum, frq, amp);

            int intNoise = (int) (noise * 256.0f);

            return intNoise;
#endif
        }

        public override string ToString()
        {
            return NAME + " " + size + "x" + size;
        }
    }
}
