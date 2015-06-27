using UnityEngine;
using System.Collections;
using PCG;

//[ExecuteInEditMode]
public class PCGView : MonoBehaviour 
{
    public string functionText;

    public Generator GetGenerator()
    {
        return new Generator(functionText);
    }
}
