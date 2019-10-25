using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SignsAndLanternsHanger))]
public class SignsAndLanternsHangerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        EditorGUILayout.LabelField("Use buttons below to place random signs and lanterns on buildings.");
        
        SignsAndLanternsHanger hanger = (SignsAndLanternsHanger)target;
        if (GUILayout.Button("PlaceSigns"))
        {
            hanger.HangSignsInPlaceholders();
        }

        if (GUILayout.Button("PlaceLanterns"))
        {
            hanger.HangLanternsInPlaceholders();
        }

        if (GUILayout.Button("ClearSignsAndLanterns"))
        {
            hanger.ClearSignsAndLanterns();
        }
    }
}
