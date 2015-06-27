using System;

namespace PCG
{
    public class RandomMatrix : FunctionMatrix
    {
        private int size;
        private int seed;
        private int min;
        private int max;

        public int Size
        {
            get { return size; }
            set { size = value; }
        }

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

        public RandomMatrix() : this(0, 0, 0, 255)
        {
        }
        
        public RandomMatrix(int size, int seed, int min, int max) :
            base(
                "RandomMatrix",
                //Input
                new ParameterDefinition[] { },
                //Output
                "x", Int32.MinValue, Int32.MaxValue
            )
        {
            this.seed = seed;
            this.size = size;
            this.min = min;
            this.max = max;
        }

        protected override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            return XXHashRandomGenerator.Range((uint) seed, min, max + 1, x, y);
        }
        
        protected override int OnEvaluateMatrixSize(Function[] inputValues)
        {
            return size;
        }

        public override string ToString()
        {
            return "RandomMatrix " + size + "x" + size + " -> [" + min + ".." + max + "]";
        }
    }
}