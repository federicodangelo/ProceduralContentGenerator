using System;

namespace PCG
{
    public class MidPointMatrix : FunctionMatrix
    {
        public const string NAME = "MidPointMatrix";

        private int seed;
        private int roughness;

        private Matrix matrix;
        
        public int Seed
        {
            get { return seed; }
            set { seed = value; }
        }

        public int Roughness
        {
            get { return roughness; }
            set { roughness = value; }
        }

        public MidPointMatrix() 
            : this(256, 0, 1)
        {
        }
        
        public MidPointMatrix(int size, int seed, int roughness) :
            base(
                NAME,
                //Input
                new ParameterDefinition[] { },
                //Output
                "x", size, 0, 255
            )
        {
            this.seed = seed;
            this.roughness = roughness;
        }
        
        protected override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            if (matrix == null)
            {
                matrix = new Matrix(size);
                PlasmaGenerator.DrawPlasma(matrix, seed, roughness);
            }

            return matrix.GetValue(x, y);
        }

        public override string ToString()
        {
            return NAME + " " + size + "x" + size;
        }
    }
}