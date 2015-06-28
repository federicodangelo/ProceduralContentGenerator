using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PCG;

public class FunctionView : MonoBehaviour 
{
    public string functionText;

    public string extraParameters;

    public Function GetFunction()
    {
        return FunctionsParser.Parse(functionText);
    }
}
