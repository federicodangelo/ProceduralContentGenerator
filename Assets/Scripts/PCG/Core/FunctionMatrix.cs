using System;

namespace PCG
{
    public abstract class FunctionMatrix : Function
    {
        public FunctionMatrix(string name, ParameterDefinition[] inputParameters, string outputName, int minValue, int maxValue) :
            base(
                name,
                inputParameters,
                new ParameterDefinition(outputName, ParameterType.Matrix, minValue, maxValue)
                )
        {
        }

        protected sealed override int OnEvaluateNumber(Function[] inputValues)
        {
            throw new NotImplementedException();
        }
    }
}