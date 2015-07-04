using System;

namespace PCG
{
    public abstract class Function
    {
        private string name;
        private ParameterDefinition[] inputParameters;
        private ParameterDefinition outputParameter;
        private Function[] inputValues;

        public ParameterDefinition[] InputParameters
        {
            get
            {
                return inputParameters;
            }
        }

        public ParameterDefinition OutputParameter
        {
            get
            {
                return outputParameter;
            }
        }

        public String Name
        {
            get 
            { 
                return name; 
            }
        }

        public Function[] GetDefaultInputValues()
        {
            Function[] defaultInputValues = new Function[inputParameters.Length];

            for (int i = 0; i < inputParameters.Length; i++)
                defaultInputValues [i] = inputParameters [i].DefaultValue;

            return defaultInputValues;
        }

        public void SetInputValues(Function[] inputValues)
        {
            this.inputValues = inputValues;

            OnInputValuesSet();
        }

        public int GetInputParameterIndex(string name)
        {
            for (int i = 0; i < inputParameters.Length; i++)
                if (inputParameters [i].Name == name)
                    return i;
            
            return -1;
        }        

        public Function(string name, ParameterDefinition[] inputParameters, ParameterDefinition outputParameter)
        {
            this.name = name;
            this.inputParameters = inputParameters;
            this.outputParameter = outputParameter;
        }

        public Function GetInputValue(string name)
        {
            for (int i = 0; i < inputParameters.Length; i++)
                if (inputParameters [i].Name == name)
                    return inputValues [i];
            
            return null;
        }

        public int EvaluateNumber()
        {
            return OnEvaluateNumber(inputValues);
        }

        public int EvaluateMatrix(int x, int y)
        {
            return OnEvaluateMatrix(inputValues, x, y);
        }

        public Matrix EvaluateFullMatrix()
        {
            int size = EvaluateMatrixSize();

            Matrix matrix = new Matrix(size);

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    matrix.SetValue(x, y, (byte) EvaluateMatrix(x, y));

            return matrix;
        }

        public int EvaluateMatrixSize()
        {
            return OnEvaluateMatrixSize(inputValues);
        }

        public object Evaluate()
        {
            if (outputParameter.Type == ParameterType.Matrix)
                return EvaluateFullMatrix();
            else
                return EvaluateNumber();
        }

        protected abstract int OnEvaluateNumber(Function[] inputValues);

        protected abstract int OnEvaluateMatrix(Function[] inputValues, int x, int y);

        protected abstract int OnEvaluateMatrixSize(Function[] inputValues);

        protected virtual void OnInputValuesSet()
        {
            //Override to handle inputValues
        }
    }
}
