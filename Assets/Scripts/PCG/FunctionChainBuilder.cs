using System;
using System.Collections.Generic;

namespace PCG
{
    public class FunctionChainBuilder
    {
        private List<Function> functions = new List<Function>();
        private List<Function[]> functionsInputValues = new List<Function[]>();

        public FunctionChainBuilder AddFunction(Function function)
        {
            Function[] inputValues = function.GetDefaultInputValues();

            functions.Add(function);
            functionsInputValues.Add(inputValues);

            function.SetInputValues(inputValues);

            return this;
        }

        public FunctionChainBuilder SetParameterValue(string name, Function value)
        {
            GetLastInputValues() [GetLastFunction().GetInputParameterIndex(name)] = value;

            return this;
        }

        private Function GetLastFunction()
        {
            return functions [functions.Count - 1];
        }

        private Function[] GetLastInputValues()
        {
            return functionsInputValues [functionsInputValues.Count - 1];
        }

        private void Pop()
        {
            functions.RemoveAt(functions.Count - 1);
            functionsInputValues.RemoveAt(functionsInputValues.Count - 1);
        }

        public FunctionChainBuilder TakeFunctionOutputAsParameter(string prevFunctionInputName)
        {
            Function function = GetLastFunction();

            Pop();

            SetParameterValue(prevFunctionInputName, function);

            return this;
        }

        public Function Finish()
        {
            Function function = GetLastFunction();

            Pop();

            return function;
        }
    }
}

