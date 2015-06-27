using System;

namespace PCG
{
    public abstract class FunctionNumber : Function
    {
        public FunctionNumber(string name, ParameterDefinition[] inputParameters, string outputName, int minValue, int maxValue) :
            base(
                name,
                inputParameters,
                new ParameterDefinition(outputName, ParameterType.Number, minValue, maxValue)
            )
        {
        }

        protected sealed override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            throw new NotImplementedException();
        }

        protected sealed override int OnEvaluateMatrixSize(Function[] inputValues)
        {
            throw new NotImplementedException();
        }
    }
}