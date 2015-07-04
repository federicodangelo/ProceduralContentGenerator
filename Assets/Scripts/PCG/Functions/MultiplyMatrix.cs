using System;

namespace PCG
{
    public class MultiplyMatrix : FunctionMatrix
    {
        public const string NAME = "MultiplyMatrix";


        public MultiplyMatrix() :
            base(
                NAME,
                //Input
                new ParameterDefinition[] 
                {
                    new ParameterDefinition("m1", new ConstantMatrix(256, 255), 0, 255),
                    new ParameterDefinition("m2", new ConstantMatrix(256, 255), 0, 255)
                },
                //Output
                "x", 256, 0, 255
            )
        {
        }

        private Function m1;
        private Function m2;

        protected override void OnInputValuesSet()
        {
            m1 = GetInputValue("m1");
            m2 = GetInputValue("m2");
        }

        protected override int OnEvaluateMatrix(Function[] inputValues, int x, int y)
        {
            int v1 = m1.EvaluateMatrix(x, y);
            int v2 = m2.EvaluateMatrix(x, y);

            int res = (v1 * v2) / 256;

            return res;
        }

        public override string ToString()
        {
            return NAME + " " + size + "x" + size;
        }
    }
}