using System;

namespace PCG
{
    public class ConstantNumber : FunctionNumber
    {
        private int value;

        public ConstantNumber(int value) :
            base(
                "ConstantNumber(" + value + ")",
                //Input
                new ParameterDefinition[] { },
                //Output
                "x", Int32.MinValue, Int32.MaxValue
            )
        {
            this.value = value;
        }

        protected override int OnEvaluateNumber(Function[] inputValues)
        {
            return value;
        }
    }
}