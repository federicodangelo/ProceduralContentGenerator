#define USE_FIXED

using System;

namespace PCG
{
    public class PerlinNoiseMatrix : FunctionMatrix
    {
        public const string NAME = "PerlinNoiseMatrix";

        private int seed;
        private int min;
        private int max;
        private int octNum;
        private float frq;
        private float amp;

#if USE_FIXED
        private PerlinNoiseFixed generator;
#else
        private PerlinNoise generator;
#endif

        public int Seed
        {
            get { return seed; }
            set { seed = value; }
        }

        public int Min
        {
            get { return this.min; }
            set { this.min = value; }
        }

        public int Max
        {
            get { return this.max; }
            set { this.max = value; }
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

        public PerlinNoiseMatrix()
            : this(256, 0, 0, 255, 1, 64, 1)
        {
        }

        public PerlinNoiseMatrix(int size, int seed, int min, int max, int octNum, float frq, float amp) :
            base(
                NAME,
                //Input
                new ParameterDefinition[] { },
                //Output
                "x", size, Int32.MinValue, Int32.MaxValue
            )
        {
            this.seed = seed;
            this.min = min;
            this.max = max;
            this.octNum = octNum;
            this.frq = frq;
            this.amp = amp;
        }

        protected override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            if (generator == null)
            {
#if USE_FIXED
                generator = new PerlinNoiseFixed(seed);
#else
                generator = new PerlinNoise(seed);
#endif

            }

#if USE_FIXED
            return min + (generator.FractalNoise2D(x, y, octNum, fint.CreateFromFloat(frq), fint.CreateFromFloat(amp)) * fint.CreateFromInt(max - min)).ToInt();
#else
            return min + (int) ((generator.FractalNoise2D(x, y, octNum, frq, amp) * (max - min)));
#endif
        }

        public override string ToString()
        {
            return NAME + " " + size + "x" + size + " -> [" + min + ".." + max + "]";
        }
    }
}
