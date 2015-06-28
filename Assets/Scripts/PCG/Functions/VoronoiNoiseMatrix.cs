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
        private int min;
        private int max;
        private int octNum;
        private float frq;
        private float amp;
        private CombinatorFunction combinatorFunction;
        private DistanceFunction distanceFunction;

        private VoronoiNoise generator;
        
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
            : this(256, 0, 0, 255, 1, 128, 1, CombinatorFunction.D1_D0, DistanceFunction.Euclidian)
        {
        }

        public VoronoiNoiseMatrix(int size, int seed, int min, int max, int octNum, float frq, float amp, CombinatorFunction combinatorFunction, DistanceFunction distanceFunction) :
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
            this.combinatorFunction = combinatorFunction;
            this.distanceFunction = distanceFunction;
        }

        protected override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            if (generator == null)
            {
                generator = new VoronoiNoise(seed);

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

            return min + (int) (generator.FractalNoise2D(x, y, octNum, frq, amp) * (max - min));
        }

        public override string ToString()
        {
            return NAME + " " + size + "x" + size + " -> [" + min + ".." + max + "]";
        }
    }
}
