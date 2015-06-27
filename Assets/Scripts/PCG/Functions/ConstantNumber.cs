using System;

namespace PCG
{
    public class ConstantNumber : FunctionNumber
    {
        private int value;

        public int Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public ConstantNumber() : this(0)
        {
        }

        public ConstantNumber(int value) :
            base(
                "ConstantNumber",
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

        public override string ToString()
        {
            return "ConstantNumber " + value;
        }
    }
}