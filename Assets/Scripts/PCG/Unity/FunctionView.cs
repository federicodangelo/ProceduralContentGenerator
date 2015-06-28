using UnityEngine;
using System.Collections;
using PCG;

public class FunctionView : MonoBehaviour 
{
    public string functionText;

    public Function GetFunction()
    {
        return FunctionsParser.Parse(functionText);
    }
}
