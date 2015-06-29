#define USE_FIXED

using System;

namespace PCG
{
    public class PerlinNoiseMatrix : FunctionMatrix
    {
        public const string NAME = "PerlinNoiseMatrix";

        private int seed;
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
            : this(256, 0, 1, 64, 1)
        {
        }

        public PerlinNoiseMatrix(int size, int seed, int octNum, float frq, float amp) :
            base(
                NAME,
                //Input
                new ParameterDefinition[] { },
                //Output
                "x", size, Int32.MinValue, Int32.MaxValue
            )
        {
            this.seed = seed;
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
            //return min + (generator.FractalNoise2D(x, y, octNum, fint.CreateFromFloat(frq), fint.CreateFromFloat(amp)) * fint.CreateFromInt(max - min)).ToInt();
            int noise = generator.FractalNoise2D(x, y, octNum, ((int)frq) << PerlinNoiseFixed.SHIFT_AMOUNT, ((int)amp) << PerlinNoiseFixed.SHIFT_AMOUNT);

            //noise = (noise * ((max - min) << PerlinNoiseFixed.SHIFT_AMOUNT)) >> PerlinNoiseFixed.SHIFT_AMOUNT;

            //noise = (noise * (max - min)) >> PerlinNoiseFixed.SHIFT_AMOUNT;

            //We asume (max - min) = 256

            int noiseInt = noise >> (PerlinNoiseFixed.SHIFT_AMOUNT - 8);

            //FractalNoise2D returns values in the -0.75 / 0.75 range (for amplitude 1, 1 octave), so remap using that as a reference
            noiseInt = (((noiseInt + 192) * 2) / 3);
                
#else
            //return min + (int) ((generator.FractalNoise2D(x, y, octNum, frq, amp) * (max - min)));
            //We asume (max - min) = 256

            float noise = generator.FractalNoise2D(x, y, octNum, frq, amp);

            //FractalNoise2D returns values in the -0.75 / 0.75 range (for amplitude 1, 1 octave), so remap using that as a reference
            int noiseInt = (int) (((noise + 0.75f) / 1.5f) * 255f);

#endif

            if (noiseInt > 255)
                noiseInt = 255;
            else if (noiseInt < 0)
                noiseInt = 0;

            return noiseInt;
        }

        public override string ToString()
        {
            return NAME + " " + size + "x" + size + " -> [0..255]";
        }
    }
}
