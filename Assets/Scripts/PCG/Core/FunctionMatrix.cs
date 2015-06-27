using System;

namespace PCG
{
    public abstract class FunctionMatrix : Function
    {
        protected int size;

        public int Size
        {
            get { return size; }
            set { size = value; }
        }
        
        public FunctionMatrix(string name, ParameterDefinition[] inputParameters, string outputName, int size, int minValue, int maxValue) :
            base(
                name,
                inputParameters,
                new ParameterDefinition(outputName, ParameterType.Matrix, minValue, maxValue)
                )
        {
            this.size = size;
        }

        protected override int OnEvaluateMatrixSize(Function[] inputValues)
        {
            return size;
        }

        protected sealed override int OnEvaluateNumber(Function[] inputValues)
        {
            throw new NotImplementedException();
        }
    }
}