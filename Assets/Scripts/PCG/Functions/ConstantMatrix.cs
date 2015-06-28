using System;

namespace PCG
{
    public class ConstantMatrix : FunctionMatrix
    {
        public const string NAME = "ConstantMatrix";

        private int value;

        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public ConstantMatrix() 
            : this(256, 0)
        {
        }

        public ConstantMatrix(int size, int value) :
            base(
                "ConstantMatrix",
                //Input
                new ParameterDefinition[] { },
                //Output
                "matrix", size, 0, 255
            )
        {
            this.size = size;
            this.value = value;
        }

        protected override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            return value;
        }

        public override string ToString()
        {
            return NAME + " " + size + "x" + size + " -> " + value;
        }
    }
}