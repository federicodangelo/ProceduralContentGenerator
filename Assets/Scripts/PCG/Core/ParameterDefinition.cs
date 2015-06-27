using System;

namespace PCG
{
    public class ParameterDefinition
    {
        private string name;
        private ParameterType type;
        private Function defaultValue;
        private int min;
        private int max;

        public string Name { get { return name; } }
        public ParameterType Type { get { return type; } }
        public Function DefaultValue { get { return defaultValue; } }
        public int Min { get { return min; } }
        public int Max { get { return max; } }

        public ParameterDefinition(string name, Function defaultValue, int min, int max)
        {
            this.name = name;
            this.type = defaultValue.OutputParameter.type;
            this.defaultValue = defaultValue;
            this.min = min;
            this.max = max;
        }

        public ParameterDefinition(string name, ParameterType type, int min, int max)
        {
            this.name = name;
            this.type = type;
            this.min = min;
            this.max = max;
        }

        public int EvaluateNumber(Function function)
        {
            int k = function.EvaluateNumber();

            if (k > max)
                k = max;
            else if (k < min)
                k = min;

            return k;
        }

        public int EvaluateMatrix(Function function, int x, int y)
        {
            int k = function.EvaluateMatrix(x, y);
            
            if (k > max)
                k = max;
            else if (k < min)
                k = min;
            
            return k;
        }
    }
}
