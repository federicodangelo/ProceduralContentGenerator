using UnityEngine;
using System.Collections;

namespace PCG
{
    public class Generator
    {
        private string functionText;

        public Generator(string functionText)
        {
            this.functionText = functionText;
        }

        public object Evaluate()
        {
            return GetFunction().Evaluate();
        }

        public Function GetFunction()
        {
            //return new FunctionChainBuilder().AddFunction(new ConstantNumber(12)).Finish();

            return new FunctionChainBuilder().AddFunction(new ConstantMatrix(256, 255)).Finish();
        }
    }
}