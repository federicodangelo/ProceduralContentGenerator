using System;
using System.Collections.Generic;

namespace PCG
{
    public class FunctionsParser
    {
        static private Dictionary<String, Type> functionMap;

        static FunctionsParser()
        {
            functionMap = new Dictionary<string, Type>();
            functionMap.Add(ConstantMatrix.NAME.ToLowerInvariant(), typeof(ConstantMatrix));
            functionMap.Add(ConstantNumber.NAME.ToLowerInvariant(), typeof(ConstantNumber));
            functionMap.Add(MidPointMatrix.NAME.ToLowerInvariant(), typeof(MidPointMatrix));
            functionMap.Add(PerlinNoiseMatrix.NAME.ToLowerInvariant(), typeof(PerlinNoiseMatrix));
            functionMap.Add(RandomMatrix.NAME.ToLowerInvariant(), typeof(RandomMatrix));
            functionMap.Add(VoronoiNoiseMatrix.NAME.ToLowerInvariant(), typeof(VoronoiNoiseMatrix));
        }

        static public Function Parse(string s)
        {
            Type functionType;

            if (!string.IsNullOrEmpty(s) && functionMap.TryGetValue(s.ToLowerInvariant(), out functionType))
            {
                return (Function) functionType.GetConstructor(new Type[0]).Invoke(null);
            }

            return null;
        }
    }
}
