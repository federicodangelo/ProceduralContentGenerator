using UnityEngine;
using UnityEditor;
using PCG;

[CustomEditor(typeof(PCGView))]
public class PCGViewEditor : Editor
{
    static private Color32[] colors;
    static private Texture2D texture;

    static private string cachedFunctionText;
    static private object cachedFunctionResult;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PCGView view = (PCGView)target;

        if (cachedFunctionText != view.functionText)
        {
            cachedFunctionText = view.functionText;

            cachedFunctionResult = view.GetGenerator().Evaluate();

            if (cachedFunctionResult is int)
            {
                //Nothing to do
            } 
            else if (cachedFunctionResult is Matrix)
            {
                Matrix matrix = (Matrix)cachedFunctionResult;

                if (colors == null || colors.Length != matrix.size * matrix.size)
                    colors = new Color32[matrix.size * matrix.size];

                if (texture == null || texture.width != matrix.size || texture.height != matrix.size)
                {
                    if (texture)
                        Component.DestroyImmediate(texture);

                    texture = new Texture2D(matrix.size, matrix.size, TextureFormat.RGB24, false);
                }

                for (int i = 0; i < matrix.size * matrix.size; i++)
                    colors [i] = new Color32(matrix.values [i], matrix.values [i], matrix.values [i], 0);

                texture.SetPixels32(colors);
                texture.Apply();
            }
        } 

        if (cachedFunctionResult is int)
        {
            EditorGUILayout.LabelField("Result", cachedFunctionResult.ToString());
        } 
        else if (cachedFunctionResult is Matrix)
        {
            if (texture)
            {
                EditorGUILayout.LabelField(new GUIContent("Result"), new GUIContent(texture), GUILayout.MaxHeight(texture.height));

                //Rect rect = EditorGUILayout.GetControlRect(true, texture.height);
                //EditorGUI.DrawTextureTransparent(rect, texture, ScaleMode.ScaleToFit);
            }
        }
    }
}


