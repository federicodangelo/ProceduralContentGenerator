using System;

namespace PCG
{
    public class ConstantMatrix : FunctionMatrix
    {
        private int size;
        private int value;

        public ConstantMatrix(int size, int value) :
            base(
                "ConstantMatrix " + size + "x" + size + " -> " + value,
                //Input
                new ParameterDefinition[] { },
                //Output
                "matrix", 0, 255
            )
        {
            this.size = size;
            this.value = value;
        }

        protected override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            return UnityEngine.Random.Range(0, 255);
            //return value;
        }

        protected override int OnEvaluateMatrixSize(Function[] inputValues)
        {
            return size;
        }
    }
}