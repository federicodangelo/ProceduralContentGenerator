using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PCG;

public class FunctionView : MonoBehaviour 
{
    [Multiline]
    public string functionText;

    public Function GetFunction()
    {
        return FunctionsParser.Parse(functionText);
    }
}
